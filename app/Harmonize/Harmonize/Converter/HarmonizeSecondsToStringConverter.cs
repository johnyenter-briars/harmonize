using System.Globalization;

namespace Harmonize.Converter;

public class HarmonizeSecondsToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double seconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);

            return timeSpan.ToString(timeSpan.Hours > 0 ? @"hh\:mm\:ss" : @"mm\:ss");
        }
        return "00:00";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("SecondsToStringConverter is a one-way converter.");
    }
}
