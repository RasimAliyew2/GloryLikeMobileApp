using System.Net.Http.Json;
using System.Text.Json;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.Services.Abstractions;

namespace MetanetA_MobileApp.Services.GetDataFromServer;

public sealed class AuthApiService : IAuthApiService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;

    public AuthApiService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<AuthApiResult<AuthUserDto>> RegisterAsync(AuthRegisterRequest request)
    {
        return await PostAuthAsync<AuthUserDto>("api/Auth/register", request);
    }

    public async Task<AuthApiResult<AuthUserDto>> LoginAsync(AuthLoginRequest request)
    {
        return await PostAuthAsync<AuthUserDto>("api/Auth/login", request);
    }

    public async Task<AuthApiResult<ForgotPasswordDto>> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        return await PostForgotAsync("api/Auth/forgot-password", request);
    }

    public async Task<AuthApiResult<object?>> ResetPasswordAsync(ResetPasswordRequest request)
    {
        return await PostEmptyAsync("api/Auth/reset-password", request);
    }

    public static AuthRegisterRequest BuildRegisterRequest(UserInfo userInfo, string password)
    {
        var email = (userInfo.Email ?? string.Empty).Trim();
        var phone = NormalizePhone(userInfo.PhoneNumber);
        var username = (userInfo.UserName ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(username))
        {
            username = !string.IsNullOrWhiteSpace(email)
                ? email.Split('@')[0].Trim()
                : phone;
        }

        return new AuthRegisterRequest
        {
            Name = (userInfo.Name ?? string.Empty).Trim(),
            Surname = (userInfo.Surname ?? string.Empty).Trim(),
            Username = username,
            PhoneNumber = phone,
            Email = email,
            Password = password
        };
    }

    public static UserInfo ApplyAuthUserToSession(UserInfo target, AuthUserDto? source, string password = "")
    {
        if (source is null)
            return target;

        target.Id = source.Id;
        target.Name = source.Name ?? string.Empty;
        target.Surname = source.Surname ?? string.Empty;
        target.UserName = source.Username ?? source.Username ?? string.Empty;
        target.Email = source.Email ?? string.Empty;
        target.PhoneNumber = source.PhoneNumber ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(password))
            target.Password = password;

        return target;
    }

    public static string NormalizePhone(string? value)
    {
        return new string((value ?? string.Empty).Where(char.IsDigit).ToArray());
    }

    private async Task<AuthApiResult<T>> PostAuthAsync<T>(string url, object request)
    {
        try
        {
            using var response = await _httpClient.PostAsJsonAsync(url, request, JsonOptions);
            var text = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return AuthApiResult<T>.Fail(ExtractErrorMessage(text, response.ReasonPhrase));

            if (string.IsNullOrWhiteSpace(text))
                return AuthApiResult<T>.Ok(default, "Success");

            var envelope = TryDeserialize<AuthEnvelope>(text);
            if (envelope is not null)
            {
                var user = envelope.User ?? envelope.Data;

                if (user is not null && typeof(T) == typeof(AuthUserDto))
                    return AuthApiResult<T>.Ok((T)(object)user, envelope.Message);

                if (!string.IsNullOrWhiteSpace(envelope.Message))
                    return AuthApiResult<T>.Ok(default, envelope.Message);
            }

            var direct = TryDeserialize<T>(text);
            return AuthApiResult<T>.Ok(direct, "Success");
        }
        catch (Exception ex)
        {
            return AuthApiResult<T>.Fail(ex.Message);
        }
    }

    private async Task<AuthApiResult<ForgotPasswordDto>> PostForgotAsync(string url, object request)
    {
        try
        {
            using var response = await _httpClient.PostAsJsonAsync(url, request, JsonOptions);
            var text = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return AuthApiResult<ForgotPasswordDto>.Fail(ExtractErrorMessage(text, response.ReasonPhrase));

            if (string.IsNullOrWhiteSpace(text))
                return AuthApiResult<ForgotPasswordDto>.Ok(new ForgotPasswordDto { Message = "Reset kod göndərildi." });

            var direct = TryDeserialize<ForgotPasswordDto>(text);
            if (direct is not null)
                return AuthApiResult<ForgotPasswordDto>.Ok(direct, direct.Message);

            var envelope = TryDeserialize<ForgotPasswordEnvelope>(text);
            if (envelope is not null)
            {
                return AuthApiResult<ForgotPasswordDto>.Ok(
                    new ForgotPasswordDto
                    {
                        Message = envelope.Message,
                        ResetCode = envelope.ResetCode,
                        Code = envelope.Code
                    },
                    envelope.Message);
            }

            return AuthApiResult<ForgotPasswordDto>.Ok(new ForgotPasswordDto { Message = text }, text);
        }
        catch (Exception ex)
        {
            return AuthApiResult<ForgotPasswordDto>.Fail(ex.Message);
        }
    }

    private async Task<AuthApiResult<object?>> PostEmptyAsync(string url, object request)
    {
        try
        {
            using var response = await _httpClient.PostAsJsonAsync(url, request, JsonOptions);
            var text = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return AuthApiResult<object?>.Fail(ExtractErrorMessage(text, response.ReasonPhrase));

            return AuthApiResult<object?>.Ok(null, ExtractErrorMessage(text, "Success"));
        }
        catch (Exception ex)
        {
            return AuthApiResult<object?>.Fail(ex.Message);
        }
    }

    private static T? TryDeserialize<T>(string text)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(text, JsonOptions);
        }
        catch
        {
            return default;
        }
    }

    private static string ExtractErrorMessage(string? responseText, string? fallback)
    {
        if (string.IsNullOrWhiteSpace(responseText))
            return fallback ?? "Server error.";

        try
        {
            using var doc = JsonDocument.Parse(responseText);
            var root = doc.RootElement;

            if (root.TryGetProperty("message", out var message))
                return message.GetString() ?? fallback ?? "Server error.";

            if (root.TryGetProperty("title", out var title))
                return title.GetString() ?? fallback ?? "Server error.";

            if (root.TryGetProperty("error", out var error))
                return error.GetString() ?? fallback ?? "Server error.";
        }
        catch
        {
            // Plain text response.
        }

        return responseText.Trim();
    }

    private sealed class AuthEnvelope
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public AuthUserDto? User { get; set; }
        public AuthUserDto? Data { get; set; }
    }

    private sealed class ForgotPasswordEnvelope
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ResetCode { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
