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
    public class IsEarlyToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool bValOut;
            bool? bVal;
            if (value is bool?)
                bVal = (bool?)value;
            else if (bool.TryParse(value?.ToString(), out bValOut))
                bVal = bValOut;
            else
                bVal = null;
            var paramColor = (Color)parameter;
            if (parameter == null || !(parameter is Color))
                paramColor = Colors.LightGray;
            if (bVal.HasValue)
            {
                return new SolidColorBrush(bVal.Value ? Colors.Red : Lighten(paramColor, 0.5));
            }
            return new SolidColorBrush(Colors.White);
        }

        private Color Lighten(Color color, double amount) => Color.FromArgb(255, (byte)(color.R + (255 - color.R) * amount), (byte)(color.G + (255 - color.G) * amount), (byte)(color.B + (255 - color.B) * amount));

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
