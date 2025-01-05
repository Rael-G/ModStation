using System.Globalization;
using Avalonia.Data.Converters;

namespace ModStation.Avalonia.StaticResources;

public class EnableStatusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? "(Enabled)" : "(Disabled)";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
