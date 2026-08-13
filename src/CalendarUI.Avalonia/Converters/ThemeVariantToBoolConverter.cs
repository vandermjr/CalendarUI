using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Styling;

namespace CalendarUI.Avalonia.Converters;

public class ThemeVariantToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is ThemeVariant themeVariant && themeVariant == ThemeVariant.Dark;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            true => ThemeVariant.Dark,
            false => ThemeVariant.Light,
            _ => ThemeVariant.Default
        };
    }
}