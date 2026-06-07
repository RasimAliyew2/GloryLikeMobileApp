using System.Globalization;
using ZXing.Net.Maui;

namespace MetanetA_MobileApp.Converters;

public class FirstBarcodeValueConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => (value is BarcodeDetectionEventArgs a) ? a.Results?.FirstOrDefault()?.Value : null;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
