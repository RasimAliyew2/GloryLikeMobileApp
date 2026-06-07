using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MetanetA_MobileApp.Model
{
    public partial class SubCategoryItem : ObservableObject
    {
        public string Name { get; set; }
        public string CategoryName { get; set; }

        [ObservableProperty]
        private bool isSelected;

        public SubCategoryItem() { }

        public SubCategoryItem(string name, string categoryName)
        {
            Name = name;
            CategoryName = categoryName;
        }
    }
}
