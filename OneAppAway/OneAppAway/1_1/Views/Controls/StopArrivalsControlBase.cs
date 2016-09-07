using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OneAppAway._1_1.Views.Controls
{
    public class StopArrivalsControlBase : UserControl
    {
        public bool ShowRoutesList
        {
            get { return (bool)GetValue(ShowRoutesListProperty); }
            set { SetValue(ShowRoutesListProperty, value); }
        }
        public static readonly DependencyProperty ShowRoutesListProperty =
            DependencyProperty.Register("ShowRoutesList", typeof(bool), typeof(StopArrivalsControlBase), new PropertyMetadata(true));
    }
}
