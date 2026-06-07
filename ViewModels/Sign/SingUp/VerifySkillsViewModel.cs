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
    public partial class VerifySkillsViewModel : ObservableObject
    {
        public ObservableCollection<IdentityStepModel> Steps { get; } = new();

        public ObservableCollection<SkillVerifyItemModel> Skills { get; } = new();

        public VerifySkillsViewModel()
        {
            Steps.Add(new IdentityStepModel { Title = "Identity", IsCompleted = true });
            Steps.Add(new IdentityStepModel { Title = "Career", IsCompleted = true });
            Steps.Add(new IdentityStepModel { Title = "Skills", IsCompleted = true });
            Steps.Add(new IdentityStepModel { Title = "Verify", SubTitle = "4 min", IsActive = true });
            Steps.Add(new IdentityStepModel { Title = "Prefs" });
            Steps.Add(new IdentityStepModel { Title = "Ready" });

            AddNewSkill("Roadmap Planning");
        }

        public void AddNewSkill(string skillName)
        {
            Skills.Add(new SkillVerifyItemModel
            {
                SkillName = skillName,
                CredibilityText = "Credibility 0/100"
            });
        }

        [RelayCommand]
        private void AddNewSkill()
        {
            AddNewSkill("New Skill");
        }

        [RelayCommand]
        private async Task AttachCertificateFile(SkillVerifyItemModel item)
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select certificate file",
                FileTypes = FilePickerFileType.Pdf
            });

            if (result == null)
                return;

            item.CertificateFilePath = result.FullPath;
            item.CertificateFileName = result.FileName;
        }

        [RelayCommand]
        private async Task SubmitCertificate(SkillVerifyItemModel item)
        {
            if (item == null)
                return;

            // Burada API-ya göndərəcəksən.
            await Application.Current.MainPage.DisplayAlert(
                "Submitted",
                $"{item.SkillName} certificate submitted",
                "OK");
        }

        [RelayCommand]
        private async Task Back()
        {
            await Shell.Current.GoToAsync($"//{nameof(SkillsSelectionPage)}");
        }

        [RelayCommand]
        private async Task SkipForNow()
        {
            // Növbəti səhifəyə keçid
        }
    }
}
