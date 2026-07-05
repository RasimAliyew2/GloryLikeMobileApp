using System.Text.Json.Serialization;

namespace MetanetA_MobileApp.Model.Auth;

public class ResetPasswordRequest
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("resetCode")]
    public string ResetCode { get; set; } = string.Empty;

    [JsonPropertyName("newPassword")]
    public string NewPassword { get; set; } = string.Empty;
}
