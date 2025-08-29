using System.Globalization;

namespace PetInsulinLogs.Converters;

public class DateTimeToDateConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dateTime)
        {
            return dateTime.Date;
        }
        return DateTime.Today;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime date && parameter is DateTime time)
        {
            return date.Date.Add(time.TimeOfDay);
        }
        return DateTime.Now;
    }
}