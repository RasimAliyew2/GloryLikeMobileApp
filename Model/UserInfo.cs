using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MetanetA_MobileApp.Model
{
    public partial class UserInfo : ObservableObject
    {
        [ObservableProperty]
        private string name;
        [ObservableProperty]
        private string surname;
        [ObservableProperty]
        private string city;
        [ObservableProperty]
        private string fatherName;
        [ObservableProperty]
        private string job;
        [ObservableProperty]
        private string phoneNumber;
        [ObservableProperty]
        private DateTime birthDate = DateTime.Today;
        [ObservableProperty]
        private string village;
        [ObservableProperty]
        private float bonus;
        [ObservableProperty]
        private string password;
        [ObservableProperty]
        private ProfileBonus bonusOfProfile = new ProfileBonus();
    }
}


