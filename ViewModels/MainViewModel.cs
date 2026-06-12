using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.Services.Cart;
using MetanetA_MobileApp.Services.Sales;
using MetanetA_MobileApp.Services.UIState;
using MetanetA_MobileApp.View;
using MetanetA_MobileApp.View.Profile;

namespace MetanetA_MobileApp.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly SalesCatalogService _catalog;

        [ObservableProperty] private CartState cart;
        [ObservableProperty] private CartService cartService;

        // MainPage search
        [ObservableProperty] private string searchText;
        [ObservableProperty] private bool isSearchResultsVisible;

        public ObservableCollection<SalesItem> SearchResults { get; } = new();

        public MainViewModel(
            BottomMenuState menuState,
            CartState cart,
            CartService cartService,
            SalesCatalogService catalog) : base(menuState)
        {
            Cart = cart;
            CartService = cartService;
            _catalog = catalog;

            // məhsul siyahısı yenilənərsə search nəticələri də yenilənsin
            _catalog.Products.CollectionChanged += Products_CollectionChanged;
        }

        private void Products_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ApplyMainSearch();
        }

        partial void OnSearchTextChanged(string value)
        {
            ApplyMainSearch();
        }

        private void ApplyMainSearch()
        {
            SearchResults.Clear();

            var key = (SearchText ?? "").Trim();
            if (string.IsNullOrWhiteSpace(key))
            {
                IsSearchResultsVisible = false;
                return;
            }

            var k = Normalize(key);

            // “oxşar” = startswith daha üstün, contains daha sonra
            var results = _catalog.Products
                .Where(p => p != null && !string.IsNullOrWhiteSpace(p.Name))
                .Select(p =>
                {
                    var name = Normalize(p.Name);
                    var score = Score(name, k);
                    return new { p, score };
                })
                .Where(x => x.score < 1000)
                .OrderBy(x => x.score)
                .ThenBy(x => x.p.Name)
                .Take(8)
                .Select(x => x.p)
                .ToList();

            foreach (var item in results)
                SearchResults.Add(item);

            IsSearchResultsVisible = SearchResults.Count > 0;
        }

        private static string Normalize(string s)
        {
            s = s.Trim().ToLowerInvariant();

            // sadə normalizasiya (boşluqları azalt)
            while (s.Contains("  "))
                s = s.Replace("  ", " ");

            return s;
        }

        private static int Score(string name, string key)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(key))
                return 1000;

            if (name.StartsWith(key, StringComparison.Ordinal))
                return 0;

            var idx = name.IndexOf(key, StringComparison.Ordinal);
            if (idx >= 0)
                return 10 + idx;

            // əlavə: söz-söz uyğunluq (məs: "ro kol" -> "rokol boya")
            var parts = key.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 1 && parts.All(p => name.Contains(p, StringComparison.Ordinal)))
                return 80;

            return 1000;
        }

        [RelayCommand]
        private void ClearMainSearch()
        {
            SearchText = string.Empty;
            SearchResults.Clear();
            IsSearchResultsVisible = false;
        }

        [RelayCommand]
        private async Task OpenSuggestedProductAsync(SalesItem item)
        {
            if (item == null) return;

            ClearMainSearch();

            //await Shell.Current.GoToAsync($"//{nameof(SalesDetailPage)}", new Dictionary<string, object>
            //{
            //    ["Item"] = item
            //});
        }

 
    }
}
