using System;
using System.Windows.Data;

namespace SpotiFire.SpotiClient.ValueConverters
{
    public class TimeSpanSecondsConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is TimeSpan)
            {
                TimeSpan d = (TimeSpan)value;
                return d.TotalSeconds;
            }
            return 0D;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double)
                return TimeSpan.FromSeconds((double)value);

            return TimeSpan.Zero;
        }
    }
}
