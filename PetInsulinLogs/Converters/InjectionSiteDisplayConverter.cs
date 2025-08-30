using System.Globalization;
using PetInsulinLogs.Models;

namespace PetInsulinLogs.Converters;

public class InjectionSiteDisplayConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            InjectionSite.LeftShoulder => "Left Shoulder",
            InjectionSite.MiddleShoulder => "Middle Shoulder",
            InjectionSite.RightShoulder => "Right Shoulder",
            InjectionSite.Other => "Other",
            null => "All",
            _ => value?.ToString() ?? "All"
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}