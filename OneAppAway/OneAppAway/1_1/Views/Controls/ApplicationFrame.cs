using OneAppAway._1_1.Views.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace OneAppAway._1_1.Views.Controls
{
    public class ApplicationFrame : Frame
    {
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            ContentChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler ContentChanged;
    }
}
