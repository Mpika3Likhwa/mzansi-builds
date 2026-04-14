using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Components.Authorization;

namespace mzansi_builds_web.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly AuthService _authService;

        public CustomAuthStateProvider(AuthService authService)
        {
            _authService = authService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                // Before I check the state, I need to make sure my AuthService 
                // has had a chance to look into LocalStorage for a saved token.
                await _authService.InitializeAsync();
            }
            catch
            {
                // If I'm prerendering on the server, JS Interop won't work yet.
                // In that case, I'll just return an anonymous state for now.
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var token = _authService.Token;

            // If I don't have a token, I'm treating the user as a guest (Anonymous).
            if (string.IsNullOrEmpty(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            // If I found a token, I'll decode it and build the user's identity.
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public void NotifyUserAuthentication()
        {
            // I'll call this when a login is successful to force the UI to refresh.
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void NotifyUserLogout()
        {
            // I'll call this during logout to clear the authentication state from the UI.
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            var claims = token.Claims.ToList();

            // Blazor's AuthorizeView and @context.User.Identity.Name specifically look 
            // for the ClaimTypes.Name. If my API uses 'unique_name' or 'name', I'll 
            // map it here to make sure the UI displays the username correctly.
            if (!claims.Any(c => c.Type == ClaimTypes.Name))
            {
                var nameClaim = claims.FirstOrDefault(c => c.Type == "unique_name" || c.Type == "name");
                if (nameClaim != null) claims.Add(new Claim(ClaimTypes.Name, nameClaim.Value));
            }

            return claims;
        }
    }
}