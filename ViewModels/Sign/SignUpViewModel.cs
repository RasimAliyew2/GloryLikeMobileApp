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

namespace MetanetA_MobileApp.ViewModels
{
    public partial class SignUpViewModel : ObservableObject
    {
        public DateTime MinBirthDate => DateTime.Today.AddYears(-120);
        public DateTime MaxBirthDate => DateTime.Today;

        private readonly IUserSession userSession;

        [ObservableProperty] private string lineNumber;
        [ObservableProperty] private UserInfo userInfo;

        // Required sahələr üçün valid flag-lar
        [ObservableProperty] private bool isNameValid = true;
        [ObservableProperty] private bool isSurnameValid = true;
        [ObservableProperty] private bool isSFathernameValid = true;
        [ObservableProperty] private bool isPhoneValid = true;
        [ObservableProperty] private bool isCityValid = true;
        [ObservableProperty] private bool isJobValid = true;
        [ObservableProperty] private bool isTermsValid = true;

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

        public ObservableCollection<string> Cities { get; } = new();
        public ObservableCollection<string> Prefixes { get; } = new();
        public ObservableCollection<string> Jobs { get; } = new();


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
            IsOtherJobSelected = value == "Digər";

            if (!IsOtherJobSelected)
            {
                OtherJobText = null;
                UserInfo.Job = value; // normal seçim
            }
            else
            {
                // Digər seçiləndə UserInfo.Job hələlik boş qala bilər
                UserInfo.Job = OtherJobText?.Trim();
            }
        }

        partial void OnOtherJobTextChanged(string value)
        {
            if (IsOtherJobSelected)
                UserInfo.Job = value?.Trim();
        }


        partial void OnSelectedPrefixChanged(string value) => OnPropertyChanged(nameof(FullPhoneNumber));
        partial void OnLineNumberChanged(string value) => OnPropertyChanged(nameof(FullPhoneNumber));
        partial void OnIsTermsAcceptedChanged(bool value)
        {
            if (HasSubmitted)
            {
                IsTermsValid = value;
                UpdateValidationPanel();
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

        private bool ValidateForm()
        {
            HasSubmitted = true;

            // trim
            UserInfo.Name = UserInfo.Name?.Trim();
            UserInfo.Surname = UserInfo.Surname?.Trim();

            IsNameValid = !string.IsNullOrWhiteSpace(UserInfo.Name);
            IsSurnameValid = !string.IsNullOrWhiteSpace(UserInfo.Surname);
            IsSFathernameValid = !string.IsNullOrWhiteSpace(UserInfo.FatherName);
            // Telefon: prefix seçilib + lineNumber 7 rəqəm
            var phone = BuildPhoneNumber();
            var lineDigits = new string((LineNumber ?? "").Where(char.IsDigit).ToArray());
            IsPhoneValid = !string.IsNullOrWhiteSpace(phone) && phone.StartsWith("994") && lineDigits.Length == 7;

            // Peşə
            if (UserInfo.Job == "Digər")
            {
                IsOtherJobSelected = true;
                IsJobValid = !string.IsNullOrWhiteSpace(OtherJobText);
            }
            else
            {
                IsOtherJobSelected = false;
                IsJobValid = !string.IsNullOrWhiteSpace(UserInfo.Job);
            }


            // Rayon və Peşə
            IsCityValid = !string.IsNullOrWhiteSpace(UserInfo.City);
            IsJobValid = !string.IsNullOrWhiteSpace(UserInfo.Job);

            // Checkbox
            IsTermsValid = IsTermsAccepted;



            UpdateValidationPanel();

            return IsNameValid && IsSurnameValid && IsPhoneValid && IsCityValid && IsJobValid && IsTermsValid;
        }

        private void UpdateValidationPanel()
        {
            if (!HasSubmitted)
            {
                IsValidationVisible = false;
                ValidationMessage = string.Empty;
                return;
            }

            // Prioritetli mesajlar
            if (!IsNameValid || !IsSurnameValid)
            {
                IsValidationVisible = true;
                ValidationMessage = "Zəhmət olmasa Ad və Soyad xanalarını doldurun.";
                return;
            }
            if (!isSFathernameValid)
            {
                IsValidationVisible = true;
                ValidationMessage = "Zəhmət olmasa ata adı xanasını doldurun.";
                return;
            }

            if (!IsPhoneValid)
            {
                IsValidationVisible = true;
                ValidationMessage = "Telefon nömrəsini düzgün daxil edin (Prefix + 7 rəqəm).";
                return;
            }

            if (!IsCityValid)
            {
                IsValidationVisible = true;
                ValidationMessage = "Zəhmət olmasa Rayon seçin.";
                return;
            }

            if (!IsJobValid)
            {
                IsValidationVisible = true;
                ValidationMessage = "Zəhmət olmasa Peşə seçin.";
                return;
            }

            if (!IsTermsValid)
            {
                IsValidationVisible = true;
                ValidationMessage = "Davam etmək üçün qaydaları qəbul etməlisiniz.";
                return;
            }

            IsValidationVisible = false;
            ValidationMessage = string.Empty;
        }

        public void SetCities()
        {
            Cities.Add("Abşeron");
            Cities.Add("Ağcabədi");
            Cities.Add("Ağdam");
            Cities.Add("Ağdaş");
            Cities.Add("Ağdərə");
            Cities.Add("Ağstafa");
            Cities.Add("Ağsu");
            Cities.Add("Astara");
            Cities.Add("Babək");
            Cities.Add("Balakən");
            Cities.Add("Beyləqan");
            Cities.Add("Bərdə");
            Cities.Add("Biləsuvar");
            Cities.Add("Cəbrayıl");
            Cities.Add("Cəlilabad");
            Cities.Add("Culfa");
            Cities.Add("Daşkəsən");
            Cities.Add("Füzuli");
            Cities.Add("Gədəbəy");
            Cities.Add("Goranboy");
            Cities.Add("Göyçay");
            Cities.Add("Göygöl");
            Cities.Add("Hacıqabul");
            Cities.Add("Xaçmaz");
            Cities.Add("Xızı");
            Cities.Add("Xocalı");
            Cities.Add("Xocavənd");
            Cities.Add("İmişli");
            Cities.Add("İsmayıllı");
            Cities.Add("Kəlbəcər");
            Cities.Add("Kəngərli");
            Cities.Add("Kürdəmir");
            Cities.Add("Qax");
            Cities.Add("Qazax");
            Cities.Add("Qəbələ");
            Cities.Add("Qobustan");
            Cities.Add("Quba");
            Cities.Add("Qubadlı");
            Cities.Add("Qusar");
            Cities.Add("Laçın");
            Cities.Add("Lerik");
            Cities.Add("Lənkəran");
            Cities.Add("Masallı");
            Cities.Add("Neftçala");
            Cities.Add("Oğuz");
            Cities.Add("Ordubad");
            Cities.Add("Saatlı");
            Cities.Add("Sabirabad");
            Cities.Add("Salyan");
            Cities.Add("Samux");
            Cities.Add("Sədərək");
            Cities.Add("Siyəzən");
            Cities.Add("Şabran");
            Cities.Add("Şahbuz");
            Cities.Add("Şamaxı");
            Cities.Add("Şəki");
            Cities.Add("Şəmkir");
            Cities.Add("Şərur");
            Cities.Add("Şuşa");
            Cities.Add("Tərtər");
            Cities.Add("Tovuz");
            Cities.Add("Ucar");
            Cities.Add("Yardımlı");
            Cities.Add("Yevlax");
            Cities.Add("Zaqatala");
            Cities.Add("Zəngilan");
            Cities.Add("Zərdab");
        }

        public void SetJobs()
        {
            Jobs.Add("Malyar");
            Jobs.Add("Pol - pataloq");
            Jobs.Add("Universal");
            Jobs.Add("Digər");
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
        public async Task SignUp()
        {
            return;
            // Job "Digər" seçilibsə, Entry-də yazılanı əsas job kimi qəbul et
            if (UserInfo.Job == "Digər")
            {
                IsOtherJobSelected = true;
                UserInfo.Job = OtherJobText?.Trim();
            }
            else
            {
                IsOtherJobSelected = false;
                OtherJobText = null;
            }

            // 1) required validation
            if (!ValidateForm())
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
                IsPhoneValid = false; // qırmızı göstərsin
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
