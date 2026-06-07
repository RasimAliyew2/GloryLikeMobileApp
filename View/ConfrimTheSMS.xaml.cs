using MetanetA_MobileApp.ViewModels;

namespace MetanetA_MobileApp.View;

public partial class ConfrimTheSMS : ContentPage
{
	public ConfrimTheSMS(ConfrimSMSViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
    private void Entry_Completed(object sender, EventArgs e)
    {
        var text = (sender as Entry)?.Text?.Trim() ?? string.Empty;
        // burda yoxlama və ya növbəti addım:
        // ValidateCode(text);
    }
}