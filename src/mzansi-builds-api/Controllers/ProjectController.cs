using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mzansi_builds_api.DTOs.Project;
using mzansi_builds_api.Services;
using System.Security.Claims;

namespace mzansi_builds_api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly ProjectService _projectService;

        public ProjectController(ProjectService projectService)
        {
            _projectService = projectService;
        }

        /// <summary>
        /// I'm adding this endpoint so the Profile page can fetch only my projects.
        /// It grabs my ID from the JWT and asks the service for the filtered list.
        /// </summary>
        [HttpGet("my-projects")]
        public async Task<ActionResult<List<ProjectResponseDto>>> GetMyProjects()
        {
            // I'm extracting my ID from the token claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("I couldn't find your User ID in the token.");
            }

            // I'm calling the service method we just defined
            var myProjects = await _projectService.GetProjectsByUserIdAsync(userId);
            return Ok(myProjects);
        }

        [HttpPost("create")]
        public async Task<ActionResult<ProjectResponseDto>> CreateProject(CreateProjectDto dto)
        {
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

        // Add this to your API-side ProjectController.cs
        [HttpPut("stages/{stageId}/complete")]
        public async Task<IActionResult> CompleteStage(int stageId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var success = await _projectService.CompleteStageAsync(stageId, userId);

            if (!success)
            {
                return BadRequest("I couldn't complete the stage. Either it doesn't exist or you don't own it.");
            }

            return Ok(new { message = "Stage marked as complete!" });
        }

        [HttpGet("feed")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ProjectResponseDto>>> GetLiveFeed()
        {
            var feed = await _projectService.GetLiveFeedAsync();
            return Ok(feed);
        }

        [HttpGet("celebration-wall")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ProjectResponseDto>>> GetCelebrationWall()
        {
            var wall = await _projectService.GetCelebrationWallAsync();
            return Ok(wall);
        }
    }
}