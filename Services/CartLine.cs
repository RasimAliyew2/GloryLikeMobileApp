using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using MetanetA_MobileApp.Model;

namespace MetanetA_MobileApp.Services.UIState
{
    public partial class CartLine : ObservableObject
    {
        public SalesItem Item { get; }

        [ObservableProperty] private int quantity;

        public decimal LineTotal => Item.Price * Quantity;

        public CartLine(SalesItem item, int quantity)
        {
            Item = item;
            Quantity = quantity;
        }
    }

    public partial class CartState : ObservableObject
    {
        public ObservableCollection<CartLine> Lines { get; } = new();

        [ObservableProperty] private int itemCount;
        [ObservableProperty] private decimal totalPrice;

        public bool HasItems => ItemCount > 0;

        public void Add(SalesItem item, int qty = 1)
        {
            if (item == null) return;

            var line = Lines.FirstOrDefault(x => x.Item.Id == item.Id);

            if (line == null)
                Lines.Add(new CartLine(item, qty));
            else
                line.Quantity += qty;

            Recalc();
        }

        public void Remove(SalesItem item)
        {
            if (item == null) return;

            var line = Lines.FirstOrDefault(x => x.Item.Id == item.Id);
            if (line != null)
                Lines.Remove(line);

            Recalc();
        }

        public void Clear()
        {
            Lines.Clear();
            Recalc();
        }

        public void Recalc()
        {
            ItemCount = Lines.Sum(x => x.Quantity);
            TotalPrice = Lines.Sum(x => x.LineTotal);

            OnPropertyChanged(nameof(HasItems));
        }
    }
}
