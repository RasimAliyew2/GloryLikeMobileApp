// ViewModels/ProductsViewModels/ProductPreSelectedViewModel.cs  (REPLACE)
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Services.UIState;
using MetanetA_MobileApp.View.Products;

namespace MetanetA_MobileApp.ViewModels.ProductsViewModels;

public partial class ProductPreSelectedViewModel : BaseViewModel
{
    public ProductPreSelectedViewModel(BottomMenuState menuState) : base(menuState) { }

    [RelayCommand]
    private async Task SelectRootCategoryAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return;

        // ✅ ProductPage səndə bottom-tab (Shell element) kimi açılırsa:
        await Shell.Current.GoToAsync($"//{nameof(ProductPage)}",
            new Dictionary<string, object>
            {
                ["CategoryKey"] = key
            });


        // Əgər səndə ProductPage Shell-də tab DEYİL, sadəcə Routing.RegisterRoute ilə açılırsa,
        // yuxarı sətri kommentə al, bunu aç:
        // await Shell.Current.GoToAsync(nameof(ProductPage), new Dictionary<string, object> { ["categoryKey"] = key });
    }
}
