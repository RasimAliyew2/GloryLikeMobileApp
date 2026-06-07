using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MetanetA_MobileApp.ViewModels.ProductsViewModels;

public partial class ProductRootCategorySection : ObservableObject
{
    public ProductRootCategorySection(string key, string name, string imageUrl)
    {
        Key = key;
        Name = name;
        ImageUrl = imageUrl;
    }

    public string Key { get; }
    public string Name { get; }
    public string ImageUrl { get; }

    [ObservableProperty]
    private bool isExpanded;

    public ObservableCollection<ProductSubCategorySection> SubCategories { get; } = new();
}