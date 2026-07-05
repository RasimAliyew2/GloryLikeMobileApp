using System.Net.Http.Json;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.Services;
using MetanetA_MobileApp.Services.GetDataFromServer;
using MetanetA_MobileApp.Services.UIState;
using MetanetA_MobileApp.View;

namespace MetanetA_MobileApp.ViewModels.Sign;

public partial class ForgetPasswordViewModel : BaseViewModel
{
    private readonly IUserSession userSession;
    private readonly AuthApiService authApiService;

    [ObservableProperty]
    private string phoneNumber = string.Empty;

    [ObservableProperty]
    private string emailOrPhone = string.Empty;

    [ObservableProperty]
    private bool phoneNotNumberNotFound;

    [ObservableProperty]
    private bool isResetStepVisible;

    [ObservableProperty]
    private string resetCode = string.Empty;

    [ObservableProperty]
    private string devResetCode = string.Empty;

    [ObservableProperty]
    private string newPassword = string.Empty;

    [ObservableProperty]
    private string confirmPassword = string.Empty;

    [ObservableProperty]
    private bool hasError;

    [ObservableProperty]
    private string message = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    public ForgetPasswordViewModel(
        IUserSession userSession,
        UserInfo userInfo,
        BottomMenuState bottomMenu,
        HttpClient httpClient) : base(bottomMenu)
    {
        this.userSession = userSession;
        this.userSession.CurrentUser ??= userInfo;
        authApiService = new AuthApiService(httpClient);
    }

    [RelayCommand]
    public async Task ApproveTheNumber()
    {
        if (IsBusy)
            return;

        var identifier = GetIdentifier();
        if (string.IsNullOrWhiteSpace(identifier))
        {
            SetError("Email və ya telefon nömrəsi daxil et.");
            return;
        }

        try
        {
            IsBusy = true;
            HasError = false;
            Message = string.Empty;
            PhoneNotNumberNotFound = false;

            var isEmail = identifier.Contains('@');
            var phone = isEmail ? string.Empty : AuthApiService.NormalizePhone(identifier);

            var result = await authApiService.ForgotPasswordAsync(new ForgotPasswordRequest
            {
                EmailOrPhone = identifier,
                Email = isEmail ? identifier : string.Empty,
                PhoneNumber = phone
            });

            if (!result.Success)
            {
                PhoneNotNumberNotFound = true;
                SetError(result.Message);
                return;
            }

            // userSession.PhoneNumber = phone;
            // if (userSession.CurrentUser is not null)
            //     userSession.CurrentUser.PhoneNumber = phone;

            DevResetCode = result.Data?.ResetCode ?? result.Data?.Code ?? string.Empty;
            IsResetStepVisible = true;
            Message = string.IsNullOrWhiteSpace(result.Data?.Message)
                ? "Reset kod göndərildi."
                : result.Data!.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task ResetPassword()
    {
        if (IsBusy)
            return;

        var identifier = GetIdentifier();
        if (string.IsNullOrWhiteSpace(identifier))
        {
            SetError("Email və ya telefon nömrəsi daxil et.");
            return;
        }

        if (string.IsNullOrWhiteSpace(ResetCode))
        {
            SetError("Reset kod daxil et.");
            return;
        }

        if (string.IsNullOrWhiteSpace(NewPassword) || NewPassword != ConfirmPassword)
        {
            SetError("Yeni password boş ola bilməz və təsdiqlə eyni olmalıdır.");
            return;
        }

        try
        {
            IsBusy = true;
            HasError = false;
            Message = string.Empty;

            var isEmail = identifier.Contains('@');
            var phone = isEmail ? string.Empty : AuthApiService.NormalizePhone(identifier);

            var result = await authApiService.ResetPasswordAsync(new ResetPasswordRequest
            {
                EmailOrPhone = identifier,
                Email = isEmail ? identifier : string.Empty,
                PhoneNumber = phone,
                ResetCode = ResetCode.Trim(),
                Code = ResetCode.Trim(),
                NewPassword = NewPassword,
                Password = NewPassword
            });

            if (!result.Success)
            {
                SetError(result.Message);
                return;
            }

            Message = "Password dəyişdirildi. İndi sign in edə bilərsən.";
            await Shell.Current.GoToAsync($"//{nameof(SignInPage)}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private string GetIdentifier()
    {
        if (!string.IsNullOrWhiteSpace(EmailOrPhone))
            return EmailOrPhone.Trim();

        if (!string.IsNullOrWhiteSpace(PhoneNumber))
            return AdjustUserInfo.AdjustPhoneNumber(PhoneNumber).Trim();

        return string.Empty;
    }

    private void SetError(string text)
    {
        HasError = true;
        Message = text;
    }
}
