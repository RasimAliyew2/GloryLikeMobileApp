using MetanetA_MobileApp.ViewModels;

namespace MetanetA_MobileApp.View;

public partial class ProfileUserPage : ContentPage
{
    public ProfileUserPage(ProfileUserViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}