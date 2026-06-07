using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.View.SignUp;

namespace MetanetA_MobileApp.ViewModels.Sign.SingUp
{
   public partial class CareerExperienceViewModel : ObservableObject
    {
        public ObservableCollection<IdentityStepModel> Steps { get; } = new();

        public CareerExperienceViewModel()
        {
            Steps.Add(new IdentityStepModel
            {
                Title = "Identity",
                SubTitle = "",
                IsActive = false,
                IsCompleted = true
            });

            Steps.Add(new IdentityStepModel
            {
                Title = "Career",
                SubTitle = "3 min",
                IsActive = true,
                IsCompleted = false
            });

            Steps.Add(new IdentityStepModel { Title = "Skills", SubTitle = "", IsActive = false });
            Steps.Add(new IdentityStepModel { Title = "Verify", SubTitle = "", IsActive = false });
            Steps.Add(new IdentityStepModel { Title = "Prefs", SubTitle = "", IsActive = false });
            Steps.Add(new IdentityStepModel { Title = "Ready", SubTitle = "", IsActive = false });
        }

        [RelayCommand]
        private async Task Back()
        {
            await Shell.Current.GoToAsync($"//{nameof(VerifyIdentityPage)}");
        }

        [RelayCommand]
        private async Task SkipForNow()
        {
            await Shell.Current.GoToAsync($"//{nameof(SkillsSelectionPage)}");
        }

        [RelayCommand]
        private void ImportFromEmas()
        {
            // Özün dolduracaqsan.
        }

        [RelayCommand]
        private void UploadFile()
        {
            // Özün dolduracaqsan.
        }
    }
}

