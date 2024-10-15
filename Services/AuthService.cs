
using DotNetEnv;
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

        public async Task<string> LoginAsync(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_backendUri}/api/collections/users/auth-with-password",
                new { 
                    identity = username, 
                    password = password
                }
                );
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<LoginResult>();
            return result.Token;
        }
    }
}

public class LoginResult
{
    public string Token { get; set; }
}