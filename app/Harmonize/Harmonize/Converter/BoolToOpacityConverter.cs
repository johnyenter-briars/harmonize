using System.Globalization;

namespace Harmonize.Converter;

public class BoolToOpacityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (bool)value ? 1.0 : 0.3; // 30% opacity when disabled
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (double)value == 1.0;
    }
}
