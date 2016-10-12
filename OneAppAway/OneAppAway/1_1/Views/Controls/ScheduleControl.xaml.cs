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
    public sealed partial class ScheduleControl : UserControl
    {
        private ScheduleControlViewModel VM;
        public ScheduleControl()
        {
            this.InitializeComponent();
            VM = new ScheduleControlViewModel();
            MainGrid.DataContext = VM;
        }
        
        public TransitStop Stop
        {
            get { return (TransitStop)GetValue(StopProperty); }
            set { SetValue(StopProperty, value); }
        }
        public static readonly DependencyProperty StopProperty =
            DependencyProperty.Register("Stop", typeof(TransitStop), typeof(ScheduleControl), new PropertyMetadata(new TransitStop(), (s, e) =>
            {
                if (e.NewValue is TransitStop)
                    ((ScheduleControl)s).VM.Stop = (TransitStop)e.NewValue;
            }));
    }
}
