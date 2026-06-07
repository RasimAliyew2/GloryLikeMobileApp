using MetanetA_MobileApp.ViewModels;

namespace MetanetA_MobileApp.View;

public partial class RequestAcceptedPage : ContentPage
{
	public RequestAcceptedPage(RequestAcceptedViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}