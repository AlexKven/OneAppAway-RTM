using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using OneAppAway._1_1.Data;

namespace OneAppAway._1_1.Converters
{
    public class StopSizeThresholdConverter : IValueConverter
    {
        public double LargeThreshold { get; set; }
        public double MediumThreshold { get; set; }
        public double SmallThreshold { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((double)value > LargeThreshold)
                return MapStopSize.Large;
            else if ((double)value > MediumThreshold)
                return MapStopSize.Medium;
            else if ((double)value > SmallThreshold)
                return MapStopSize.Small;
            else
                return MapStopSize.Invisible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
