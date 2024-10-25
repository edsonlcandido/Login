using LoginApp.Responses;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Text.Json;

namespace LoginApp.Services
{
    public class LocalStorageService
    {
        private ProtectedSessionStorage _protectedSessionStorage;
        private readonly string _key;

        public LocalStorageService(ProtectedSessionStorage protectedSessionStorage)
        {
            _protectedSessionStorage = protectedSessionStorage;
        }
        public async Task PersistUserToBrowser(LoginResponse loginResponse)
        {
            string userJson = JsonSerializer.Serialize(loginResponse.User);
            await _protectedSessionStorage.SetAsync(_key, userJson);
        }
        public async Task<LoginResponse> GetUserFromBrowser()
        {
            var result = await _protectedSessionStorage.GetAsync<string>(_key);
            var userJson = result.Success ? result.Value : null;
            if (string.IsNullOrEmpty(userJson))
            {
                return null;
            }
            return JsonSerializer.Deserialize<LoginResponse>(userJson);
        }
        public async Task RemoveUserFromBrowser()
        {
            await _protectedSessionStorage.DeleteAsync(_key);
        }
    }
}
