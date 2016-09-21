using OneAppAway._1_1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OneAppAway._1_1.Selectors
{
    public class RealTimeArrivalControlTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item)
        {
            //string value = ((item as RealTimeArrivalViewModel)?.TemplateType)?.ToString();
            //switch (value)
            //{
            //    case "Normal":
            //        return NormalTemplate;
            //    default:
            //        return null;
            //}
            throw new NotImplementedException();
        }
        
        public DataTemplate NormalTemplate { get; set; }
    }
}
