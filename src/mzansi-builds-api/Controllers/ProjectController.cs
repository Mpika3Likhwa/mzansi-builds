using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mzansi_builds_api.DTOs.Project;
using mzansi_builds_api.Services;
using System.Security.Claims;

namespace mzansi_builds_api.Controllers
{
    [Authorize] // Protects all endpoints in this controller
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly ProjectService _projectService;

        public ProjectController(ProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpPost("create")]
        public async Task<ActionResult<ProjectResponseDto>> CreateProject(CreateProjectDto dto)
        {
            // Extract the NameIdentifier (User ID) from the JWT claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            try
            {
                var result = await _projectService.CreateProjectAsync(dto, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating project: {ex.Message}");
            }
        }

        [HttpGet("feed")]
        [AllowAnonymous] // Anyone can see the live feed
        public async Task<ActionResult<List<ProjectResponseDto>>> GetLiveFeed()
        {
            var feed = await _projectService.GetLiveFeedAsync();
            return Ok(feed);
        }

        [HttpGet("celebration-wall")]
        [AllowAnonymous] // Anyone can see the celebration wall
        public async Task<ActionResult<List<ProjectResponseDto>>> GetCelebrationWall()
        {
            var wall = await _projectService.GetCelebrationWallAsync();
            return Ok(wall);
        }
    }
}