using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace EleGantt.core.utils.converters
{
    //see https://stackoverflow.com/questions/37949599/how-to-dynamically-draw-a-timeline-in-wpfhttps://stackoverflow.com/questions/37949599/how-to-dynamically-draw-a-timeline-in-wpf
    public class EventLengthConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            /*
             * From Task class :
             * public string Name;
             * public DateTime DateStart;
             * public DateTime DateEnd;
             * the dates are important here
             */
            TimeSpan timelineDuration = (TimeSpan)values[0];
            TimeSpan relativeTime = (TimeSpan)values[1];
            double containerWidth = (double)values[2];
            double factor = relativeTime.TotalSeconds / timelineDuration.TotalSeconds;
            double rval = factor * containerWidth;

            if (targetType == typeof(Thickness))
            {
                return new Thickness(rval, 0, 0, 0);
            }
            else
            {
                return rval;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
