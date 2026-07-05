using System.Text.Json.Serialization;

namespace MetanetA_MobileApp.Model.Auth;

public class ForgotPasswordResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("developmentResetCode")]
    public string? DevelopmentResetCode { get; set; }
}
