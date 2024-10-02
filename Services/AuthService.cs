

namespace LoginApp.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuraion;
        public AuthService(HttpClient httpClient, IConfiguration configuraion)
        {
            _httpClient = httpClient;
            _configuraion = configuraion;
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            var baseUrl = _configuraion["ApiBaseUrl"];
            var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/api/collections/users/auth-with-password",
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