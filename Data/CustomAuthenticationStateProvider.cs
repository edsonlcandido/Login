using LoginApp.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LoginApp.Data
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly AuthService _authenticationService;
        private readonly ILogger<CustomAuthenticationStateProvider> _logger;
        private string _token;

        public CustomAuthenticationStateProvider(AuthService authenticationService, ILogger<CustomAuthenticationStateProvider> logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity();

            if (!string.IsNullOrEmpty(_token))
            {
                identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "User"), // Ajuste conforme necessário
                    new Claim("Token", _token)
                }, "apiauth_type");
            }

            var user = new ClaimsPrincipal(identity);
            return await Task.FromResult(new AuthenticationState(user));
        }
        public async Task<bool> LoginAsync(string username, string password)
        {
            var token = await _authenticationService.LoginAsync(username, password);
            if (token != null)
            {
                _token = token;
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim("Token", _token)
                }, "apiauth_type");
                var user = new ClaimsPrincipal(identity);
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
                _logger.LogInformation("User {Username} logged in with token {Token}", username, token);

                return true;
            }
            _logger.LogWarning("Failed login attempt for user {Username}", username);
            return false;
        }
        public async Task LogoutAsync()
        {
            _token = null;
            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
            _logger.LogInformation("User logged out");
        }

    }
}