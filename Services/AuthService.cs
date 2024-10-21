using DotNetEnv;
using LoginApp.Responses;

namespace LoginApp.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _backendUri;
        public AuthService(HttpClient httpClient)
        {
            Env.Load();
            _httpClient = httpClient;
            _backendUri = Env.GetString("BACKEND_URI");
        }

        public async Task<LoginResponse> LoginAsync(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_backendUri}/api/collections/users/auth-with-password",
                new { 
                    identity = username, 
                    password = password
                }
                );
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return result;
        }
    }
}

public class LoginResult
{
    public string Token { get; set; }
}