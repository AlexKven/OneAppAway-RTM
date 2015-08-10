using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace OneAppAway.Converters
{
    class MaxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double max = double.Parse(parameter.ToString());
            if ((double)value > max)
                return max;
            else
                return (double)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    class MinConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double min = double.Parse(parameter.ToString());
            if ((double)value < min)
                return min;
            else
                return (double)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
