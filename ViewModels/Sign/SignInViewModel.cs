using System.Net.Http.Json;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.Services;
using MetanetA_MobileApp.Services.Abstractions;
using MetanetA_MobileApp.View;
using MetanetA_MobileApp.View.SignUp;

namespace MetanetA_MobileApp.ViewModel;

public partial class SignInViewModel : ObservableObject
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IUserSession _userSession;
    private readonly HttpClient _httpClient;

    [ObservableProperty] private string login = string.Empty;
    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string phoneNumber = string.Empty;
    [ObservableProperty] private string password = string.Empty;

    [ObservableProperty] private bool fillTheArea;
    [ObservableProperty] private bool invalidCredentials;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string errorMessage = string.Empty;

    [ObservableProperty] private bool isPasswordHidden = true;
    [ObservableProperty] private bool isConfirmPasswordHidden = true;

    public SignInViewModel(IUserSession userSession, HttpClient httpClient)
    {
        _userSession = userSession;
        _httpClient = httpClient;
    }

    [RelayCommand]
    public async Task SignIn()
    {
        if (IsBusy)
            return;

        var loginValue = FirstNonEmpty(Login, Email, PhoneNumber);

        FillTheArea = false;
        InvalidCredentials = false;
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(loginValue) || string.IsNullOrWhiteSpace(Password))
        {
            FillTheArea = true;
            ErrorMessage = "Email, username və ya telefon + password daxil et.";
            return;
        }

        try
        {
            IsBusy = true;

            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", new
            {
                Login = loginValue.Trim(),
                Password
            });

            var result = await ReadAuthResponse(response);

            if (!response.IsSuccessStatusCode || result is null || !result.Success)
            {
                InvalidCredentials = true;
                ErrorMessage = result?.Message ?? "Email/username/telefon və ya password yanlışdır.";
                return;
            }

            SaveLoggedInUser(result.User, loginValue.Trim());
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }
        catch (Exception ex)
        {
            InvalidCredentials = true;
            ErrorMessage = "Backend-ə qoşulmaq olmadı: " + ex.Message;
        }
        finally
        {
            IsBusy = false;
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
    public async Task SignUp()
    {
        await Shell.Current.GoToAsync($"//{nameof(SignUpPage)}");
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
                Message = string.IsNullOrWhiteSpace(text) ? "Login cavabı oxunmadı." : text
            };
        }
    }

    private void SaveLoggedInUser(AuthUserDto? user, string fallbackLogin)
    {
        var currentUser = _userSession.CurrentUser ?? new UserInfo();

        if (user is not null)
        {
            currentUser.Id = user.Id;
            currentUser.UserName = user.UserName ?? string.Empty;
            currentUser.Name = user.Name ?? string.Empty;
            currentUser.Surname = user.Surname ?? string.Empty;
            currentUser.PhoneNumber = user.PhoneNumber ?? string.Empty;
            currentUser.Email = user.Email ?? string.Empty;
        }
        else
        {
            currentUser.UserName = fallbackLogin;
            currentUser.Email = fallbackLogin;
            currentUser.PhoneNumber = fallbackLogin;
        }

        _userSession.CurrentUser = currentUser;
    }

    private static string FirstNonEmpty(params string?[] values)
    {
        foreach (var value in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return value.Trim();
        }

        return string.Empty;
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
