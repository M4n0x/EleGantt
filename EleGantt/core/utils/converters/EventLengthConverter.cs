using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace EleGantt.core.utils.converters
{
    //see https://stackoverflow.com/questions/37949599/how-to-dynamically-draw-a-timeline-in-wpf
    public class EventLengthConverter : IMultiValueConverter
    {
        private DateTime lastStartProject;
        private double lastCellWidth;
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            /*
             * From Task class :
             * public string Name;
             * public DateTime DateStart;
             * public DateTime DateEnd;
             * the dates are important here
             */
            DateTime startProjectTime = (DateTime)values[0];
            DateTime startTaskTime = (DateTime)values[1];
            double containerWidth = (double)values[2];
            double factor = (startTaskTime - startProjectTime).Days;
            double rval = factor * containerWidth;

            //stock values for two-way
            lastStartProject = startProjectTime;
            lastCellWidth = containerWidth;

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
            Thickness? th = value as Thickness?;
            if (th.HasValue)
            {
                foreach(var i in targetTypes)
                    Console.WriteLine($"expected :  {i}");

                double marginLeft = th.Value.Left;
                double delta = marginLeft / lastCellWidth;
                Console.WriteLine($"delta : {delta}, new day : {lastStartProject.AddDays(delta)}");
                return new object[] { DateTime.Now.AddDays(1), DateTime.Now.AddDays(10), 10};
            }
            return new object[] { };
        }
    }
}
