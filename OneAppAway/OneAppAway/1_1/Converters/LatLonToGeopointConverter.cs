using OneAppAway._1_1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Data;

namespace OneAppAway._1_1.Converters
{
    public class LatLonToGeopointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is LatLon)
                return ((LatLon)value).ToGeopoint();
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Geopoint)
                return ((Geopoint)value).ToLatLon();
            return null;
        }

        public static LatLonToGeopointConverter Instance { get; }

        static LatLonToGeopointConverter()
        {
            Instance = new LatLonToGeopointConverter();
        }
    }
}
