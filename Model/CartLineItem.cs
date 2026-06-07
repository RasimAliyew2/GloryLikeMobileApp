using CommunityToolkit.Mvvm.ComponentModel;

namespace MetanetA_MobileApp.Model
{
    public partial class CartLineItem : ObservableObject
    {
        public CartLineItem(SalesItem item, int quantity = 1)
        {
            Item = item;
            Quantity = quantity;
        }

        public SalesItem Item { get; }

        [ObservableProperty]
        private int quantity = 1;

        public decimal LineTotal => Item.Price * Quantity;

        partial void OnQuantityChanged(int value)
        {
            OnPropertyChanged(nameof(LineTotal));
        }
    }
}
