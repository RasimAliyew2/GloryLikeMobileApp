using MetanetA_MobileApp.ViewModels.Job;

namespace MetanetA_MobileApp.View.Job;

public partial class ApplicationsPage : ContentPage
{
	public ApplicationsPage(ApplicationsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
		
	}
}