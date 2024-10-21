using LoginApp.Models;
using System.Text.Json.Serialization;

namespace LoginApp.Responses
{
    public class LoginResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
        // esse campo do retorno JSOn não se chama User mas record
        [JsonPropertyName("record")]
        public ApiUserModel User { get; set; }
    }
}
