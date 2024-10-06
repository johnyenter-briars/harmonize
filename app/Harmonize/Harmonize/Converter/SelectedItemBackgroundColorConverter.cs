using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harmonize.Converter;

public class SelectedItemBackgroundColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool isSelected = (bool)value;
        return isSelected ? Color.FromArgb("A3CCC0") : Color.FromArgb("4A355A");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
