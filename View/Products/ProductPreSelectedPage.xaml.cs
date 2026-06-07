// View/Products/ProductPreSelectedPage.xaml.cs
using MetanetA_MobileApp.ViewModels.ProductsViewModels;

namespace MetanetA_MobileApp.View.Products;

public partial class ProductPreSelectedPage : ContentPage
{
    public ProductPreSelectedPage(ProductPreSelectedViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
