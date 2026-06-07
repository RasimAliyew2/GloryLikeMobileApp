using MetanetA_MobileApp.ViewModels.QRCode;

namespace MetanetA_MobileApp.View;

public partial class QrCodeAccepted : ContentPage
{
    public QrCodeAccepted(QRCodeAcceptedViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
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