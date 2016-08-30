using OneAppAway._1_1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace OneAppAway._1_1.Converters
{
    public class LatLonTransformConverter : IValueConverter
    {
        public Func<LatLon, LatLon> Transform { get; set; }
        public Func<LatLon, LatLon> ReverseTransform { get; set; }

        public LatLonTransformConverter()
        {
            Transform = ll => ll;
            ReverseTransform = ll => ll;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Transform((LatLon)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return ReverseTransform((LatLon)value);
        }
    }
}
