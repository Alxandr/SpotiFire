using System;
using System.Globalization;
using System.Windows.Data;

namespace SpotiFire.SpotiClient.ValueConverters
{
    public class ThumbIndicatorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                double val = (double)values[0],
                    min = (double)values[1],
                    max = (double)values[2],
                    width = (double)values[3] - 5;

                if (min == max)
                    return width;

                double ret = ((val - min) / (max - min)) * width;
                return ret + 5;
            }
            catch
            {
                return 0;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
