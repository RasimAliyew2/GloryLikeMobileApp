using MetanetA_MobileApp.View.SignUp;
using MetanetA_MobileApp.ViewModel;

namespace MetanetA_MobileApp.View;

public partial class SignInPage : ContentPage
{
    public SignInPage()
    {
        InitializeComponent();
    }

    public SignInPage(SignInViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private async void ContinueWithEmail_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"//{nameof(SignUpPage)}");
    }

    private async void ContinueWithGoogle_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert("Google", "Google sign-up integration is not connected yet.", "OK");
    }

    private async void ContinueWithApple_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert("Apple", "Apple sign-up integration is not connected yet.", "OK");
    }

    private async void BackToRegistration_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync($"//{nameof(SignUpPage)}");
    }

    private async void Terms_Tapped(object sender, TappedEventArgs e)
    {
        await DisplayAlert("Terms", "Terms and Privacy Policy screen is not connected yet.", "OK");
    }
}
