using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MetanetA_MobileApp.Model
{
    public class ApplicationModel
    {
        public string LogoLetter { get; set; }
        public string Company {  get; set; }
        public string Title { get; set; }
        public string AppliedText { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public bool NeedsAttention { get; set; }
        public string CardStrokeColor { get; set; } = "#E2E6EF";
        public string StatusBackgroundColor { get; set; } = "#EEF2FA";
        public string StatusTextColor { get; set; } = "#475166";

        public bool HasNote { get; set; }

    }
}
