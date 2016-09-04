using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using static System.Math;

namespace OneAppAway._1_1.Selectors
{
    public class LargeAndSmallTitleBarTemplateSelector : TitleBarTemplateSelectorBase
    {
        public DataTemplate LargeTemplate { get; set; }
        public DataTemplate SmallTemplate { get; set; }
        public DataTemplate OverflowTemplate { get; set; }
        public double LargeTemplateThresholdWidth { get; set; }
        public double LargeTemplateThresholdWidthMobile { get; set; }
        public double PreferedLargeWidth { get; set; }
        public double PreferedSmallWidth { get; set; }

        public LargeAndSmallTitleBarTemplateSelector()
        {
            TitleTemplate = App.Current.Resources["SimpleTitleTemplate"] as DataTemplate;
        }
        private bool firstLoad = true;
        private bool oldLarge = false;

        public override void ReceiveAvailableSize(double size, bool onMobile)
        {
            bool large = onMobile ? size >= LargeTemplateThresholdWidthMobile : size >= LargeTemplateThresholdWidth;
            ControlsWidth = onMobile ? size : Min(large ? PreferedLargeWidth : PreferedSmallWidth, size);
            TitleWidth = size - ControlsWidth;
            if (large != oldLarge || firstLoad)
            {
                firstLoad = false;
                ControlsTemplate = large ? LargeTemplate : SmallTemplate;
                OverflowControlsTemplate = large ? null : OverflowTemplate;
            }
            oldLarge = large;
        }
        
    }
}
