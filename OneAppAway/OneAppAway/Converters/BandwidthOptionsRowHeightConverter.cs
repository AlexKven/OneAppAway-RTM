using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace OneAppAway.Converters
{
    public class BandwidthOptionsRowHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter == null) return 0;
            int param = int.Parse(parameter.ToString());
            int val = ((int)((BandwidthOptions)value));
            if (val == param)
                return new GridLength(50);
            else
                return new GridLength(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
