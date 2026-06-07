using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MetanetA_MobileApp.Model
{
    public partial class ProductItem : ObservableObject
    {
        [ObservableProperty]
        public string name;
        [ObservableProperty]
        public string description;
        [ObservableProperty]
        public string title;
        [ObservableProperty]
        public string imageUrl;
        [ObservableProperty]
        public float price;
        [ObservableProperty]
        public string aboutTheProduct;
        // Filter üçün:
        public string Category { get; set; }
        public string SubCategory { get; set; }
    }
}
