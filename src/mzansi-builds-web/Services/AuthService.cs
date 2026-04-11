using System.Net.Http.Json;
using mzansi_builds_api.DTOs.User; 

namespace mzansi_builds_web.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        // Stores the JWT in memory for the duration of the browser session
        public string? Token { get; private set; }

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // 1. REGISTER using UserRegistrationDto
        public async Task<bool> RegisterAsync(UserRegistrationDto registrationDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", registrationDto);
            return response.IsSuccessStatusCode;
        }

        // 2. LOGIN using UserLoginDto
        public async Task<bool> LoginAsync(UserLoginDto loginDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginDto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result != null)
                {
                    Token = result.Token;
                    return true;
                }
            }
            return false;
        }
    }

    // This matches the structure of the JSON returned by your Login endpoint
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    // Generic API message wrapper used by register/login responses (e.g. { message: "..." })
    internal class ApiMessageResponse
    {
        public string Message { get; set; } = string.Empty;
    }
}