using Microsoft.EntityFrameworkCore;
using mzansi_builds_api.Data;
using mzansi_builds_api.DTOs.Project;
using mzansi_builds_api.DTOs.ProjectStage;
using mzansi_builds_api.Models;

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
                Stages = dto.Stages.Select(s => new ProjectStage
                {
                    Name = s.Name,
                    SupportRequired = s.SupportRequired,
                    IsCompleted = false
                }).ToList()
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return MapToResponseDto(project);
        }

        // FEED: Get latest projects for the "Live Feed"
        public async Task<List<ProjectResponseDto>> GetLiveFeedAsync()
        {
            var projects = await _context.Projects
                .Include(p => p.Stages)
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

        // Helper method to keep code DRY (Don't Repeat Yourself)
        private ProjectResponseDto MapToResponseDto(Project p)
        {
            return new ProjectResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                DeveloperId = p.UserId,
                IsFullyCompleted = p.IsFullyCompleted,
                CreatedAt = p.CreatedAt,
                Stages = p.Stages.Select(s => new StageResponseDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    SupportRequired = s.SupportRequired,
                    IsCompleted = s.IsCompleted
                }).ToList()
            };
        }
    }
}