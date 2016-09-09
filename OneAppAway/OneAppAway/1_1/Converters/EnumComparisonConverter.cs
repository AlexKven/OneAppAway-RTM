using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace OneAppAway._1_1.Converters
{
    public class EnumComparisonConverter<T> : IValueConverter where T : struct
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value?.ToString() == parameter?.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            T result = new T();
            if (value is bool && (bool)value)
            {
                Enum.TryParse(parameter.ToString(), out result);
            }
            return result;
        }
    }
}
