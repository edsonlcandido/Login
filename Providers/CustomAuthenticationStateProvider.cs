using LoginApp.Models;
using LoginApp.Responses;
using LoginApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LoginApp.Providers
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private string? _token;
        private ApiUserModel? _user;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CustomAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor)
        {
            
            _httpContextAccessor = httpContextAccessor;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = _httpContextAccessor.HttpContext.User;
            return new AuthenticationState(user);
        }

        public async Task MarkUserAsAuthenticated(LoginResponse loginResponse)
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
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var user = new ClaimsPrincipal(identity);

            var props = new AuthenticationProperties{
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
            };

            var HttpContext = _httpContextAccessor.HttpContext;
            
            if (HttpContext != null)
            {
                await HttpContext.SignInAsync(user);
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
            }
            //write cookie in local storage
        }
    }
}