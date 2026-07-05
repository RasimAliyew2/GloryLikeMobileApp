using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.Model.Auth;
using MetanetA_MobileApp.Services;
using MetanetA_MobileApp.Services.Abstractions;
using MetanetA_MobileApp.Services.GetDataFromServer;
using MetanetA_MobileApp.View;
using MetanetA_MobileApp.View.SignUp;

namespace MetanetA_MobileApp.ViewModels;


    public partial class SignUpViewModel : ObservableObject
    {
        private readonly IAuthApiService _authApiService;
        private readonly IUserSession _userSession;

        public SignUpViewModel(IAuthApiService authApiService, IUserSession userSession)
        {
            _authApiService = authApiService;
            _userSession = userSession;
        }

        [ObservableProperty]
        private string userName = string.Empty;

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private string surname = string.Empty;

        [ObservableProperty]
        private string phoneNumber = string.Empty;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string confirmPassword = string.Empty;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string? errorMessage;

        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

        [RelayCommand]
        private async Task RegisterAsync()
        {
            if (IsBusy)
                return;

            var validation = Validate();
            if (!string.IsNullOrWhiteSpace(validation))
            {
                ErrorMessage = validation;
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = null;

                var response = await _authApiService.RegisterAsync(new RegisterRequest
                {
                    UserName = UserName.Trim(),
                    Name = Name.Trim(),
                    Surname = Surname.Trim(),
                    PhoneNumber = PhoneNumber.Trim(),
                    Email = Email.Trim(),
                    Password = Password
                });

                if (!response.Success || response.User is null)
                {
                    ErrorMessage = response.Message;
                    return;
                }

                _userSession.CurrentUser = new UserInfo
                {
                    Id = response.User.Id,
                    UserName = response.User.UserName,
                    Name = response.User.Name,
                    Surname = response.User.Surname,
                    PhoneNumber = response.User.PhoneNumber,
                    Email = response.User.Email
                };

                await Shell.Current.GoToAsync(nameof(VerifyIdentityPage));
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoToSignInAsync()
        {
            await Shell.Current.GoToAsync($"//{nameof(SignInPage)}");
        }

    [RelayCommand]
    private async Task GoToSignUp()
    {
        await Shell.Current.GoToAsync($"//{nameof(SignUpPage)}");
    }


    private string? Validate()
        {
            if (string.IsNullOrWhiteSpace(UserName))
                return "Username daxil et.";

            if (string.IsNullOrWhiteSpace(Name))
                return "Ad daxil et.";

            if (string.IsNullOrWhiteSpace(Surname))
                return "Soyad daxil et.";

            if (string.IsNullOrWhiteSpace(PhoneNumber))
                return "Telefon nömrəsi daxil et.";

            if (string.IsNullOrWhiteSpace(Email))
                return "Email daxil et.";

            if (string.IsNullOrWhiteSpace(Password) || Password.Length < 8)
                return "Password ən azı 8 simvol olmalıdır.";

            if (Password != ConfirmPassword)
                return "Password-lər eyni deyil.";

            return null;
        }

        partial void OnErrorMessageChanged(string? value)
        {
            OnPropertyChanged(nameof(HasError));
        }
    }

