using System;
using System.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.Services.Abstractions;
using MetanetA_MobileApp.Services.UIState;

namespace MetanetA_MobileApp.ViewModels
{
    public partial class ProfileUserViewModel : BaseViewModel
    {
        private readonly IUserSession _session;

        private UserInfo _originalSnapshot;

        [ObservableProperty]
        private UserInfo draft;

        [ObservableProperty]
        private bool hasChanges;

        [ObservableProperty]
        private bool isBusy;

        public ProfileUserViewModel(IUserSession session, BottomMenuState menuState)
            : base(menuState)
        {
            _session = session;

            // CurrentUser yoxdursa boş user ilə aç
            var src = _session.CurrentUser ?? new UserInfo();

            Draft = CloneUser(src);
            _originalSnapshot = CloneUser(Draft);

            Draft.PropertyChanged += Draft_PropertyChanged;

            EvaluateChanges();
        }

        private void Draft_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Hər dəyişiklikdə HasChanges hesabla
            EvaluateChanges();
        }

        private void EvaluateChanges()
        {
            HasChanges = !AreEqual(Draft, _originalSnapshot);
        }

        private static bool AreEqual(UserInfo a, UserInfo b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;

            // DateTime-də sadəcə tarixi müqayisə edək
            var da = a.BirthDate.Date;
            var db = b.BirthDate.Date;

            return string.Equals(a.Name ?? "", b.Name ?? "", StringComparison.Ordinal)
                   && string.Equals(a.Surname ?? "", b.Surname ?? "", StringComparison.Ordinal)
                   && string.Equals(a.City ?? "", b.City ?? "", StringComparison.Ordinal)
                   && string.Equals(a.FatherName ?? "", b.FatherName ?? "", StringComparison.Ordinal)
                   && string.Equals(a.Job ?? "", b.Job ?? "", StringComparison.Ordinal)
                   && string.Equals(a.PhoneNumber ?? "", b.PhoneNumber ?? "", StringComparison.Ordinal)
                   && da == db
                   && string.Equals(a.Village ?? "", b.Village ?? "", StringComparison.Ordinal)
                   && Math.Abs(a.Bonus - b.Bonus) < 0.0001f
                   && string.Equals(a.Password ?? "", b.Password ?? "", StringComparison.Ordinal);
            // BonusOfProfile burada compare edilmir (adətən formda edit olunmur).
        }

        private static UserInfo CloneUser(UserInfo src)
        {
            // UserInfo field-ları: Name, Surname, City, FatherName, Job, PhoneNumber, BirthDate, Village, Bonus, Password, BonusOfProfile :contentReference[oaicite:1]{index=1}
            return new UserInfo
            {
                Name = src.Name,
                Surname = src.Surname,
                City = src.City,
                FatherName = src.FatherName,
                Job = src.Job,
                PhoneNumber = src.PhoneNumber,
                BirthDate = src.BirthDate,
                Village = src.Village,
                Bonus = src.Bonus,
                Password = src.Password,
                BonusOfProfile = src.BonusOfProfile // referens olaraq qalır
            };
        }

        private static void Apply(UserInfo from, UserInfo to)
        {
            to.Name = from.Name;
            to.Surname = from.Surname;
            to.City = from.City;
            to.FatherName = from.FatherName;
            to.Job = from.Job;
            to.PhoneNumber = from.PhoneNumber;
            to.BirthDate = from.BirthDate;
            to.Village = from.Village;
            to.Bonus = from.Bonus;
            to.Password = from.Password;

            // BonusOfProfile edit olunmursa belə, sync saxlayırıq
            to.BonusOfProfile = from.BonusOfProfile;
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                if (_session.CurrentUser == null)
                    _session.CurrentUser = new UserInfo();

                Apply(Draft, _session.CurrentUser);

                // Session-da ayrıca PhoneNumber da varsa, onu da sync et
                _session.PhoneNumber = _session.CurrentUser.PhoneNumber; // IUserSession-də var :contentReference[oaicite:2]{index=2}

                // Snapshot yenilə
                _originalSnapshot = CloneUser(Draft);
                EvaluateChanges();

                await Application.Current.MainPage.DisplayAlert(
                    "Hazır",
                    "Məlumatlar yadda saxlanıldı.",
                    "OK");

                await Shell.Current.GoToAsync("..");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            // Heç nə apply eləmirik -> əvvəlki kimi qalır
            await Shell.Current.GoToAsync("..");
        }
    }
}
