using OneAppAway._1_1.Data;
using OneAppAway._1_1.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OneAppAway._1_1.Views.Controls
{
    public sealed partial class RealTimeArrivalControl : UserControl
    {
        public RealTimeArrivalControl()
        {
            this.InitializeComponent();
        }
        
        public RealTimeArrival Arrival
        {
            get { return (RealTimeArrival)GetValue(ArrivalProperty); }
            set { SetValue(ArrivalProperty, value); }
        }
        public static readonly DependencyProperty ArrivalProperty =
            DependencyProperty.Register("Arrival", typeof(RealTimeArrival), typeof(RealTimeArrivalControl), new PropertyMetadata(new RealTimeArrival(), (s, e) => ((RealTimeArrivalControl)s).DataContext = new RealTimeArrivalViewModel((RealTimeArrival)e.NewValue)));
    }
}
