using MetanetA_MobileApp.Services;
using MetanetA_MobileApp.Services.Abstractions;
using MetanetA_MobileApp.View;
using MetanetA_MobileApp.ViewModels;
using Microsoft.Extensions.Logging;
using ZXing.Net.Maui.Controls;
using CommunityToolkit.Maui;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.ViewModels.QRCode;
using MetanetA_MobileApp.ViewModels.Profile;
using MetanetA_MobileApp.View.Profile;
using Microsoft.Maui.Handlers;
using MetanetA_MobileApp.View.Products;
using MetanetA_MobileApp.ViewModels.ProductsViewModels;
using MetanetA_MobileApp.ViewModels.Sign;
using MetanetA_MobileApp.View.Sign;
using MetanetA_MobileApp.ViewModel;
using MetanetA_MobileApp.Services.UIState;
using MetanetA_MobileApp.Services.Sales;
using MetanetA_MobileApp.Services.Cart;
using MetanetA_MobileApp.View.Map;
using MetanetA_MobileApp.View.Job;
using MetanetA_MobileApp.ViewModels.Job;
using MetanetA_MobileApp.View.SignUp;
using MetanetA_MobileApp.ViewModels.Sign.SingUp;
using MetanetA_MobileApp.Services.GetDataFromServer;


#if ANDROID
using Android.Webkit;
using Android.OS;




#endif

#if IOS
using WebKit;
using Microsoft.Maui.Platform;
using MetanetA_MobileApp.ViewModels.Job;
using MetanetA_MobileApp.View.Job;
#endif




#if ANDROID
using AndroidX.AppCompat.Widget;
#endif

namespace MetanetA_MobileApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
             .UseMauiApp<App>()
             .UseMauiMaps()
             .UseBarcodeReader()          // ZXing 0.4.0
             .UseMauiCommunityToolkit();

#if ANDROID
        EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
        {
            if (handler.PlatformView is AndroidX.AppCompat.Widget.AppCompatEditText editText)
            {
                // alt xetti verən background-u silirik
                editText.Background = null;
            }
        });
#endif

#if DEBUG
            builder.Logging.AddDebug();
#endif




            builder.ConfigureMauiHandlers(handlers =>
            {
                WebViewHandler.Mapper.AppendToMapping("YouTubeFix", (handler, view) =>
                {
#if ANDROID
                    var wv = handler.PlatformView;

                    // JS + storage
                    wv.Settings.JavaScriptEnabled = true;
                    wv.Settings.DomStorageEnabled = true;
                    wv.Settings.JavaScriptCanOpenWindowsAutomatically = true;

                    // Video autoplay/inline
                    wv.Settings.MediaPlaybackRequiresUserGesture = false;

                    // Mixed content bəzən lazımdır
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                        wv.Settings.MixedContentMode = MixedContentHandling.AlwaysAllow;

                    // YouTube iframe üçün WebChromeClient şərtdir
                    wv.SetWebChromeClient(new WebChromeClient());
                    wv.SetWebViewClient(new WebViewClient());

                    // 3rd party cookies (ən çox 153-ü bu həll edir)
                    var cookieMgr = CookieManager.Instance;
                    cookieMgr.SetAcceptCookie(true);
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                        cookieMgr.SetAcceptThirdPartyCookies(wv, true);

                    // User-Agent: YouTube bəzən default webview UA-da problem çıxarır
                    wv.Settings.UserAgentString =
                        "Mozilla/5.0 (Linux; Android 14; Mobile) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Mobile Safari/537.36";

                    // Cache təmiz (test üçün)
                    wv.ClearCache(true);
#endif
                });
            });

            //Pages
            builder.Services.AddTransient<FaqPage>();
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddTransient<SkillsPage>();
            builder.Services.AddSingleton<SignInPage>(); 
            builder.Services.AddSingleton<SignUpPage>();
            builder.Services.AddSingleton<VideosPage>(); 
            builder.Services.AddSingleton<OthersPage>();
            builder.Services.AddSingleton<ProductPage>();
            builder.Services.AddSingleton<ProfilePage>();
            builder.Services.AddSingleton<QrScannerPage>();
            builder.Services.AddSingleton<ConfrimTheSMS>(); 
            builder.Services.AddSingleton<QrCodeAccepted>();
            builder.Services.AddTransient<LocationMapPage>();
            builder.Services.AddSingleton<SetPasswordPage>();
            builder.Services.AddSingleton<ProfileUserPage>();
            builder.Services.AddSingleton<ApplicationsPage>();
            builder.Services.AddSingleton<ProductDetailPage>();
            builder.Services.AddTransient<OpportunitiesPage>();
            builder.Services.AddSingleton<QRCodeNotAccepted>();
            builder.Services.AddSingleton<VerifyIdentityPage>();
            builder.Services.AddSingleton<ForgetPasswordPage>(); 
            builder.Services.AddSingleton<RequestAcceptedPage>();
            builder.Services.AddSingleton<CareerExperiencePage>();


            //View Models

            builder.Services.AddTransient<BaseViewModel>(); 
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<VideosViewModel>();
            builder.Services.AddTransient<LocationMapViewModel>();
            builder.Services.AddTransient<ProfileUserViewModel>();
            builder.Services.AddTransient<VerifySkillsViewModel>();
            builder.Services.AddTransient<ApplicationsViewModel>();
            builder.Services.AddTransient<CareerExperienceViewModel>();
            builder.Services.AddTransient<SkillsSelectionViewModel>();
            





            builder.Services.AddSingleton<CartState>();
            builder.Services.AddTransient<SkillsViewModel>();
            builder.Services.AddTransient<SignInViewModel>();
            builder.Services.AddTransient<SignUpViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<ProductViewModel>();
            builder.Services.AddTransient<FaqPageViewModel>();
            builder.Services.AddSingleton<VerifySkillsPage>();
            builder.Services.AddTransient<QrScannerViewModel>(); 
            builder.Services.AddSingleton<SkillsSelectionPage>();
            builder.Services.AddTransient<OthersPageViewModel>();
            builder.Services.AddTransient<ConfrimSMSViewModel>();
            builder.Services.AddTransient<SetPasswordViewModel>();
            builder.Services.AddTransient<ProductDetailViewModel>();
            builder.Services.AddTransient<OpportunitiesViewModel>();
            builder.Services.AddTransient<ForgetPasswordViewModel>();
            builder.Services.AddTransient<QRCodeAcceptedViewModel>();
            builder.Services.AddSingleton<VerifyIdentityViewModel>();
            builder.Services.AddTransient<RequestAcceptedViewModel>();
            builder.Services.AddTransient<QRCodeNotAcceptedViewModel>();
            //Services
            builder.Services.AddSingleton<IQRBonusNotifier, QRBonusNotifier>(); 
            builder.Services.AddSingleton<IUserSession, UserSession>();  
            builder.Services.AddSingleton<IQrCode, QrCode>();
            builder.Services.AddSingleton<BottomMenuState>(); 
            builder.Services.AddSingleton<SalesCatalogService>();
            builder.Services.AddSingleton<CartService>();
            builder.Services.AddTransient<ISkillAndJobApiService, SkillAndJobApiService>();
            builder.Services.AddSingleton<HttpClient>(sp =>
            {
#if DEBUG
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };

                return new HttpClient(handler)
                {
                    BaseAddress = new Uri(GetApiBaseUrl())
                };
#else
    return new HttpClient
    {
        BaseAddress = new Uri(GetApiBaseUrl())
    };
#endif
            });


            //Models
            builder.Services.AddSingleton<IBonus, Bonus>();
            builder.Services.AddSingleton<UserInfo>();
            builder.Services.AddSingleton<ProfileBonus>();


            static string GetApiBaseUrl()
            {
#if ANDROID
    return "https://10.0.2.2:7016/";
#else
                return "https://localhost:7016/";
#endif
            }

            return builder.Build();
        }
    }
}
