using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using MetanetA_MobileApp.Model;

namespace MetanetA_MobileApp.ViewModels.Sign.SingUp
{
    public partial class SkillSelectionItem : ObservableObject
    {
        public SkillSelectionItem(Skill skill)
        {
            Skill = skill;
        }

        public Skill Skill { get; }

        [ObservableProperty]
        private bool isSelected;
    }
}
