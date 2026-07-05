using MetanetA_MobileApp.ViewModel;
using MetanetA_MobileApp.View.SignUp;

namespace MetanetA_MobileApp.View;

public partial class SignInPage : ContentPage
{
    private readonly SignInViewModel _viewModel;

    public SignInPage(SignInViewModel vm)
    {
        InitializeComponent();
        _viewModel = vm;
        BindingContext = vm;
    }

    private async void BackToRegistration_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync($"//{nameof(SignUpPage)}");
    }

    private async void ContinueEmail_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync($"//{nameof(SignUpPage)}");
    }

    private async void ContinueGoogle_Tapped(object sender, EventArgs e)
    {
        await DisplayAlert("Google", "Google sign-in hələ qoşulmayıb.", "OK");
    }

    private async void ContinueApple_Tapped(object sender, EventArgs e)
    {
        await DisplayAlert("Apple", "Apple sign-in hələ qoşulmayıb.", "OK");
    }

    private void ShowSignIn_Tapped(object sender, TappedEventArgs e)
    {
        // Kept for compatibility if an old XAML reference still exists.
    }

    private async void ShowRegistration_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync($"//{nameof(SignUpPage)}");
    }

    private async void ForgotPassword_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync($"//{nameof(ForgetPasswordPage)}");
    }

    private async void SignIn_Tapped(object sender, EventArgs e)
    {
        CopyEntryValuesIntoViewModelIfNeeded();
        await _viewModel.SignIn();
    }

    private void CopyEntryValuesIntoViewModelIfNeeded()
    {
        var login = GetEntryText("LoginEntry")
                    ?? GetEntryText("EmailEntry")
                    ?? GetEntryText("PhoneEntry")
                    ?? GetEntryText("PhoneNumberEntry")
                    ?? GetEntryText("UsernameEntry");

        var password = GetEntryText("PasswordEntry");

        if (!string.IsNullOrWhiteSpace(login))
        {
            _viewModel.Login = login.Trim();
            _viewModel.PhoneNumber = login.Trim();
            _viewModel.Email = login.Trim();
        }

        if (!string.IsNullOrWhiteSpace(password))
            _viewModel.Password = password;
    }

    private string? GetEntryText(string name)
    {
        try
        {
            return this.FindByName<Entry>(name)?.Text;
        }
        catch
        {
            return null;
        }
    }
}
