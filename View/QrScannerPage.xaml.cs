using MetanetA_MobileApp.ViewModels;

namespace MetanetA_MobileApp.View;

public partial class QrScannerPage : ContentPage
{
    public QrScannerPage(QrScannerViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        //Resources.Add("FirstBarcodeValueConverter", new Converters.FirstBarcodeValueConverter());
    }
    protected override bool OnBackButtonPressed()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await (BindingContext as QrScannerViewModel).Home();
         //   await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        });

        return true; // default back işləməsin, app çıxmasın
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Kamera icazəsi (bəzən resume zamanı yenidən tələb olunur kimi davranır)
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
            status = await Permissions.RequestAsync<Permissions.Camera>();

        if (status != PermissionStatus.Granted)
            return;

        // Preview-ni yenidən start et
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Scanner.IsEnabled = true;
            Scanner.IsDetecting = true;
        });
    }

    protected override void OnDisappearing()
    {
        // Preview-ni stop et ki, kamera resursu buraxılsın
        Scanner.IsDetecting = false;
        Scanner.IsEnabled = false;

        base.OnDisappearing();
    }

}
