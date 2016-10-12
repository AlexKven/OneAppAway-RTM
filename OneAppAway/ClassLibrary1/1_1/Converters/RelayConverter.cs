using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace OneAppAway._1_1.Converters
{
    public class RelayConverter<TSource, TDest, TParam> : IValueConverter
    {
        private Func<TSource, TParam, TDest> ConvertCallback;
        private Func<TDest, TParam, TSource> ConvertBackCallback;

        public RelayConverter(Func<TSource, TParam, TDest> convertCallback, Func<TDest, TParam, TSource> convertBackCallback)
        {
            ConvertCallback = convertCallback;
            ConvertBackCallback = convertBackCallback;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (ConvertCallback == null)
                throw new NotImplementedException();
            return ConvertCallback((TSource)value, (TParam)parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (ConvertBackCallback == null)
                throw new NotImplementedException();
            return ConvertBackCallback((TDest)value, (TParam)parameter);
        }
    }
}
