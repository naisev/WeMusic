using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WeMusic.Converter
{
    class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(Visibility))
            {
                return (bool)value ? Visibility.Visible : Visibility.Hidden;
            }
            else if (targetType == typeof(bool))
            {
                return (Visibility)value == Visibility.Visible ? true : false;
            }
            else { return null; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            return null;
        }
    }
}
