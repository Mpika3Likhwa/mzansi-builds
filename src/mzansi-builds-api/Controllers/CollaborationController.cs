using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using mzansi_builds_api.DTOs.Collaboration;
using mzansi_builds_api.Services;

namespace mzansi_builds_api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CollaborationController : ControllerBase
    {
        private readonly CollaborationService _collabService;

        public CollaborationController(CollaborationService collabService)
        {
            _collabService = collabService;
        }

        // --- REQUESTER ENDPOINTS ---

        [HttpPost("request")]
        public async Task<ActionResult<CollaborationRequestResponseDto>> CreateRequest([FromBody] CreateCollaborationRequestDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            try
            {
                var result = await _collabService.CreateRequestAsync(dto, userId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("my-sent-requests")]
        public async Task<ActionResult<List<CollaborationRequestResponseDto>>> GetMySentRequests()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var results = await _collabService.GetMySentRequestsAsync(userId);
            return Ok(results);
        }

        [HttpDelete("{id}/withdraw")]
        public async Task<IActionResult> WithdrawRequest(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            try
            {
                await _collabService.WithdrawRequestAsync(id, userId);
                return Ok(new { message = "Request withdrawn successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // --- PROJECT OWNER ENDPOINTS ---

        [HttpGet("received")]
        public async Task<ActionResult<List<CollaborationRequestResponseDto>>> GetReceivedRequests()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var results = await _collabService.GetRequestsReceivedAsync(userId);
            return Ok(results);
        }

        [HttpPut("{id}/accept")]
        public async Task<ActionResult<CollaborationRequestResponseDto>> AcceptRequest(int id, [FromHeader(Name = "X-GitHub-Token")] string githubToken)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            if (string.IsNullOrEmpty(githubToken))
                return BadRequest("GitHub token is required to invite collaborators to your repo.");

            try
            {
                // We pass the token from the UI so the API can act on the owner's behalf
                var updated = await _collabService.AcceptRequestAsync(id, userId, githubToken);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/decline")]
        public async Task<IActionResult> DeclineRequest(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            try
            {
                await _collabService.DeclineRequestAsync(id, userId);
                return Ok(new { message = "Request declined." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // --- NOTIFICATION ENDPOINT ---

        [HttpGet("notifications/count")]
        public async Task<ActionResult<int>> GetPendingNotificationCount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // Fetch received requests and count only the 'Pending' ones
            var received = await _collabService.GetRequestsReceivedAsync(userId);
            var pendingCount = received.Count(r => r.Status == "Pending");

            return Ok(pendingCount);
        }
    }
}