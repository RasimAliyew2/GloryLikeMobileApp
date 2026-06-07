using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Services.UIState;

namespace MetanetA_MobileApp.ViewModels.Profile
{
    
    public partial class SkillsViewModel : BaseViewModel
    {

        public SkillsViewModel(BottomMenuState menuState) : base(menuState)
        {
            
        }
        [RelayCommand]
        private async Task EditProfile()
        {
            await Shell.Current.DisplayAlert("Edit profile", "Edit profile clicked", "OK");
        }

        [RelayCommand]
        private async Task AddSkill()
        {

            await Shell.Current.DisplayAlert("Skills", "Add skill clicked", "OK");
        }

        [RelayCommand]
        private async Task AddExperience()
        {
            await Shell.Current.DisplayAlert("Experience", "Add experience clicked", "OK");
        }
    }
}
