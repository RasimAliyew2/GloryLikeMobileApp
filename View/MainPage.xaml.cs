using MetanetA_MobileApp.ViewModels;
using Microsoft.VisualBasic;

namespace MetanetA_MobileApp.View
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }

}