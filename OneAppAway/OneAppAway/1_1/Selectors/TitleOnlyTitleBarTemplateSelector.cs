using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace OneAppAway._1_1.Selectors
{
    public class TitleOnlyTitleBarTemplateSelector : TitleBarTemplateSelectorBase
    {
        public TitleOnlyTitleBarTemplateSelector()
        {
            TitleTemplate = App.Current.Resources["SimpleTitleTemplate"] as DataTemplate;
        }
        
        public DataTemplate Template
        {
            get { return TitleTemplate; }
            set { TitleTemplate = value; }
        }

        public override void ReceiveAvailableSize(double size, bool onMobile) { TitleWidth = size; }
    }
}
