﻿using LoginApp.Models;
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
        private readonly IJSRuntime _jsRuntime;
        private bool _isInitialized;
        private string? authToken;
        private string? _token;
        private ApiUserModel? _user;
        public CustomAuthenticationStateProvider(ProtectedLocalStorage localStorage, 
            JwtService jwtService, IJSRuntime jSRuntime)
        {
            _localStorage = localStorage;
            _jwtService = jwtService;
            _jsRuntime = jSRuntime;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                //var result = await _localStorage.GetAsync<string>("authToken");
                //authToken = result.Success ? result.Value : null;
                //_isInitialized = true;
                if (authToken != null)
                {
                    var claims = _jwtService.GetClaimsFromToken(authToken);
                    var identity = new ClaimsIdentity(claims, "apiauth_type");
                    var user = new ClaimsPrincipal(identity);
                    NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                    return new AuthenticationState(user);
                }
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
            catch
            {
                var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                return new AuthenticationState(anonymousUser);
            }
        }

        public async Task InitializeAsync()
        {
            authToken = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
            _isInitialized = true;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
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