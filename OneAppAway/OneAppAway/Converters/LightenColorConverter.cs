using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;

namespace OneAppAway.Converters
{
    public class LightenColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double param = double.Parse(parameter.ToString());
            Color clr = (Color)value;
            if (param < 1)
                return Color.FromArgb(clr.A, (byte)(clr.R * param), (byte)(clr.G * param), (byte)(clr.B * param));
            else
                return Color.FromArgb(clr.A, (byte)(255.0 - (255.0 - clr.R) / param), (byte)(255.0 - (255.0 - clr.G) / param), (byte)(255.0 - (255.0 - clr.B) / param));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
