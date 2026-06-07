using MetanetA_MobileApp.ViewModels;

namespace MetanetA_MobileApp.View;
public partial class QRCodeNotAccepted : ContentPage   // ContentPage
{
    public QRCodeNotAccepted(QRCodeNotAcceptedViewModel notAccepted)
    {
        InitializeComponent();
        BindingContext = notAccepted;
    }
    protected override bool OnBackButtonPressed()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        });

        return true; // default back işləməsin, app çıxmasın
    }
}
