using MetanetA_MobileApp.ViewModels;
using MetanetA_MobileApp.ViewModels.Sign;

namespace MetanetA_MobileApp.View;

public partial class ForgetPasswordPage : ContentPage
{
    public ForgetPasswordPage(ForgetPasswordViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override bool OnBackButtonPressed()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Shell.Current.GoToAsync($"//{nameof(SignInPage)}");
        });

        return true;
    }
}
