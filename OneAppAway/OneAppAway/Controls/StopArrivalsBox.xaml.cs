using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OneAppAway
{
    public sealed partial class StopArrivalsBox : UserControl
    {
        #region Constructor
        public StopArrivalsBox()
        {
            InitializeComponent();
        }
        #endregion

        #region Properties
        public static DependencyProperty StopProperty = DependencyProperty.Register("Stop", typeof(BusStop), typeof(StopArrivalsBox), new PropertyMetadata(new BusStop(), new PropertyChangedCallback(OnStopChangedStatic)));

        public BusStop Stop
        {
            get { return (BusStop)GetValue(StopProperty); }
            set { SetValue(StopProperty, value); }
        }

        private async static void OnStopChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            StopArrivalsBox typedSender = (StopArrivalsBox)sender;
            BusStop value = (BusStop)e.NewValue;
            typedSender.NameBlock.Text = value.Name;
            typedSender.DirectionImage.Source = new BitmapImage(new Uri(value.Direction == StopDirection.Unspecified ? "ms-appx:///Assets/Icons/BusBase20.png" : "ms-appx:///Assets/Icons/BusDirection" + value.Direction.ToString() + "20.png"));
            await typedSender.RefreshArrivals();
        }
        #endregion

        private CancellationTokenSource MasterCancellationTokenSource = new CancellationTokenSource();

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await RefreshArrivals();
        }

        private async Task RefreshArrivals()
        {
            ProgressIndicator.IsActive = true;
            var arrivals = await ApiLayer.GetBusArrivals(Stop.ID, MasterCancellationTokenSource.Token);
            var removals = MainStackPanel.Children.Where(child => !arrivals.Contains(((BusArrivalBox)child).Arrival));
            foreach (var item in removals)
                MainStackPanel.Children.Remove(item);
            foreach (var item in arrivals)
            {
                if (MainStackPanel.Children.Any(child => ((BusArrivalBox)child).Arrival == item))
                    ((BusArrivalBox)MainStackPanel.Children.First(child => ((BusArrivalBox)child).Arrival == item)).Arrival = item;
                else
                    MainStackPanel.Children.Add(new BusArrivalBox() { Arrival = item });
            }
            LastRefreshBox.Text = "Last update: " + DateTime.Now.ToString("h:mm:ss");
            ProgressIndicator.IsActive = false;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            ((App)App.Current).RootFrame.Navigate(typeof(StopViewPage), Stop.ID);
        }
    }
}
