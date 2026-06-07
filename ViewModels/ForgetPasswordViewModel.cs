using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.Services;
using MetanetA_MobileApp.Services.Abstractions;
using MetanetA_MobileApp.Services.GetDataFromServer;
using MetanetA_MobileApp.Services.UIState;
using MetanetA_MobileApp.View;

namespace MetanetA_MobileApp.ViewModels;

public partial class ForgetPasswordViewModel : BaseViewModel
{
    [ObservableProperty]
    string phoneNumber;

    [ObservableProperty]
    bool phoneNotNumberNotFound = false;
    IUserSession userSession;
    public ForgetPasswordViewModel(IUserSession userSession,UserInfo userInfo, BottomMenuState bottomMenu) : base(bottomMenu)
    {
        this.userSession = userSession;
        this.userSession.CurrentUser = userInfo;
    }
    [RelayCommand]
    public async Task ApproveTheNumber()
    {
        PhoneNumber = AdjustUserInfo.AdjustPhoneNumber(PhoneNumber);
       // var response = await GetAndPostAllDataForUser.PostAsyncUserInfoUnique(new Model.UserInfo() { PhoneNumber = phoneNumber }, "CanChangePassword");

        //if(response != "Success")
        //{
        //    PhoneNotNumberNotFound = true;
        //    return;
        //}
        //userSession.CurrentUser.PhoneNumber = PhoneNumber;
        //userSession.OtpCode = await SendEmail.SendSmsAsync(PhoneNumber);
        //await Shell.Current.GoToAsync($"//{nameof(ConfrimTheSMS)}", new Dictionary<string, object>
        //{
        //    ["OperationType"] = OperationType.ChangePassword
        //});

    }
}