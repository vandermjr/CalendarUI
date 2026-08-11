using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Styling;

namespace CalendarUI.Demo.ViewModels;

public class ThemeVariantToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ThemeVariant variant)
        {
            return variant == ThemeVariant.Dark;
        }
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isDark)
        {
            return isDark ? ThemeVariant.Dark : ThemeVariant.Light;
        }
        return ThemeVariant.Light;
    }
}
