using System.Security.Cryptography;
using MetanetA_MobileApp.View;
using MetanetA_MobileApp.View.SignUp;
using Microsoft.Maui.Controls;

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


        Routing.RegisterRoute(nameof(CareerPreferencesPage), typeof(CareerPreferencesPage));
        Routing.RegisterRoute(nameof(ProfileReviewPage), typeof(ProfileReviewPage));


    }
}
