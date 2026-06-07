using MetanetA_MobileApp.ViewModels.Sign.SingUp;

namespace MetanetA_MobileApp.View.SignUp;

public partial class VerifyIdentityPage : ContentPage
{
	public VerifyIdentityPage(VerifyIdentityViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}