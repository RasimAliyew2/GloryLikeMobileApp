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
    private async void ForgotPassword_Tapped(object sender, TappedEventArgs e)
    {
        // Sənin unutdum səhifənin adı nədirsə onu yaz
        await Shell.Current.GoToAsync($"//{nameof(ForgetPasswordPage)}");
    }

}