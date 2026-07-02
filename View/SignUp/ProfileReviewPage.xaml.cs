using MetanetA_MobileApp.ViewModels.Sign.SingUp;

namespace MetanetA_MobileApp.View.SignUp;

public partial class ProfileReviewPage : ContentPage
{
    public ProfileReviewPage(ProfileReviewViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
