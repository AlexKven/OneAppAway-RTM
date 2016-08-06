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
        public double Threshold { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((double)value > Threshold)
            {
                System.Diagnostics.Debug.WriteLine("Medium");
                return MapStopSize.Medium;
            }
            System.Diagnostics.Debug.WriteLine("Small");
            return MapStopSize.Small;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
