using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.View;

namespace MetanetA_MobileApp.ViewModels
{
    public partial class QRCodeNotAcceptedViewModel : ObservableObject
    {
        [RelayCommand]
        public async Task GoToMain()
        {
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }

    }
}
