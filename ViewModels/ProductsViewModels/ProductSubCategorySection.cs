using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using MetanetA_MobileApp.Model;

namespace MetanetA_MobileApp.ViewModels.ProductsViewModels;

public partial class ProductSubCategorySection : ObservableObject
{
    public ProductSubCategorySection(string name)
    {
        Name = name;
    }

    public string Name { get; }

    [ObservableProperty]
    private bool isExpanded;

    public ObservableCollection<ProductItem> Products { get; } = new();
}