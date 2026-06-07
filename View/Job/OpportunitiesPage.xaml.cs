using MetanetA_MobileApp.ViewModels.Job;

namespace MetanetA_MobileApp.View.Job;

public partial class OpportunitiesPage : ContentPage
{
	public OpportunitiesPage(OpportunitiesViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}