using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace OneAppAway.Converters
{
    public class RandomConverter : IValueConverter
    {
        private static Random Generator = new Random();
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double input = double.Parse(value.ToString());
            double param = double.Parse(parameter.ToString());
            double rnd = Generator.NextDouble();
            unchecked
            {
                Generator = new Random(Generator.Next() + Generator.Next());
            }
            return input * (1.0 + 2.0 * param * (.5 - rnd));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
