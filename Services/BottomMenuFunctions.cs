using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.View;

namespace MetanetA_MobileApp.Services
{
    public static partial class BottomMenuFunctions
    {
        public static async Task Home()
        {
             await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }
        public static async Task Profile()
        {
            await Shell.Current.GoToAsync($"//{nameof(ProfilePage)}");
        }

        public static async Task Products()
        {
            // await Shell.Current.GoToAsync($"//{nameof(QrScannerPage)}");
        }
        public static async Task Qr()
        {
            await Shell.Current.GoToAsync($"//{nameof(QrScannerPage)}");
        }
    }
}
