using MetanetA_MobileApp.ViewModels;

namespace MetanetA_MobileApp.View;

public partial class ProfilePage : ContentPage
{
	public ProfilePage(ProfileViewModel vm)
	{
		InitializeComponent();
	    BindingContext = vm;
	}
}