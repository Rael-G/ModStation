using System.Globalization;
using Avalonia.Data.Converters;

namespace ModStation.Avalonia.StaticResources;

public class EnableStatusConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool isEnabled = (bool)value!;

        if (parameter is string format)
        {
            if (format == "EnableDisable")
            {
                return isEnabled ? "Enable" : "Disable";
            }
            else if (format == "EnabledDisabled")
            {
                return isEnabled ? "Enabled" : "Disabled";
            }
        }

        return isEnabled ? "Enable" : "Disable";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
