using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.Services.Abstractions;

namespace MetanetA_MobileApp.Services.Cart
{
    public partial class CartService : ObservableObject
    {
        public ObservableCollection<CartLineItem> Items { get; } = new();
        public UserInfo User { get; set; }
        public int TotalCount => Items.Sum(x => x.Quantity);
        public decimal TotalPrice => Items.Sum(x => x.LineTotal);

        public CartService(IUserSession userSession)
        {
            Items.CollectionChanged += Items_CollectionChanged;
            User = userSession.CurrentUser;
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (CartLineItem item in e.NewItems)
                    item.PropertyChanged += Item_PropertyChanged;

            if (e.OldItems != null)
                foreach (CartLineItem item in e.OldItems)
                    item.PropertyChanged -= Item_PropertyChanged;

            NotifyTotals();
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CartLineItem.Quantity))
                NotifyTotals();
        }

        private void NotifyTotals()
        {
            OnPropertyChanged(nameof(TotalCount));
            OnPropertyChanged(nameof(TotalPrice));
        }

        public void Add(SalesItem item)
        {
            if (item == null) return;

            var existing = Items.FirstOrDefault(x => x.Item.Id == item.Id);
            if (existing != null)
            {
                existing.Quantity += 1;
            }
            else
            {
                Items.Add(new CartLineItem(item, 1));
            }

            NotifyTotals();
        }

        public void Increase(CartLineItem item)
        {
            if (item == null) return;
            item.Quantity += 1;
            NotifyTotals();
        }

        public void Decrease(CartLineItem item)
        {
            if (item == null) return;

            item.Quantity -= 1;
            if (item.Quantity <= 0)
                Items.Remove(item);

            NotifyTotals();
        }

        public void Clear()
        {
            Items.Clear();
            NotifyTotals();
        }
    }
}
