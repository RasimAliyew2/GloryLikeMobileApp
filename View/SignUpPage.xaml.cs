using MetanetA_MobileApp.ViewModels;

namespace MetanetA_MobileApp.View;

public partial class SignUpPage : ContentPage
{
    public SignUpPage(SignUpViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
