using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace OneAppAway._1_1.Converters
{
    public class NullZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return 0.0;
            else
            {
                if (parameter is double)
                    return (double)parameter;
                else
                    return double.Parse(parameter.ToString());
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public static NullZeroConverter Instance { get; }

        static NullZeroConverter()
        {
            Instance = new NullZeroConverter();
        }
    }
}
