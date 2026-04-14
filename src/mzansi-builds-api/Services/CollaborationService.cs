using Microsoft.EntityFrameworkCore;
using mzansi_builds_api.Data;
using mzansi_builds_api.DTOs.Collaboration;
using mzansi_builds_api.Models;
using Octokit; 

namespace mzansi_builds_api.Services
{
    public class CollaborationService
    {
        private readonly AppDbContext _context;

        public CollaborationService(AppDbContext context)
        {
            _context = context;
        }

        // --- Core Methods ---

        public async Task<CollaborationRequestResponseDto> CreateRequestAsync(CreateCollaborationRequestDto dto, string requesterUserId)
        {
            // 1. Validate project exists
            var project = await _context.Projects.FindAsync(dto.ProjectId);
            if (project == null) throw new InvalidOperationException("Project not found.");

            // 2. Prevent requester from collaborating on their own project
            if (project.UserId == requesterUserId)
                throw new InvalidOperationException("You cannot request to collaborate on your own project.");

            // 3. Prevent duplicate pending requests
            var alreadyExists = await _context.CollaborationRequests
                .AnyAsync(r => r.ProjectId == dto.ProjectId &&
                               r.RequesterUserId == requesterUserId &&
                               r.Status == CollabStatus.Pending);

            if (alreadyExists) throw new InvalidOperationException("You already have a pending request for this project.");

            var request = new CollaborationRequest
            {
                ProjectId = dto.ProjectId,
                RequesterUserId = requesterUserId,
                Message = dto.Message,
                Status = CollabStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.CollaborationRequests.Add(request);
            await _context.SaveChangesAsync();

            return MapToDto(request);
        }

        // Fetch requests I have SENT (so I can withdraw them)
        public async Task<List<CollaborationRequestResponseDto>> GetMySentRequestsAsync(string userId)
        {
            var requests = await _context.CollaborationRequests
                .Include(r => r.Project)
                .Where(r => r.RequesterUserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return requests.Select(MapToDto).ToList();
        }

        // Fetch requests RECEIVED for my projects (to see the "New Request" signs)
        public async Task<List<CollaborationRequestResponseDto>> GetRequestsReceivedAsync(string currentUserId)
        {
            // Do a manual join between Requests, Projects, and Users
            var query = from req in _context.CollaborationRequests
                        join proj in _context.Projects on req.ProjectId equals proj.Id
                        join user in _context.Users on req.RequesterUserId equals user.Id.ToString()
                        where proj.UserId == currentUserId
                        select new CollaborationRequestResponseDto
                        {
                            Id = req.Id,
                            ProjectId = req.ProjectId,
                            ProjectTitle = proj.Title,
                            RequesterUserId = req.RequesterUserId,
                            // Use the Username from the User table join
                            RequesterName = user.Username,
                            Message = req.Message ?? string.Empty,
                            Status = req.Status.ToString(),
                            CreatedAt = req.CreatedAt
                        };

            return await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
        }

        // --- Action Methods ---

        public async Task<bool> WithdrawRequestAsync(int requestId, string requesterUserId)
        {
            var request = await _context.CollaborationRequests.FindAsync(requestId);

            if (request == null || request.RequesterUserId != requesterUserId)
                throw new UnauthorizedAccessException("Request not found or you don't own it.");

            if (request.Status != CollabStatus.Pending)
                throw new InvalidOperationException("Only pending requests can be withdrawn.");

            _context.CollaborationRequests.Remove(request);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<CollaborationRequestResponseDto> AcceptRequestAsync(int requestId, string ownerUserId, string ownerGithubToken)
        {
            var request = await _context.CollaborationRequests
                .Include(r => r.Project)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null || request.Project.UserId != ownerUserId)
                throw new UnauthorizedAccessException("Unauthorized.");

            // --- GITHUB INTEGRATION LOGIC ---
            // 1. Get the Requester's GitHub Username from the Users table
            var requester = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == request.RequesterUserId);
            if (string.IsNullOrEmpty(requester?.Username)) // Use your User model's GitHub field here
                throw new InvalidOperationException("Requester does not have a linked GitHub account.");

            // 2. Parse the Repo info (Assuming URL is github.com/owner/repo)
            var repoUri = new Uri(request.Project.GitHubRepoUrl);
            var segments = repoUri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var repoOwner = segments[0];
            var repoName = segments[1];

            try
            {
                // 3. Use Octokit to send the invite
                var client = new GitHubClient(new ProductHeaderValue("MzansiBuilds"))
                {
                    Credentials = new Credentials(ownerGithubToken)
                };

                await client.Repository.Collaborator.Add(repoOwner, repoName, requester.Username);

                // 4. Update Database
                request.Status = CollabStatus.Accepted;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"GitHub API Error: {ex.Message}");
            }

            return MapToDto(request);
        }

        public async Task<bool> DeclineRequestAsync(int requestId, string ownerUserId)
        {
            var request = await _context.CollaborationRequests
                .Include(r => r.Project)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null || request.Project.UserId != ownerUserId)
                throw new UnauthorizedAccessException();

            request.Status = CollabStatus.Declined;
            return await _context.SaveChangesAsync() > 0;
        }

        // --- Helpers ---

        private static CollaborationRequestResponseDto MapToDto(CollaborationRequest r)
        {
            return new CollaborationRequestResponseDto
            {
                Id = r.Id,
                ProjectId = r.ProjectId,
                ProjectTitle = r.Project?.Title ?? "Unknown Project",
                RequesterUserId = r.RequesterUserId,
                Message = r.Message ?? string.Empty,
                Status = r.Status.ToString(),
                CreatedAt = r.CreatedAt
            };
        }
    }
}