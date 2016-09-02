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
    public class DegreeOfConfidenceToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double dVal;
            if (value is double)
                dVal = (double)value;
            else if (!double.TryParse(value.ToString(), out dVal))
                return new SolidColorBrush(Colors.Transparent);
            if (parameter == null || !(parameter is Color))
                return new SolidColorBrush(Colors.Transparent);
            var paramColor = (Color)parameter;
            if (double.IsNaN(dVal))
                return new SolidColorBrush(Alpha(Darken(paramColor, 3), 64));
            if (dVal < 0 || dVal > 1)
                return new SolidColorBrush(Blend(paramColor, BlackAndWhite(paramColor), 0.5));
            return new SolidColorBrush(Blend(Darken(paramColor, 5), paramColor, dVal));
        }

        private Color Blend(Color left, Color right, double portion)
        {
            double[] rgbLeft = new[] { (double)left.R, (double)left.G, (double)left.B };
            double[] rgbRight = new[] { (double)right.R, (double)right.G, (double)right.B };
            var rgbResult = System.Linq.Enumerable.Zip(rgbLeft, rgbRight, (l, r) => l + (r - l) * portion).ToArray();
            return Color.FromArgb(255, (byte)rgbResult[0], (byte)rgbResult[1], (byte)rgbResult[2]);
        }

        private Color Alpha(Color color, byte a)
        {
            return Color.FromArgb(a, color.R, color.G, color.B);
        }

        private Color Darken(Color color, double amount) => Color.FromArgb(255, (byte)(color.R / amount), (byte)(color.G / amount), (byte)(color.B / amount));
        private Color BlackAndWhite(Color color)
        {
            byte mean = (byte)((color.R + color.G + color.B) / 3);
            return Color.FromArgb(255, mean, mean, mean);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
