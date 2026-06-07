using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MetanetA_MobileApp.Model
{
    public partial class FaqItem : ObservableObject
    {
        [ObservableProperty]
        private string question = string.Empty;

        [ObservableProperty]
        private string answer = string.Empty;

        [ObservableProperty]
        private bool isExpanded;

        public string ToggleText => IsExpanded ? "-" : "+";

        partial void OnIsExpandedChanged(bool value)
        {
            OnPropertyChanged(nameof(ToggleText));
        }
    }
}
