using System.Collections.Specialized;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;
using MetanetA_MobileApp.Model;
using MetanetA_MobileApp.ViewModels;
using System.Globalization;
using Microsoft.Maui.ApplicationModel;

namespace MetanetA_MobileApp.View.Map;

public partial class LocationMapPage : ContentPage
{
    private readonly LocationMapViewModel _vm;
    private readonly Dictionary<Pin, MapPointItem> _pinLookup = new();

    public LocationMapPage(LocationMapViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;

        _vm.Points.CollectionChanged += OnPointsCollectionChanged;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RenderMap();
    }

    private void OnPointsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(RenderMap);
    }

    private void RenderMap()
    {
        MainMap.Pins.Clear();
        _pinLookup.Clear();

        foreach (var point in _vm.Points)
        {
            var pin = new Pin
            {
                Label = point.Title,
                Address = point.ShortInfo,
                Type = PinType.Place,
                Location = new Location(point.Latitude, point.Longitude)
            };

          //  pin.InfoWindowClicked += OnPinInfoWindowClicked;
            pin.MarkerClicked += OnPinMarkerClicked;

            MainMap.Pins.Add(pin);
            _pinLookup[pin] = point;
        }

        var center = new Location(_vm.CenterLatitude, _vm.CenterLongitude);
        var span = MapSpan.FromCenterAndRadius(center, Distance.FromKilometers(_vm.StartRadiusKm));
        MainMap.MoveToRegion(span);
    }
    private async void OnPinMarkerClicked(object? sender, PinClickedEventArgs e)
    {
        if (sender is not Pin pin)
            return;

        if (!_pinLookup.TryGetValue(pin, out var point))
            return;

        // İstəsən info pəncərəsi açılmasın
        e.HideInfoWindow = true;

        string lat = point.Latitude.ToString(CultureInfo.InvariantCulture);
        string lng = point.Longitude.ToString(CultureInfo.InvariantCulture);

        string wazeAppUrl = $"waze://?ll={lat},{lng}&navigate=yes";
        string wazeWebUrl = $"https://waze.com/ul?ll={lat},{lng}&navigate=yes";

        try
        {
            if (await Launcher.Default.CanOpenAsync(wazeAppUrl))
                await Launcher.Default.OpenAsync(wazeAppUrl);
            else
                await Launcher.Default.OpenAsync(wazeWebUrl);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Xəta", $"Waze açıla bilmədi: {ex.Message}", "OK");
        }
    }
    private async void OnPinInfoWindowClicked(object? sender, PinClickedEventArgs e)
    {
        if (sender is not Pin pin)
            return;

        if (!_pinLookup.TryGetValue(pin, out var point))
            return;

        await DisplayAlert(point.Title, point.Description, "Bağla");
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _vm.Points.CollectionChanged -= OnPointsCollectionChanged;

        foreach (var pin in MainMap.Pins)
            // pin.InfoWindowClicked -= OnPinInfoWindowClicked;
            pin.MarkerClicked -= OnPinMarkerClicked;
    }
}