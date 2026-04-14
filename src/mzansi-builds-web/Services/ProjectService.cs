using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using mzansi_builds_api.DTOs.Collaboration;
using mzansi_builds_api.DTOs.Project;

namespace mzansi_builds_web.Services
{
    public partial class ProjectService
    {
        private readonly HttpClient _httpClient;

        // Last human-readable message returned by the API (success or error)
        public string? LastMessage { get; private set; } = string.Empty;

        // Convenience: holds last created project (optional)
        public ProjectResponseDto? LastCreatedProject { get; private set; }

        public ProjectService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Fetches all projects for the feed (includes stages)
        public async Task<List<ProjectResponseDto>> GetAllProjectsAsync()
        {
            LastMessage = null;

            try
            {
                using var resp = await _httpClient.GetAsync("api/project/feed");
                if (!resp.IsSuccessStatusCode)
                {
                    LastMessage = await SafeReadContentAsync(resp);
                    return new List<ProjectResponseDto>();
                }

                var projects = await resp.Content.ReadFromJsonAsync<List<ProjectResponseDto>>();
                return projects ?? new List<ProjectResponseDto>();
            }
            catch (Exception ex)
            {
                LastMessage = ex.Message;
                return new List<ProjectResponseDto>();
            }
        }

        // Fetch a single project by id (includes stages)
        public async Task<ProjectResponseDto?> GetProjectByIdAsync(int id)
        {
            LastMessage = null;

            try
            {
                using var resp = await _httpClient.GetAsync($"api/project/{id}");
                if (!resp.IsSuccessStatusCode)
                {
                    LastMessage = await SafeReadContentAsync(resp);
                    return null;
                }

                var project = await resp.Content.ReadFromJsonAsync<ProjectResponseDto>();
                return project;
            }
            catch (Exception ex)
            {
                LastMessage = ex.Message;
                return null;
            }
        }

        // Create a new project (with initial stages)
        // Returns true on success; LastMessage will contain API message on failure or additional info.
        public async Task<bool> CreateProjectAsync(CreateProjectDto projectDto)
        {
            LastMessage = null;
            LastCreatedProject = null;

            try
            {
                using var resp = await _httpClient.PostAsJsonAsync("api/project/create", projectDto);

                if (!resp.IsSuccessStatusCode)
                {
                    LastMessage = await SafeReadContentAsync(resp);
                    return false;
                }

                // Attempt to read the created project response
                try
                {
                    var created = await resp.Content.ReadFromJsonAsync<ProjectResponseDto>();
                    LastCreatedProject = created;
                }
                catch
                {
                    // ignore parse errors; API might have returned a simple message
                }

                // Capture any message if present
                var maybeMsg = await TryReadJsonMessageAsync(resp);
                if (!string.IsNullOrWhiteSpace(maybeMsg)) LastMessage = maybeMsg;

                return true;
            }
            catch (Exception ex)
            {
                LastMessage = ex.Message;
                return false;
            }
        }

        // Request collaboration on a project
        // Returns true on success; LastMessage will contain API message on failure or additional info.
        public async Task<bool> RequestCollaborationAsync(CreateCollaborationRequestDto requestDto)
        {
            LastMessage = null;

            try
            {
                using var resp = await _httpClient.PostAsJsonAsync("api/collaboration", requestDto);

                if (!resp.IsSuccessStatusCode)
                {
                    LastMessage = await SafeReadContentAsync(resp);
                    return false;
                }

                var maybeMsg = await TryReadJsonMessageAsync(resp);
                if (!string.IsNullOrWhiteSpace(maybeMsg))
                    LastMessage = maybeMsg;
                else
                    LastMessage = "Collaboration request sent.";

                return true;
            }
            catch (Exception ex)
            {
                LastMessage = ex.Message;
                return false;
            }
        }

        // Helpers

        private static async Task<string?> TryReadJsonMessageAsync(HttpResponseMessage resp)
        {
            try
            {
                // Try reading a common { message: "..." } wrapper
                var wrapper = await resp.Content.ReadFromJsonAsync<ApiMessageResponse>();
                return wrapper?.Message;
            }
            catch
            {
                return null;
            }
        }

        private static async Task<string> SafeReadContentAsync(HttpResponseMessage resp)
        {
            try
            {
                return await resp.Content.ReadAsStringAsync();
            }
            catch
            {
                return $"Request failed ({(int)resp.StatusCode})";
            }
        }

        // Add this method to your web-side ProjectService.cs
        public async Task<List<ProjectResponseDto>> GetMyProjectsAsync()
        {
            LastMessage = null;

            try
            {
                // I'm calling the new endpoint we just added to the controller.
                // Because of our AuthService, the Bearer token is already in the header.
                using var resp = await _httpClient.GetAsync("api/project/my-projects");

                if (!resp.IsSuccessStatusCode)
                {
                    LastMessage = await SafeReadContentAsync(resp);
                    return new List<ProjectResponseDto>();
                }

                var projects = await resp.Content.ReadFromJsonAsync<List<ProjectResponseDto>>();
                return projects ?? new List<ProjectResponseDto>();
            }
            catch (Exception ex)
            {
                LastMessage = ex.Message;
                return new List<ProjectResponseDto>();
            }
        }

        // Add this to your web-side ProjectService.cs
        public async Task<bool> CompleteStageAsync(int stageId)
        {
            LastMessage = null;
            try
            {
                // I'm calling the completion endpoint I just created
                var resp = await _httpClient.PutAsync($"api/project/stages/{stageId}/complete", null);

                if (resp.IsSuccessStatusCode)
                {
                    LastMessage = "Stage completed!";
                    return true;
                }

                LastMessage = await SafeReadContentAsync(resp);
                return false;
            }
            catch (Exception ex)
            {
                LastMessage = ex.Message;
                return false;
            }
        }

        // Matches the API message wrapper used in several endpoints
        private class ApiMessageResponse
        {
            public string Message { get; set; } = string.Empty;
        }
    }
}
