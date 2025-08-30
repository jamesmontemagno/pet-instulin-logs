using System.Globalization;
using PetInsulinLogs.Models;

namespace PetInsulinLogs.Converters;

public class OnTimeFlagDisplayConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            OnTimeFlag.OnTime => "On Time",
            OnTimeFlag.Early => "Early",
            OnTimeFlag.Late => "Late",
            null => "All",
            _ => value?.ToString() ?? "All"
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}