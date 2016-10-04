using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace OneAppAway._1_1.Views.Controls
{
    public class PassivelySizedContentControl : ContentControl
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            var result = base.MeasureOverride(availableSize);
            return result;
        }
    }
}
