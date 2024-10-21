using LoginApp.Models;
using LoginApp.Responses;
using LoginApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LoginApp.Providers
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ProtectedLocalStorage _localStorage;
        private readonly JwtService _jwtService;
        private string? _token;
        private ApiUserModel? _user;
        public CustomAuthenticationStateProvider(ProtectedLocalStorage localStorage, JwtService jwtService)
        {
            _localStorage = localStorage;
            _jwtService = jwtService;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {

            if (string.IsNullOrEmpty(_token))
            {
                var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
                return new AuthenticationState(anonymousUser);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _user.Username),
                new Claim(ClaimTypes.Email, _user.Email),
                new Claim(ClaimTypes.Role, _user.Role),
                new Claim("Token", _token)
            };
            var identity = new ClaimsIdentity(claims, "apiauth_type");
            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        public async Task InitializeAsync()
        {
            //var result = await _localStorage.GetAsync<string>("authToken");
            //if (result.Success && _jwtService.ValidateToken(result.Value))
            //{
            //    string authToken = result.Value;
            //    // Usar o JwtService para obter as claims do token
            //    var authTokenClaims = _jwtService.GetClaimsFromToken(authToken).ToList();
            //    var authTokenIdentity = new ClaimsIdentity(authTokenClaims, "apiauth_type");
            //    var authTokenUser = new ClaimsPrincipal(authTokenIdentity);
            //    NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            //}
        }

        internal ClaimsPrincipal MarkUserAsAuthenticated(LoginResponse loginResponse)
        {
            _token = loginResponse.Token;
            _user = loginResponse.User;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _user.Username),
                new Claim(ClaimTypes.Email, _user.Email),
                new Claim(ClaimTypes.Role, _user.Role),
                new Claim("Token", _token)
            };
            var identity = new ClaimsIdentity(claims, "apiauth_type");
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
            return user;
        }
    }
}