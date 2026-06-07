using MetanetA_MobileApp.ViewModels.Sign.SingUp;

namespace MetanetA_MobileApp.View.SignUp;

public partial class VerifySkillsPage : ContentPage
{
	public VerifySkillsPage(VerifySkillsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}