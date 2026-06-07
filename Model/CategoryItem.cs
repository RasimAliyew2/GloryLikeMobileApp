using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetanetA_MobileApp.Model
{
    public class CategoryItem
    {
        public string Name { get; set; }
        public ObservableCollection<SubCategoryItem> SubCategories { get; set; } = new();

        public CategoryItem() { }

        public CategoryItem(string name, IEnumerable<SubCategoryItem> subs)
        {
            Name = name;
            SubCategories = new ObservableCollection<SubCategoryItem>(subs);
        }
    }
}
