using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.Services;
using MetanetA_MobileApp.Services.Abstractions;
using MetanetA_MobileApp.View;

namespace MetanetA_MobileApp.ViewModels.Sign;

[QueryProperty(nameof(OperationType), "OperationType")]
public partial class SetPasswordViewModel : ObservableObject
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IUserSession userSession;
    private readonly HttpClient httpClient;

    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string confirmPassword = string.Empty;
    [ObservableProperty] private bool isMismatch;
    [ObservableProperty] private string mismatchText = "uyğun gəlmir";
    [ObservableProperty] private OperationType operationType;

    [ObservableProperty] private bool isPasswordHidden = true;
    [ObservableProperty] private bool isConfirmPasswordHidden = true;

    [ObservableProperty] private bool isPasswordRuleInvalid;
    [ObservableProperty] private string passwordRuleText = string.Empty;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string errorMessage = string.Empty;

    public SetPasswordViewModel(IUserSession userSession, HttpClient httpClient)
    {
        this.userSession = userSession;
        this.httpClient = httpClient;
    }

    partial void OnPasswordChanged(string value)
    {
        ValidatePasswordRules();
        UpdateMismatch();
    }

    partial void OnConfirmPasswordChanged(string value)
    {
        ValidatePasswordRules();
        UpdateMismatch();
    }

    private void UpdateMismatch()
    {
        IsMismatch = !string.IsNullOrWhiteSpace(ConfirmPassword)
                     && !string.Equals(Password, ConfirmPassword, StringComparison.Ordinal);
    }

    private void ValidatePasswordRules()
    {
        var pwd = (Password ?? string.Empty).Trim();
        var minLen = pwd.Length >= 8;
        var hasUpper = pwd.Any(char.IsUpper);

        if (string.IsNullOrWhiteSpace(pwd))
        {
            IsPasswordRuleInvalid = false;
            PasswordRuleText = string.Empty;
            return;
        }

        if (!minLen || !hasUpper)
        {
            IsPasswordRuleInvalid = true;

            PasswordRuleText = (!minLen, !hasUpper) switch
            {
                (true, true) => "Parol minimum 8 simvol olmalı və ən azı 1 böyük hərf (A-Z) içərməlidir.",
                (true, false) => "Parol minimum 8 simvol olmalıdır.",
                _ => "Parolda ən azı 1 böyük hərf (A-Z) olmalıdır."
            };
        }
        else
        {
            IsPasswordRuleInvalid = false;
            PasswordRuleText = string.Empty;
        }
    }

    [RelayCommand]
    private void TogglePasswordVisibility()
    {
        IsPasswordHidden = !IsPasswordHidden;
    }

    [RelayCommand]
    private void ToggleConfirmPasswordVisibility()
    {
        IsConfirmPasswordHidden = !IsConfirmPasswordHidden;
    }

    [RelayCommand]
    private async Task ConfirmAsync()
    {
        ValidatePasswordRules();
        UpdateMismatch();
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            IsMismatch = true;
            MismatchText = "parol boş ola bilməz";
            return;
        }

        if (IsPasswordRuleInvalid || IsMismatch)
            return;

        if (userSession.CurrentUser is null)
        {
            ErrorMessage = "Session-da istifadəçi məlumatı yoxdur.";
            return;
        }

        userSession.CurrentUser.Password = Password;

        if (OperationType == OperationType.SetPassword)
            await RegisterUserAsync();
        else if (OperationType == OperationType.ChangePassword)
            await Shell.Current.GoToAsync($"//{nameof(SignInPage)}");
    }

    private async Task RegisterUserAsync()
    {
        if (IsBusy)
            return;

        var user = userSession.CurrentUser!;

        NormalizeUserForRegister(user);

        var validation = ValidateUserForRegister(user);
        if (!string.IsNullOrWhiteSpace(validation))
        {
            ErrorMessage = validation;
            return;
        }

        try
        {
            IsBusy = true;

            var response = await httpClient.PostAsJsonAsync("api/Auth/register", new
            {
                UserName = user.UserName,
                Name = user.Name,
                Surname = user.Surname,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Password = user.Password
            });

            var result = await ReadAuthResponse(response);

            if (!response.IsSuccessStatusCode || result is null || !result.Success)
            {
                ErrorMessage = result?.Message ?? "Qeydiyyat alınmadı.";
                return;
            }

            if (result.User is not null)
            {
                user.Id = result.User.Id;
                user.UserName = result.User.UserName ?? user.UserName;
                user.Name = result.User.Name ?? user.Name;
                user.Surname = result.User.Surname ?? user.Surname;
                user.PhoneNumber = result.User.PhoneNumber ?? user.PhoneNumber;
                user.Email = result.User.Email ?? user.Email;
            }

            userSession.CurrentUser = user;
            await Shell.Current.GoToAsync($"//{nameof(SignInPage)}");
        }
        catch (Exception ex)
        {
            ErrorMessage = "Backend-ə qoşulmaq olmadı: " + ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static void NormalizeUserForRegister(UserInfo user)
    {
        user.Name = (user.Name ?? string.Empty).Trim();
        user.Surname = (user.Surname ?? string.Empty).Trim();
        user.PhoneNumber = (user.PhoneNumber ?? string.Empty).Trim();
        user.Email = (user.Email ?? string.Empty).Trim().ToLowerInvariant();
        user.UserName = (user.UserName ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(user.UserName))
        {
            if (!string.IsNullOrWhiteSpace(user.Email) && user.Email.Contains('@'))
                user.UserName = user.Email.Split('@')[0];
            else if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
                user.UserName = "user" + new string(user.PhoneNumber.Where(char.IsDigit).ToArray());
        }
    }

    private static string ValidateUserForRegister(UserInfo user)
    {
        if (string.IsNullOrWhiteSpace(user.Name)) return "Ad boş ola bilməz.";
        if (string.IsNullOrWhiteSpace(user.Surname)) return "Soyad boş ola bilməz.";
        if (string.IsNullOrWhiteSpace(user.PhoneNumber)) return "Telefon nömrəsi boş ola bilməz.";
        if (string.IsNullOrWhiteSpace(user.Email)) return "Email boş ola bilməz.";
        if (string.IsNullOrWhiteSpace(user.UserName)) return "Username boş ola bilməz.";
        if (string.IsNullOrWhiteSpace(user.Password) || user.Password.Length < 8) return "Password minimum 8 simvol olmalıdır.";
        return string.Empty;
    }

    private async Task<AuthResponse?> ReadAuthResponse(HttpResponseMessage response)
    {
        try
        {
            return await response.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions);
        }
        catch
        {
            var text = await response.Content.ReadAsStringAsync();
            return new AuthResponse
            {
                Success = false,
                Message = string.IsNullOrWhiteSpace(text) ? "Server cavabı oxunmadı." : text
            };
        }
    }

    private sealed class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public AuthUserDto? User { get; set; }
    }

    private sealed class AuthUserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
