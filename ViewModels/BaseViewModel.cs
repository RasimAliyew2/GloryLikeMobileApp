using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Services.UIState;
using MetanetA_MobileApp.View;
using MetanetA_MobileApp.View.Job;
using MetanetA_MobileApp.View.Products;
using MetanetA_MobileApp.View.Profile;
using MetanetA_MobileApp.ViewModels.Job;

namespace MetanetA_MobileApp.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty] private BottomMenuState menuState;

        public BaseViewModel(BottomMenuState menuState)
        {
            this.menuState = menuState;
        }
  
        [RelayCommand]
        public async Task Qr()
        {
            MenuState.Select(BottomTab.Qr);
            await Shell.Current.GoToAsync(nameof(QrScannerPage));

        }
        [RelayCommand]
        public async Task Applications()
        {
            MenuState.Select(BottomTab.Qr);
            await Shell.Current.GoToAsync($"//{nameof(ApplicationsPage)}");

        }
       

        [RelayCommand]
        public async Task Home()
        {
            MenuState.Select(BottomTab.Home);
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }
        [RelayCommand]
        public async Task Profile()
        {
            MenuState.Select(BottomTab.Profile);
            await Shell.Current.GoToAsync($"//{nameof(SkillsPage)}");
        }
        [RelayCommand]
        public async Task Skills()
        {
            MenuState.Select(BottomTab.Profile);
            await Shell.Current.GoToAsync($"//{nameof(SkillsPage)}");
        }

        [RelayCommand]
        public async Task Products()
        {
            MenuState.Select(BottomTab.Products);
            await Shell.Current.GoToAsync($"//{nameof(OpportunitiesPage)}");
        }
        [RelayCommand]
        public async Task Other()
        {
            MenuState.Select(BottomTab.Other);
            await Shell.Current.GoToAsync($"//{nameof(OthersPage)}");
        }
    
        [RelayCommand]
        public async Task Campaigns()
        {
            // await Shell.Current.GoToAsync($"//{nameof(QrScannerPage)}");
        }

    }
}
