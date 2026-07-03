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
        private JobFamily job;
        [ObservableProperty]
        private string phoneNumber;
        [ObservableProperty]
        private string password;
    }
}


