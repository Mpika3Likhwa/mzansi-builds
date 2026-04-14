using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using mzansi_builds_api.DTOs.User;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace mzansi_builds_web.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly IJSRuntime _jsRuntime;
        private string? _token;

        // I'm keeping the token in a private field and exposing it here 
        // to make sure I have a single source of truth for the app's auth state.
        public string? Token => _token;

        // I'll use this to store the latest feedback from the API so I can 
        // easily display success or error messages in my components.
        public string? LastMessage { get; private set; }

        public bool IsAuthenticated => !string.IsNullOrEmpty(_token);

        public AuthService(HttpClient httpClient, IServiceProvider serviceProvider, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _serviceProvider = serviceProvider;
            _jsRuntime = jsRuntime;
        }

        /// <summary>
        /// I need to call this to pull my saved token out of the browser's local storage.
        /// It helps me stay logged in even if I refresh the page.
        /// </summary>
        public async Task InitializeAsync()
        {
            if (string.IsNullOrEmpty(_token))
            {
                try
                {
                    // I'm reaching into localStorage to see if I have a session saved.
                    var savedToken = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
                    if (!string.IsNullOrEmpty(savedToken))
                    {
                        _token = savedToken;
                        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                    }
                }
                catch
                {
                    // I'm catching this because JS Interop isn't available during 
                    // the initial server-side prerender. It'll run correctly once interactive.
                }
            }
        }

        public async Task<bool> RegisterAsync(UserRegistrationDto registrationDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", registrationDto);

            try
            {
                var apiResp = await response.Content.ReadFromJsonAsync<ApiMessageResponse>();
                LastMessage = apiResp?.Message;
            }
            catch
            {
                try { LastMessage = await response.Content.ReadAsStringAsync(); }
                catch { LastMessage = null; }
            }

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> LoginAsync(UserLoginDto loginDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginDto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result != null && !string.IsNullOrEmpty(result.Token))
                {
                    _token = result.Token;
                    LastMessage = result.Message;

                    // I'm saving the token to the browser so the session persists across redirects.
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", _token);

                    // I need to set this header immediately so my subsequent API calls are authorized.
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

                    // I have to notify my CustomAuthStateProvider here so the UI 
                    // reacts immediately to the successful login.
                    try
                    {
                        var provider = _serviceProvider.GetService<AuthenticationStateProvider>();
                        if (provider is CustomAuthStateProvider customProvider)
                        {
                            customProvider.NotifyUserAuthentication();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"I failed to notify the AuthStateProvider: {ex.Message}");
                    }

                    return true;
                }
            }
            else
            {
                try
                {
                    var apiResp = await response.Content.ReadFromJsonAsync<ApiMessageResponse>();
                    LastMessage = apiResp?.Message ?? "Login failed. I should check my credentials.";
                }
                catch
                {
                    LastMessage = "An error occurred while I was trying to log in.";
                }
            }

            return false;
        }

        public async Task Logout()
        {
            _token = null;
            LastMessage = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;

            // I'm clearing the browser storage so I'm fully logged out.
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");

            try
            {
                var provider = _serviceProvider.GetService<AuthenticationStateProvider>();
                if (provider is CustomAuthStateProvider customProvider)
                {
                    customProvider.NotifyUserLogout();
                }
            }
            catch { /* Best effort logout notification */ }
        }
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    internal class ApiMessageResponse
    {
        public string Message { get; set; } = string.Empty;
    }
}