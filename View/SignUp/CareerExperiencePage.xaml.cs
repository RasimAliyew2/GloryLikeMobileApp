using MetanetA_MobileApp.ViewModels.Sign.SingUp;

namespace MetanetA_MobileApp.View.SignUp;

public partial class CareerExperiencePage : ContentPage
{
	public CareerExperiencePage(CareerExperienceViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}