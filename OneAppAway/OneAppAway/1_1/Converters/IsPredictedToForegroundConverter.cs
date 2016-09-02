using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace OneAppAway._1_1.Converters
{
    public class IsPredictedToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool val = false;
            if (value is bool)
                val = (bool)value;
            Color paramColor;
            if (!(parameter is Color))
                val = false;
            else
                paramColor = (Color)parameter;
            return new SolidColorBrush(val ? Lighten(paramColor, 0.5) : Colors.White);
        }

        private Color Lighten(Color color, double amount) => Color.FromArgb(255, (byte)(color.R + (255 - color.R) * amount), (byte)(color.G + (255 - color.G) * amount), (byte)(color.B + (255 - color.B) * amount));

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
