using System.Text.Json.Serialization;

namespace MetanetA_MobileApp.Model.Auth;

public class ForgotPasswordRequest
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
}
