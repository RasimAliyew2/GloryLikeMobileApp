using System.Globalization;
using MetanetA_MobileApp.ViewModels.ProductsViewModels;
using Microsoft.Maui.Devices;

namespace MetanetA_MobileApp.View.Products;

public partial class ProductDetailPage : ContentPage
{
    public ProductDetailPage(ProductDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;

    }

    protected override bool OnBackButtonPressed()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Shell.Current.GoToAsync($"//{nameof(ProductPage)}");
        });

        return true; // default back işləməsin, app çıxmasın
    }

    //protected override void OnAppearing()
    //{
    //    base.OnAppearing();

    //    if (BindingContext is ProductDetailViewModel vm)
    //        AboutWebView.Source = vm.AboutTheProductHtml;
            
    //}
 
    
}