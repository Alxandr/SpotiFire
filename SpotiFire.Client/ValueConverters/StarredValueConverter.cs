using System;
using System.Globalization;
using System.Windows.Data;

namespace SpotiFire.SpotiClient.ValueConverters
{
    public class StarredValueConverter : IValueConverter
    {
        private const string STARRED = "/Images/ico_track_starred.png";
        private const string UNSTARRED = "/Images/ico_track_unstarred.png";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? val = value as bool?;
            if (val == null)
                val = false;

            bool b = val.Value;
            if (b)
                return STARRED;
            else
                return UNSTARRED;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = value as string;

            if (val != UNSTARRED)
                return true;
            return false;
        }
    }
}
