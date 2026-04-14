using Microsoft.EntityFrameworkCore;
using mzansi_builds_api.Data;
using mzansi_builds_api.DTOs.Project;
using mzansi_builds_api.DTOs.ProjectStage;
using mzansi_builds_api.Models;
using System.Linq;

namespace mzansi_builds_api.Services
{
    public class ProjectService
    {
        private readonly AppDbContext _context;

        public ProjectService(AppDbContext context)
        {
            _context = context;
        }

        // CREATE: Project + Stages (Atomic)
        public async Task<ProjectResponseDto> CreateProjectAsync(CreateProjectDto dto, string userId)
        {
            var project = new Project
            {
                Title = dto.Title,
                Description = dto.Description,
                UserId = userId,
                CreatedAt = DateTime.UtcNow, // Set the timestamp here
                GitHubRepoUrl = dto.GitHubRepoUrl ?? string.Empty,
                IsGitHubRepoPrivate = dto.IsGitHubRepoPrivate,
                Stages = dto.Stages.Select(s => new ProjectStage
                {
                    Name = s.Name,
                    SupportRequired = s.SupportRequired,
                    IsCompleted = false
                }).ToList()
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            // Ensure stages are loaded for mapping
            await _context.Entry(project).Collection(p => p.Stages).LoadAsync();

            return MapToResponseDto(project);
        }

        // FEED: Get latest projects for the "Live Feed"
        public async Task<List<ProjectResponseDto>> GetLiveFeedAsync()
        {
            var projects = await _context.Projects
                .Include(p => p.Stages) // Eager loading stages
                .Where(p => !p.IsFullyCompleted)
                .OrderByDescending(p => p.CreatedAt) // Freshest first
                .ToListAsync();

            return projects.Select(p => MapToResponseDto(p)).ToList();
        }

        // CELEBRATION: Get only completed projects
        public async Task<List<ProjectResponseDto>> GetCelebrationWallAsync()
        {
            var projects = await _context.Projects
                .Include(p => p.Stages)
                .Where(p => p.IsFullyCompleted)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return projects.Select(p => MapToResponseDto(p)).ToList();
        }

        // Get a single project by id including stages
        public async Task<ProjectResponseDto?> GetProjectByIdAsync(int id)
        {
            var p = await _context.Projects
                .Include(x => x.Stages)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (p == null) return null;

            return MapToResponseDto(p);
        }

        // Calculates project progress (0-100) based on completed stages
        private int CalculateProgress(Project p)
        {
            var total = p.Stages?.Count ?? 0;
            if (total == 0) return 0;
            var completed = p.Stages.Count(s => s.IsCompleted);
            return (int)Math.Round((double)completed / total * 100);
        }

        // Helper method to keep code DRY (Don't Repeat Yourself)
        private ProjectResponseDto MapToResponseDto(Project p)
        {
            return new ProjectResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                DeveloperId = p.UserId,
                // I'm pulling the actual Username from the linked User record.
                DeveloperName = _context.Users.FirstOrDefault(u => u.Id.ToString() == p.UserId)?.Username ?? "Unknown",
                IsFullyCompleted = p.IsFullyCompleted,
                CreatedAt = p.CreatedAt,
                GitHubRepoUrl = p.GitHubRepoUrl ?? string.Empty,
                IsGitHubRepoPrivate = p.IsGitHubRepoPrivate,
                Stages = p.Stages.Select(s => new StageResponseDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    SupportRequired = s.SupportRequired,
                    IsCompleted = s.IsCompleted
                }).ToList(),
                ProgressPercentage = CalculateProgress(p)
            };
        }

        // Add this method to your API-side ProjectService.cs
        public async Task<List<ProjectResponseDto>> GetProjectsByUserIdAsync(string userId)
        {
            // I'm fetching all projects where the UserId matches the one from the token.
            // I also include the stages so I can calculate the progress on the profile.
            var projects = await _context.Projects
                .Include(p => p.Stages)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return projects.Select(p => MapToResponseDto(p)).ToList();
        }

        // Add this to your API-side ProjectService.cs
        // Inside your API-side ProjectService.cs
        public async Task<bool> CompleteStageAsync(int stageId, string userId)
        {
            // I need to include the stages so I can check the count accurately
            var stage = await _context.ProjectStages
                .Include(s => s.Project)
                .ThenInclude(p => p.Stages)
                .FirstOrDefaultAsync(s => s.Id == stageId);

            if (stage == null || stage.Project.UserId != userId)
            {
                return false;
            }

            stage.IsCompleted = true;

            // THE FIX: I need to make sure I'm checking the actual count of 
            // completed stages vs total stages before hiding it from the feed.
            var project = stage.Project;

            // I only mark it fully complete if every stage in the list is now true
            bool allStagesDone = project.Stages.All(s => s.IsCompleted);

            if (allStagesDone)
            {
                project.IsFullyCompleted = true;
            }
            else
            {
                // Just in case it was true before, I set it to false if 
                // there are still open stages.
                project.IsFullyCompleted = false;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}