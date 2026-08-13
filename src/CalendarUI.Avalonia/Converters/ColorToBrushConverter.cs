using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace CalendarUI.Avalonia.Converters;

public class ColorToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            Color color => new SolidColorBrush(color),
            string colorStr when Color.TryParse(colorStr, out var parsedColor) => new SolidColorBrush(parsedColor),
            _ => Brushes.Transparent
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            ISolidColorBrush brush => brush.Color,
            _ => default
        };
    }
}