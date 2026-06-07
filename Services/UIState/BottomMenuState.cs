using CommunityToolkit.Mvvm.ComponentModel;
using MetanetA_MobileApp.Services.Cart;

namespace MetanetA_MobileApp.Services.UIState
{
    public enum BottomTab
    {
        Home, Products, Qr, Profile, Other
    }

    public partial class BottomMenuState : ObservableObject
    {
        [ObservableProperty] private BottomTab selectedTab;
        [ObservableProperty] private int cartCount;

        public bool IsHomeSelected => SelectedTab == BottomTab.Home;
        public bool IsProductsSelected => SelectedTab == BottomTab.Products;
        public bool IsQrSelected => SelectedTab == BottomTab.Qr;
        public bool IsProfileSelected => SelectedTab == BottomTab.Profile;
        public bool IsOtherSelected => SelectedTab == BottomTab.Other;

        public BottomMenuState(CartService cart)
        {
            CartCount = cart.TotalCount;

            cart.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(cart.TotalCount))
                    CartCount = cart.TotalCount;
            };
        }

        partial void OnSelectedTabChanged(BottomTab value)
        {
            OnPropertyChanged(nameof(IsHomeSelected));
            OnPropertyChanged(nameof(IsProductsSelected));
            OnPropertyChanged(nameof(IsQrSelected));
            OnPropertyChanged(nameof(IsProfileSelected));
            OnPropertyChanged(nameof(IsOtherSelected));
        }

        public void Select(BottomTab tab)
        {
            SelectedTab = tab;
        }
    }
}
