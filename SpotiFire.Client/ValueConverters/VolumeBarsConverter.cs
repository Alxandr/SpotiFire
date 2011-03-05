using System;
using System.Globalization;
using System.Windows.Data;

namespace SpotiFire.SpotiClient.ValueConverters
{
    public class VolumeBarsConverter : IValueConverter
    {
        private const int COUNT = 3;
        private const double MIN = 0.25;
        private const double MAX = 1;
        private const int MIN_VAL = 0;
        private const int MAX_VAL = 100;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = 0, param = 0;
            if (!double.TryParse(value.ToString(), out val) || !double.TryParse(parameter.ToString(), out param))
                return GetRelative(0, 0);

            return GetRelative(val, param);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private double GetRelative(double val, double num)
        {
            var regionMin = num / (double)COUNT;
            var regionMax = (num + 1) / (double)COUNT;
            var relVal = (val - (double)MIN_VAL) / ((double)MAX_VAL - (double)MIN_VAL);

            return (MAX - MIN) * Math.Max(Math.Min((relVal - regionMin) / (regionMax - regionMin), 1), 0) + MIN;
        }
    }
}
