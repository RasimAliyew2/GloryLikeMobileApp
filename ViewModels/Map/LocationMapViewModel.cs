using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.Services.UIState;
using MetanetA_MobileApp.View;

namespace MetanetA_MobileApp.ViewModels;

public partial class LocationMapViewModel : BaseViewModel
{
    public ObservableCollection<MapPointItem> Points { get; } = new();

    [ObservableProperty]
    private double centerLatitude = 40.4093;   // Bakı üçün nümunə

    [ObservableProperty]
    private double centerLongitude = 49.8671;

    [ObservableProperty]
    private double startRadiusKm = 8;

    private BottomMenuState bottomMenu;
    public LocationMapViewModel(BottomMenuState bottomMenu) : base(bottomMenu)
    {
        this.bottomMenu =  bottomMenu;
        // Nümunə nöqtələr
        AddPoint(
            latitude: 40.449083,
            longitude: 49.8920,
            title: "İlham Qala",
            shortInfo: "Metro və ətraf zona",
            description: "(Rüstəmov İlham Əlməmməd oğlu)(MİQ)");

        AddPoint(
            latitude: 40.427565,
            longitude: 49.889082,
            title: "Fuad Qafarlı A.",
            shortInfo: "Mərkəzi hissə",
            description: "Fuad Qafarlı A.(Zabrat)(MİQ)");

        AddPoint(
            latitude: 40.4400086763817,
            longitude: 49.74052193871677,
            title: "Əzizov Sovqat",
            shortInfo: "",
            description: "Hökuməli (MİQ)");
    }

    public void AddPoint(
        double latitude,
        double longitude,
        string title,
        string shortInfo,
        string description)
    {
        Points.Add(new MapPointItem
        {
            Latitude = latitude,
            Longitude = longitude,
            Title = title,
            ShortInfo = shortInfo,
            Description = description
        });
    }
    [RelayCommand]
    public async void GoBack()
    {
        await Shell.Current.GoToAsync($"//{nameof(OthersPage)}");
    }
}