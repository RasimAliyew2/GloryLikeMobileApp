using MetanetA_MobileApp.ViewModels.ProductsViewModels;

namespace MetanetA_MobileApp.View.Products;

public partial class ProductPage : ContentPage
{
    private readonly ProductViewModel _vm;

    public ProductPage(ProductViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query == null) return;

        if (query.TryGetValue("categoryKey", out var raw) && raw != null)
        {
            var key = raw.ToString();
           // _vm.LoadRootCategory(key);
        }
    }
    protected override bool OnBackButtonPressed()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await _vm.Home();
         //   await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        });

        return true; // default back işləməsin, app çıxmasın
    }
}

