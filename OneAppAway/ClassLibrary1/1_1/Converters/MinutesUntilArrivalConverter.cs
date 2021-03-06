﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace OneAppAway._1_1.Converters
{
    public class MinutesUntilArrivalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (int)((DateTime)parameter - (DateTime)value).TotalMinutes;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public static MinutesUntilArrivalConverter Instance { get; } = new MinutesUntilArrivalConverter();
    }
}
