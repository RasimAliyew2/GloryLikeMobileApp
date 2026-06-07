using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Services.UIState;
using MetanetA_MobileApp.View;

namespace MetanetA_MobileApp.ViewModels
{
    public partial class RequestAcceptedViewModel : BaseViewModel
    {
        public RequestAcceptedViewModel(BottomMenuState menuState) : base(menuState)
        {
        }

        [RelayCommand]
        public async Task GoToSignInPage()
        {
            await Shell.Current.GoToAsync($"//{nameof(SignInPage)}");
        }
    }
}
