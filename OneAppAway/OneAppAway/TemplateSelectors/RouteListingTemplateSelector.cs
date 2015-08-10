using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OneAppAway.TemplateSelectors
{
    public class RouteListingTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ShortFormTemplate { get; set; }
        public DataTemplate LongFormTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container) => (((RouteListing)item).Name.Length > 8 || ((RouteListing)item).Description.Length > 80) ? LongFormTemplate : ShortFormTemplate;

        internal class RouteListing
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Agency { get; set; }
            public string RouteId { get; set; }
        }
    }
}
