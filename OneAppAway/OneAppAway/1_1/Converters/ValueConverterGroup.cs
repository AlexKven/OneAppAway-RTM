﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace OneAppAway._1_1.Converters
{
    public class ValueConverterGroup : List<IValueConverter>, IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return this.Aggregate(value, (current, converter) => converter.Convert(current, targetType, parameter, language));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return this.AsEnumerable().Reverse().Aggregate(value, (current, converter) => converter.ConvertBack(current, targetType, parameter, language));
        }

        #endregion
    }
}
