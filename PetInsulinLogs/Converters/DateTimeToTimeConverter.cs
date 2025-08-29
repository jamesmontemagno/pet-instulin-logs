using System.Globalization;

namespace PetInsulinLogs.Converters;

public class DateTimeToTimeConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dateTime)
        {
            return dateTime.TimeOfDay;
        }
        return DateTime.Now.TimeOfDay;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TimeSpan time && parameter is DateTime date)
        {
            return date.Date.Add(time);
        }
        return DateTime.Now;
    }
}