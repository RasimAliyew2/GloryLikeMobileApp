using MetanetA_MobileApp.View;
using MetanetA_MobileApp.View.SignUp;

namespace MetanetA_MobileApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(QrScannerPage), typeof(QrScannerPage));
        Routing.RegisterRoute(nameof(SkillsSelectionPage), typeof(SkillsSelectionPage));
        Routing.RegisterRoute(nameof(VerifySkillsPage), typeof(VerifySkillsPage));
        Routing.RegisterRoute(nameof(SkillEvidencePage), typeof(SkillEvidencePage));
    }
}
