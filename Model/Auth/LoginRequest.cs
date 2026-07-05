using System.Text.Json.Serialization;

namespace MetanetA_MobileApp.Model.Auth;

public class LoginRequest
{
    [JsonPropertyName("login")]
    public string Login { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
}
