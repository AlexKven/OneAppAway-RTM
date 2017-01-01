using OneAppAway._1_1.Data;
using OneAppAway._1_1.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
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
            DependencyProperty.Register("Arrival", typeof(RealTimeArrival), typeof(RealTimeArrivalControl), new PropertyMetadata(new RealTimeArrival(), OnArrivalChangedStatic));
        static void OnArrivalChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var typedSender = sender as RealTimeArrivalControl;
            if (e.NewValue is RealTimeArrival)
            {
                var viewModel = new RealTimeArrivalViewModel((RealTimeArrival)e.NewValue);
                typedSender.MainButton.DataContext = viewModel;
                if (viewModel.HasLongRouteName)
                    VisualStateManager.GoToState(typedSender, "LongRouteNameArrivalState", true);
                if (viewModel.IsFrequencyBased)
                    VisualStateManager.GoToState(typedSender, "FrequencyBasedArrivalState", true);
            }
        }
        
        public ICommand NavigateToLocationCommand
        {
            get { return (ICommand)GetValue(NavigateToLocationCommandProperty); }
            set { SetValue(NavigateToLocationCommandProperty, value); }
        }
        public static readonly DependencyProperty NavigateToLocationCommandProperty =
            DependencyProperty.Register("NavigateToLocationCommand", typeof(ICommand), typeof(RealTimeArrivalControl), new PropertyMetadata(null));
        
        private void Flyout_Opened(object sender, object e)
        {
        }

        private void FindVehicle_Click(object sender, RoutedEventArgs e)
        {
            if (Arrival.KnownVehicleLocation.HasValue)
                NavigateToLocationCommand?.Execute(Arrival.KnownVehicleLocation);
        }

        private void FindVehicleScheduled_Click(object sender, RoutedEventArgs e)
        {
            if (Arrival.ScheduledVehicleLocation.HasValue)
                NavigateToLocationCommand?.Execute(Arrival.ScheduledVehicleLocation);
        }
    }
}
