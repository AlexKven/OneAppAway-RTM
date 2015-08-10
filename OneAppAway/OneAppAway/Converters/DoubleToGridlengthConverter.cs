using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace OneAppAway.Converters
{
    public class DoubleToGridlengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((double)value < 0) return new GridLength();
            return new GridLength((double)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return ((GridLength)value).Value;
        }
    }
}
