using System.Text.Json.Serialization;

namespace MetanetA_MobileApp.Model.Auth;

public class AuthResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("user")]
    public AuthUserDto? User { get; set; }
}
