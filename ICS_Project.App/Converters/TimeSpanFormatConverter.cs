using ICS_Project.App.Extensions;
using System.Globalization;

namespace ICS_Project.App.Converters 
{
    public class TimeSpanFormatConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpan)
            {
                return timeSpan.ToFormattedDuration();
            }
            return value; // Or string.Empty or a default display if value is not a TimeSpan
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}