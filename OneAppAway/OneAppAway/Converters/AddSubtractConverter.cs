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
            double amount;
            if (!UseParameter)
                amount = Amount;
            else
                amount = (parameter == null ? 0 : double.Parse(parameter.ToString()));
            return (double)value + amount;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            double amount;
            if (!UseParameter)
                amount = Amount;
            else
                amount = (parameter == null ? 0 : double.Parse(parameter.ToString()));
            return (double)value - amount;
        }

        public double Amount { get; set; }

        public bool UseParameter { get; set; } = true;
    }
}
