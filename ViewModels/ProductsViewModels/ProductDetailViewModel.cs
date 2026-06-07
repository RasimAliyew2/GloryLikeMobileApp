using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.Services.UIState;
using MetanetA_MobileApp.View;
using MetanetA_MobileApp.View.Products;
using Microsoft.Maui.Controls;

namespace MetanetA_MobileApp.ViewModels.ProductsViewModels
{
    [QueryProperty(nameof(ProductItem), "ProductItem")]
    public partial class ProductDetailViewModel : BaseViewModel
    {
        [ObservableProperty]
        public ProductItem productItem;

        [ObservableProperty]
        public bool isVisible;

        private readonly UserInfo userInfo;

        public string Name => ProductItem?.Name ?? string.Empty;
        public string ImageUrl => ProductItem?.ImageUrl ?? string.Empty;
        public float Price => ProductItem?.Price ?? 0;

        public HtmlWebViewSource AboutTheProductHtml => new HtmlWebViewSource
        {
            Html = BuildHtml(ProductItem?.AboutTheProduct)
        };

        public ProductDetailViewModel(UserInfo userInfo, BottomMenuState bottomMenu) : base(bottomMenu)
        {
            this.userInfo = userInfo;
        }

        partial void OnProductItemChanged(ProductItem value)
        {
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(ImageUrl));
            OnPropertyChanged(nameof(Price));
            OnPropertyChanged(nameof(AboutTheProductHtml));
        }

        private string BuildHtml(string htmlBody)
        {
            if (string.IsNullOrWhiteSpace(htmlBody))
            {
                htmlBody = "<p>Məhsul haqqında məlumat yoxdur.</p>";
            }

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{
            font-family: Arial, Helvetica, sans-serif;
            font-size: 14px;
            color: #111827;
            line-height: 1.6;
            margin: 0;
            padding: 0;
            background-color: white;
        }}

        h1, h2, h3, h4 {{
            color: #111827;
            margin-top: 0;
        }}

        p {{
            margin: 0 0 10px 0;
        }}

        table {{
            width: 100%;
            border-collapse: collapse;
            margin-top: 12px;
            margin-bottom: 12px;
        }}

        th, td {{
            border: 1px solid #D1D5DB;
            padding: 8px;
            text-align: left;
            font-size: 13px;
        }}

        th {{
            background-color: #F3F4F6;
            font-weight: bold;
        }}

        ul {{
            padding-left: 18px;
        }}
    </style>
</head>
<body>
    {htmlBody}
</body>
</html>";
        }

        [RelayCommand]
        public async Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        public async Task Buy()
        {
            if (userInfo.BonusOfProfile.CurrentBonus > Price)
                await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
            else
                IsVisible = true;
        }

        [RelayCommand]
        public async Task Decline()
        {
            await Shell.Current.GoToAsync($"//{nameof(ProductPage)}");
        }
    }
}