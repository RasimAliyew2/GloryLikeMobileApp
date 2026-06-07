using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MetanetA_MobileApp.Model
{
    public partial class ProfileBonus : ObservableObject
    {
        [ObservableProperty]
        public float usedBonus;
        [ObservableProperty]
        public float currentBonus;
        [ObservableProperty]
        public float collectedBonus;
    }
}
