using System.Net.Http.Json;
using mzansi_builds_api.DTOs.Collaboration;

namespace mzansi_builds_web.Services
{
    public class CollaborationService
    {
        private readonly HttpClient _http;
        public string? LastMessage { get; private set; }

        public CollaborationService(HttpClient http)
        {
            _http = http;
        }

        // --- Sending Requests ---
        public async Task<bool> SendCollaborationRequestAsync(CreateCollaborationRequestDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/collaboration/request", dto);
            if (response.IsSuccessStatusCode) return true;

            LastMessage = await response.Content.ReadAsStringAsync();
            return false;
        }

        // --- Fetching Data ---
        public async Task<List<CollaborationRequestResponseDto>> GetMySentRequestsAsync()
        {
            return await _http.GetFromJsonAsync<List<CollaborationRequestResponseDto>>("api/collaboration/my-sent-requests")
                   ?? new List<CollaborationRequestResponseDto>();
        }

        public async Task<List<CollaborationRequestResponseDto>> GetReceivedRequestsAsync()
        {
            return await _http.GetFromJsonAsync<List<CollaborationRequestResponseDto>>("api/collaboration/received")
                   ?? new List<CollaborationRequestResponseDto>();
        }

        public async Task<int> GetNotificationCountAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<int>("api/collaboration/notifications/count");
            }
            catch
            {
                return 0; // Fail silently for the badge
            }
        }

        // --- Actions ---
        public async Task<bool> WithdrawRequestAsync(int requestId)
        {
            var response = await _http.DeleteAsync($"api/collaboration/{requestId}/withdraw");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeclineRequestAsync(int requestId)
        {
            var response = await _http.PutAsync($"api/collaboration/{requestId}/decline", null);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Accepts a request and triggers the Octokit GitHub invitation
        /// </summary>
        public async Task<bool> AcceptRequestAsync(int requestId, string githubToken)
        {
            // We create a custom request to add the GitHub Token to the header
            var request = new HttpRequestMessage(HttpMethod.Put, $"api/collaboration/{requestId}/accept");
            request.Headers.Add("X-GitHub-Token", githubToken);

            var response = await _http.SendAsync(request);

            if (response.IsSuccessStatusCode) return true;

            LastMessage = await response.Content.ReadAsStringAsync();
            return false;
        }
    }
}