using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Services.UIState;
using MetanetA_MobileApp.View;

namespace MetanetA_MobileApp.ViewModels.QRCode
{
    [QueryProperty(nameof(Bonus), "Bonus")]
    public partial class QRCodeAcceptedViewModel : BaseViewModel
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BonusText))]
        private string bonus;
        public string BonusText => Bonus;

        public QRCodeAcceptedViewModel(BottomMenuState bottomMenuState) : base(bottomMenuState) { }

        [RelayCommand]
        public async Task GoToMainPage()
        {
            Home();
        }

        partial void OnBonusChanged(string value)
        {
            // If you need to trigger logic once the "100" arrives, do it here.
            Debug.WriteLine($"The value received is: {value}");
        }
    }
}
