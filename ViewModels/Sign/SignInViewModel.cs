using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.Services;
using MetanetA_MobileApp.Services.Abstractions;
using MetanetA_MobileApp.Services.GetDataFromServer;
using MetanetA_MobileApp.View;
using MetanetA_MobileApp.View.SignUp;

namespace MetanetA_MobileApp.ViewModel
{
    public partial class SignInViewModel : ObservableObject
    {

        public Bonus UserBonus;

        [ObservableProperty]
        string phoneNumber;


        [ObservableProperty]
        string password;

        [ObservableProperty]
        bool fillTheArea = false;

        [ObservableProperty]
        bool invalidCredentials = false;

        // Eye toggle state-lər
        [ObservableProperty]
        private bool isPasswordHidden = true;

        [ObservableProperty]
        private bool isConfirmPasswordHidden = true;
        IUserSession userSession;

        public SignInViewModel(IUserSession userSession)
        {
            this.userSession = userSession;
            // username = "test";
        }

        [RelayCommand] 
        public async Task SignIn()
        {
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
            if (string.IsNullOrEmpty(PhoneNumber) || string.IsNullOrEmpty(Password))
                return;
            
            
            PhoneNumber = AdjustUserInfo.AdjustPhoneNumber(PhoneNumber);
            string text = "";
           // string text = await GetAndPostAllDataForUser.GetAsyncUserInfo(new UserInfo() { Password = password, PhoneNumber = phoneNumber });
            if (string.IsNullOrEmpty(text))
            {
                FillTheArea = true;
                InvalidCredentials = false;
            }
            else if (text == "Invalid credentials")
            {
                InvalidCredentials = true;
                FillTheArea = false;
            }
            else
            {
                userSession.CurrentUser = JsonSerializer.Deserialize<UserInfo[]>(text)[0];
              await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
            }

        }

        [RelayCommand]
        private void TogglePasswordVisibility()
        {
            IsPasswordHidden = !IsPasswordHidden;
        }
        [RelayCommand]
        public async Task SignUp()
        {
            //await Shell.Current.GoToAsync(nameof(SignUpPage));

            await Shell.Current.GoToAsync($"//{nameof(VerifyIdentityPage)}");
        }
    }
}
