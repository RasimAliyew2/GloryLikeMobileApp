using MetanetA_MobileApp.ViewModels.Sign;

namespace MetanetA_MobileApp.View.Sign;

public partial class SetPasswordPage : ContentPage
{
	public SetPasswordPage(SetPasswordViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}