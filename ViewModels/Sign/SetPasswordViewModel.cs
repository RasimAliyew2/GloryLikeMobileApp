using System;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.Services;
using MetanetA_MobileApp.Services.Abstractions;
using MetanetA_MobileApp.Services.GetDataFromServer;
using MetanetA_MobileApp.View;

namespace MetanetA_MobileApp.ViewModels.Sign
{
    [QueryProperty(nameof(OperationType), "OperationType")]
    public partial class SetPasswordViewModel : ObservableObject
    {
        [ObservableProperty] private string password;
        [ObservableProperty] private string confirmPassword;

        [ObservableProperty] private bool isMismatch;
        [ObservableProperty] private string mismatchText = "uyğun gəlmir";

        [ObservableProperty] private OperationType operationType;

        // Eye toggle state-lər
        [ObservableProperty] private bool isPasswordHidden = true;
        [ObservableProperty] private bool isConfirmPasswordHidden = true;

        // NEW: password rule flags
        [ObservableProperty] private bool isPasswordRuleInvalid;
        [ObservableProperty] private string passwordRuleText;

        private readonly IUserSession userSession;

        partial void OnPasswordChanged(string value)
        {
            ValidatePasswordRules();
            UpdateMismatch();
        }

        partial void OnConfirmPasswordChanged(string value)
        {
            ValidatePasswordRules();
            UpdateMismatch();
        }

        private void UpdateMismatch()
        {
            IsMismatch = !string.IsNullOrWhiteSpace(ConfirmPassword)
                         && !string.Equals(Password, ConfirmPassword, StringComparison.Ordinal);
        }

        private void ValidatePasswordRules()
        {
            var pwd = (Password ?? string.Empty).Trim();

            // 8 simvol
            bool minLen = pwd.Length >= 8;

            // 1 böyük hərf
            bool hasUpper = pwd.Any(char.IsUpper);

            // boş olma vəziyyətində qayda mesajını çox aqressiv göstərməyək
            if (string.IsNullOrWhiteSpace(pwd))
            {
                IsPasswordRuleInvalid = false;
                PasswordRuleText = string.Empty;
                return;
            }

            if (!minLen || !hasUpper)
            {
                IsPasswordRuleInvalid = true;

                if (!minLen && !hasUpper)
                    PasswordRuleText = "Parol minimum 8 simvol olmalı və ən azı 1 böyük hərf (A-Z) içərməlidir.";
                else if (!minLen)
                    PasswordRuleText = "Parol minimum 8 simvol olmalıdır.";
                else
                    PasswordRuleText = "Parolda ən azı 1 böyük hərf (A-Z) olmalıdır.";
            }
            else
            {
                IsPasswordRuleInvalid = false;
                PasswordRuleText = string.Empty;
            }
        }

        public SetPasswordViewModel(IUserSession userSession)
        {
            this.userSession = userSession;
        }

        [RelayCommand]
        private void TogglePasswordVisibility()
        {
            IsPasswordHidden = !IsPasswordHidden;
        }

        [RelayCommand]
        private void ToggleConfirmPasswordVisibility()
        {
            IsConfirmPasswordHidden = !IsConfirmPasswordHidden;
        }

        [RelayCommand]
        private async Task ConfirmAsync()
        {
            ValidatePasswordRules();
            UpdateMismatch();

            // boş ola bilməz
            if (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                IsMismatch = true;
                MismatchText = "parol boş ola bilməz";
                return;
            }

            // qayda ödənmirsə
            if (IsPasswordRuleInvalid)
            {
                // rule text onsuz da görünür
                return;
            }

            // mismatch varsa
            if (IsMismatch)
                return;

            userSession.CurrentUser.Password = Password;

            if (OperationType == OperationType.SetPassword)
                await SetPassword();
            else if (OperationType == OperationType.ChangePassword)
            {
                userSession.CurrentUser.PhoneNumber = AdjustUserInfo.AdjustPhoneNumber(userSession.CurrentUser.PhoneNumber);
               // await GetAndPostAllDataForUser.PostAsyncUserInfoUnique(userSession.CurrentUser, "UpdateUserInfo");
                await Shell.Current.GoToAsync($"//{nameof(SignInPage)}");
            }
        }

        private async Task SetPassword()
        {
           //userSession.CurrentUser.PhoneNumber = AdjustUserInfo.AdjustPhoneNumber(userSession.CurrentUser.PhoneNumber);
           //await GetAndPostAllDataForUser.PostAsyncUserInfo(userSession.CurrentUser);
           //await Shell.Current.GoToAsync($"//{nameof(RequestAcceptedPage)}");
        }
    }
}
