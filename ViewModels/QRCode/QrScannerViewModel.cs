using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.Services.Abstractions;
using MetanetA_MobileApp.Services.UIState;
using MetanetA_MobileApp.View;


namespace MetanetA_MobileApp.ViewModels;

public partial class QrScannerViewModel : BaseViewModel
{
    bool _handling;
    public UserInfo _userInfo;
    private IQRBonusNotifier QRBonusNotifier;
    private float bonus;
 
    public QrScannerViewModel(IUserSession userSession, 
        BottomMenuState bottomMenu,
        IQRBonusNotifier bonusNotifier) : base(bottomMenu)
    {
        QRBonusNotifier = bonusNotifier;
        _userInfo = userSession.CurrentUser;

       // QrDetected("1234-12345");
    }
    [RelayCommand]
    private void QrDetected(string? value)
    {
        if (_handling || string.IsNullOrWhiteSpace(value))
            return;

        _handling = true; // eyni QR-i bir dəfə emal et

        // Scanner event-i hansı thread-dən gəlsə də,
        // bütün UI işini main thread-ə daşıyırıq
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                bool isValid = IsValidFormat(value);
                // burda istəsən sync, istəsən async yoxlama et
                bool used = IsThisBonusUsed(value); // və ya: await IsThisBonusUsedAsync(value);
                
                if (used || !isValid)
                {
                    await Shell.Current.GoToAsync($"//{nameof(QRCodeNotAccepted)}");
                }
                else if(isValid && !used)
                {
                     bonus = GetbonusBasedOnProductType(value);
                    _userInfo.BonusOfProfile.CurrentBonus += bonus;
                  
                    _userInfo.BonusOfProfile.CollectedBonus += bonus;

                    QRBonusNotifier.PublishNewQRBonus(new BonusTransaction
                    {
                        Date = System.DateTime.Today,
                        Type = BonusTransactionType.Earned,
                        Amount = bonus,
                        Description = "QrCode bonus"
                    });


           

                    //await Shell.Current.GoToAsync($"//{nameof(QrCodeAccepted)}");
                    await Shell.Current.GoToAsync($"//{nameof(QrCodeAccepted)}", new Dictionary<string, object>
                    {
                        ["Bonus"] = "100"
                    });
                }
            }
            catch (Exception ex)
            {
                // nəsə səhv getsə görmək üçün
                await Application.Current.MainPage.DisplayAlert("Xəta", ex.Message, "OK");
            }
            finally
            {
                _handling = false;
            }
        });
    }
    [RelayCommand]
    public async Task GoToMainCommand()
    {
        await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
    }
    private bool CheckTheQRCode(string QrCode)
    {
        string[] QrParts = QrCode.Split("-");

        return true;


    }

    static bool IsValidFormat(string value)
    {
        if (value == null) return false;

        value = value.Replace("Code:", "");
        // 4 rakam, tire, 5 rakam
        return Regex.IsMatch(value, @"^\d{4}-\d{5}$");
    }
    private float GetbonusBasedOnProductType(string productTypeCode)
    {
        return 100f;
    }
    private bool IsThisBonusUsed(string QrCode)
    {
        return false;
    }

}

