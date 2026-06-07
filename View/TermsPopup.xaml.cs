using System;
using CommunityToolkit.Maui.Views;

namespace MetanetA_MobileApp.View
{
    public partial class TermsPopup : Popup
    {
        public TermsPopup()
        {
            InitializeComponent();
        }

        private void Close_Clicked(object sender, EventArgs e)
        {
            Close();
        }
    }
}
