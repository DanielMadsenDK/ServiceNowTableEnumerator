namespace SN.WidEnum;

using Microsoft.Maui.Controls;
using System.Globalization;
public class ColorValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool? val = (bool?)value;
        if (val == null)
            return Color.FromArgb("#FF000000"); //black
        if(val == false)
            return Color.FromArgb("#FF228B22"); //green
        return Color.FromArgb("#FF8B0000"); //red
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}