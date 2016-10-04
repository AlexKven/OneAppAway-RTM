using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace OneAppAway._1_1.Converters
{
    public class ScrollViewerScrollModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool scroll = false;
            if (value is double && double.IsNaN((double)value))
                scroll = true;
            if (value is bool)
                scroll = (bool)value;
            if (parameter?.ToString() == "ScrollBarVisibility")
                return scroll ? ScrollBarVisibility.Auto : ScrollBarVisibility.Hidden;
            return scroll ? ScrollMode.Auto : ScrollMode.Disabled;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
