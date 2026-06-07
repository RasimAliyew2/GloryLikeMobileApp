using MetanetA_MobileApp.View;

namespace MetanetA_MobileApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(QrScannerPage), typeof(QrScannerPage));

        }
    }
}
