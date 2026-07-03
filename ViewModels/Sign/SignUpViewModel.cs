using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.Services;
using MetanetA_MobileApp.Services.Abstractions;
using MetanetA_MobileApp.Services.GetDataFromServer;
using MetanetA_MobileApp.View;
using MetanetA_MobileApp.View.SignUp;

namespace MetanetA_MobileApp.ViewModels
{
    public partial class SignUpViewModel : ObservableObject
    {
        public DateTime MinBirthDate => DateTime.Today.AddYears(-120);
        public DateTime MaxBirthDate => DateTime.Today;

        private readonly IUserSession userSession;
        [ObservableProperty] private UserInfo userInfo;

        [ObservableProperty] private string lineNumber;

 

        // UI: yalnız "submit" cəhdindən sonra qırmızı göstərmək üçün
        [ObservableProperty] private bool hasSubmitted;

        // Warning panel
        [ObservableProperty] private bool isValidationVisible;
        [ObservableProperty] private string validationMessage;

        [ObservableProperty] private string selectedPrefix = "+994 50";
        [ObservableProperty] private bool isTermsAccepted;

        // Eye toggle state-lər
        [ObservableProperty]
        private bool isPasswordHidden = true;

        [ObservableProperty]
        private bool isConfirmPasswordHidden = true;


        [ObservableProperty]
        private bool isOtherJobSelected;

        [ObservableProperty]
        private string otherJobText;

        public ObservableCollection<string> Prefixes { get; } = new();



        [ObservableProperty] private Color nameBorderColor = Colors.LightGray;
        [ObservableProperty] private Color surnameBorderColor = Colors.LightGray;
        [ObservableProperty] private Color fatherBorderColor = Colors.LightGray;
        [ObservableProperty] private Color phoneBorderColor = Colors.LightGray;
        [ObservableProperty] private Color cityBorderColor = Colors.LightGray;
        [ObservableProperty] private Color jobBorderColor = Colors.LightGray;


        public string FullPhoneNumber => BuildPhoneNumber();

        public SignUpViewModel(IUserSession userSession, UserInfo userInfo)
        {
            this.userSession = userSession;
            UserInfo = userInfo;


            // qeydiyyat zamanı da session-da current user eyni reference olsun
            this.userSession.CurrentUser = UserInfo;

            SetCities();
            SetJobs();
            SetPrefixes();
        }

        [ObservableProperty]
        private string selectedJob;


        partial void OnSelectedJobChanged(string value)
        {
            // Picker dəyişəndə bura düşəcək
          //  IsOtherJobSelected = value == "Digər";
          //
          //  if (!IsOtherJobSelected)
          //  {
          //      OtherJobText = null;
          //      UserInfo.Job = value; // normal seçim
          //  }
          //  else
          //  {
          //      // Digər seçiləndə UserInfo.Job hələlik boş qala bilər
          //      UserInfo.Job = OtherJobText?.Trim();
          //  }
        }

        partial void OnOtherJobTextChanged(string value)
        {
           // if (IsOtherJobSelected)
           //     UserInfo.Job = value?.Trim();
        }


        partial void OnSelectedPrefixChanged(string value) => OnPropertyChanged(nameof(FullPhoneNumber));
        partial void OnLineNumberChanged(string value) => OnPropertyChanged(nameof(FullPhoneNumber));
        partial void OnIsTermsAcceptedChanged(bool value)
        {
            if (HasSubmitted)
            {
               // IsTermsValid = value;
               // UpdateValidationPanel();
            }
        }

        private string BuildPhoneNumber()
        {
            var prefixDigits = (SelectedPrefix ?? "")
                .Replace("+", "")
                .Replace(" ", "")
                .Trim();

            var lineDigits = new string((LineNumber ?? "").Where(char.IsDigit).ToArray());
            if (string.IsNullOrWhiteSpace(prefixDigits) && string.IsNullOrWhiteSpace(lineDigits))
                return string.Empty;

            return prefixDigits + lineDigits;
        }



   

        public void SetCities()
        {
         
        }

        public void SetJobs()
        {

        }

        public void SetPrefixes()
        {
            Prefixes.Add("+994 50");
            Prefixes.Add("+994 51");
            Prefixes.Add("+994 55");
            Prefixes.Add("+994 10");
            Prefixes.Add("+994 60");
            Prefixes.Add("+994 70");
            Prefixes.Add("+994 77");
            Prefixes.Add("+994 99");
        }
        [RelayCommand]
        public async Task OpenUrl()
        {

        }

        [RelayCommand]
        public async Task Continue()
        {
            await Shell.Current.GoToAsync($"//{nameof(VerifyIdentityPage)}");
        }


            
        [RelayCommand]
        public async Task SignUp()
        {
          
            return;
          
            // 2) phone build
            UserInfo.PhoneNumber = BuildPhoneNumber();
            string response = null;
            // 3) server check
           // var response = await GetAndPostAllDataForUser.PostAsyncUserInfoUnique(UserInfo, "CheckIfUserExists");

            if (response == "user_already_exists")
            {
                IsValidationVisible = true;
                ValidationMessage = "Bu nömrə ilə daha öncə qeydiyyatdan keçilib!";
               // IsPhoneValid = false; // qırmızı göstərsin
                return;
            }

            // 4) send otp
            userSession.OtpCode = await SendEmail.SendSmsAsync(UserInfo.PhoneNumber);

            // 5) go next
            await Shell.Current.GoToAsync($"//{nameof(ConfrimTheSMS)}", new Dictionary<string, object>
            {
                ["OperationType"] = OperationType.SetPassword
            });
        }
    }
}
