using MetanetA_MobileApp.ViewModels;

namespace MetanetA_MobileApp.View;

public partial class FaqPage : ContentPage
{
    public FaqPage(FaqPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
    protected override bool OnBackButtonPressed()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Shell.Current.GoToAsync($"//{nameof(OthersPage)}");
        });

        return true; // default back işləməsin, app çıxmasın
    }
}