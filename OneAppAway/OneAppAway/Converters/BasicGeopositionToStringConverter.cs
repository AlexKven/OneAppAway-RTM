using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Data;

namespace OneAppAway.Converters
{
    public class BasicGeopositionToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            BasicGeoposition typedValue = (BasicGeoposition)value;
            string format = parameter as string;
            if (format == null)
                return typedValue.Latitude + ", " + typedValue.Longitude;
            else
                return typedValue.Latitude.ToString(format) + ", " + typedValue.Longitude.ToString(format);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
