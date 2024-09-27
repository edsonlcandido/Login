

namespace Login.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("https://api.ligas.edsonluizcandido.com.br/api/collections/users/auth-with-password",
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