using MetanetA_MobileApp.ViewModel;
using MetanetA_MobileApp.ViewModels;

namespace MetanetA_MobileApp.View;

public partial class SignInPage : ContentPage
{
    public SignInPage(SignInViewModel vm)
    {
        InitializeComponent();
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

    private async void ContinueGoogle_Tapped(object sender, TappedEventArgs e)
    {
        await DisplayAlert("Google", "Google sign-in hələ qoşulmayıb.", "OK");
    }

    private async void ContinueApple_Tapped(object sender, TappedEventArgs e)
    {
        await DisplayAlert("Apple", "Apple sign-in hələ qoşulmayıb.", "OK");
    }

    private void ShowSignIn_Tapped(object sender, TappedEventArgs e)
    {
        RegistrationPanel.IsVisible = false;
        SignInPanel.IsVisible = true;
    }

    private void ShowRegistration_Tapped(object sender, TappedEventArgs e)
    {
        SignInPanel.IsVisible = false;
        RegistrationPanel.IsVisible = true;
    }

    private async void ForgotPassword_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync($"//{nameof(ForgetPasswordPage)}");
    }

    private async void SignIn_Tapped(object sender, TappedEventArgs e)
    {
        var email = EmailEntry.Text?.Trim();
        var password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Sign in", "Email və password daxil et.", "OK");
            return;
        }

        await DisplayAlert("Sign in", "Sign-in API hələ qoşulmayıb.", "OK");
    }
}
