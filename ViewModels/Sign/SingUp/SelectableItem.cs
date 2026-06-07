using CommunityToolkit.Mvvm.ComponentModel;

namespace MetanetA_MobileApp.ViewModels
{
    public partial class SelectableItem<T> : ObservableObject
    {
        public SelectableItem(T value, string title)
        {
            Value = value;
            Title = title;
        }

        public T Value { get; }

        [ObservableProperty]
        private string title = string.Empty;

        [ObservableProperty]
        private bool isSelected;
    }
}