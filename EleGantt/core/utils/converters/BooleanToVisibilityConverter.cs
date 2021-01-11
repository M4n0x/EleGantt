using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EleGantt.core.utils.converters
{
    /// <summary>
    /// Utility class is used to convert Boolean to Int, convertBack is not supported !
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    class BooleanToVisibilityConverter : IValueConverter
    {
        //CommandParameter=1 to inverse sens of visibility
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool val = (bool)value;
            if (parameter != null && parameter.ToString().Equals("1"))
                val = !val;

            return (val) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
