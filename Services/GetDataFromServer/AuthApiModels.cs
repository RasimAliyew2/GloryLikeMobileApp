using System.Text.Json.Serialization;

namespace MetanetA_MobileApp.Services.GetDataFromServer;

public sealed class AuthApiResult<T>
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public T? Data { get; set; }

    public static AuthApiResult<T> Ok(T? data, string message = "") => new()
    {
        Success = true,
        Data = data,
        Message = message
    };

    public static AuthApiResult<T> Fail(string message) => new()
    {
        Success = false,
        Message = string.IsNullOrWhiteSpace(message) ? "Server error." : message
    };
}

public sealed class AuthRegisterRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("surname")]
    public string Surname { get; set; } = string.Empty;

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
}

public sealed class AuthLoginRequest
{
    [JsonPropertyName("emailOrPhone")]
    public string EmailOrPhone { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
}

public sealed class ForgotPasswordRequest
{
    [JsonPropertyName("emailOrPhone")]
    public string EmailOrPhone { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; } = string.Empty;
}

public sealed class ResetPasswordRequest
{
    [JsonPropertyName("emailOrPhone")]
    public string EmailOrPhone { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("resetCode")]
    public string ResetCode { get; set; } = string.Empty;

    // Some backend DTOs call the same field "code". Sending both is harmless for ASP.NET Core model binding.
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("newPassword")]
    public string NewPassword { get; set; } = string.Empty;

    // Some backend DTOs call the same field "password". Sending both is harmless for ASP.NET Core model binding.
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
}

public sealed class AuthUserDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("surname")]
    public string Surname { get; set; } = string.Empty;

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
}

public sealed class AuthEnvelopeDto
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("user")]
    public AuthUserDto? User { get; set; }

    [JsonPropertyName("data")]
    public AuthUserDto? Data { get; set; }

    [JsonPropertyName("resetCode")]
    public string? ResetCode { get; set; }

    [JsonPropertyName("code")]
    public string? Code { get; set; }
}

public sealed class ForgotPasswordDto
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("resetCode")]
    public string? ResetCode { get; set; }

    [JsonPropertyName("code")]
    public string? Code { get; set; }
}
