using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace OneAppAway.Converters
{
    public class AddSubtractConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (double)value + (parameter == null ? 0 : double.Parse(parameter.ToString()));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (double)value - (parameter == null ? 0 : double.Parse(parameter.ToString()));
        }
    }
}
