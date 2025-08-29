using System.Globalization;
using PetInsulinLogs.Models;

namespace PetInsulinLogs.Converters;

public class OnTimeFlagColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            OnTimeFlag.OnTime => Colors.Green,
            OnTimeFlag.Early => Colors.Orange,
            OnTimeFlag.Late => Colors.Red,
            _ => Colors.Gray
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}