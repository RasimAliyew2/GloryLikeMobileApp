using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Services.UIState;
using MetanetA_MobileApp.View;
using MetanetA_MobileApp.View.Products;

namespace MetanetA_MobileApp.ViewModels
{
    public partial class OthersPageViewModel : BaseViewModel
    {
        public OthersPageViewModel(BottomMenuState menuState) : base(menuState)
        {
        }
    }
}
