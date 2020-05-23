using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WeMusic.Converter
{
    public class TimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(string))
            {
                return ((TimeSpan)value).ToString("mm':'ss");
            }
            else if(targetType == typeof(double))
            {
                return ((TimeSpan)value).TotalSeconds;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() == typeof(double))
            {
                return TimeSpan.FromSeconds((double)value);
            }
            return null;
        }
    }
}
