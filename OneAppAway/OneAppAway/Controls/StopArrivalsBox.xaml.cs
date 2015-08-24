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

        public bool HideTitle
        {
            get { return !TitleRow.Height.IsAuto; }
            set { TitleRow.Height = value ? new GridLength(0) : GridLength.Auto; }
        }

        private async static void OnStopChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            StopArrivalsBox typedSender = (StopArrivalsBox)sender;
            BusStop value = (BusStop)e.NewValue;
            typedSender.NameBlock.Text = value.Name;
            typedSender.DirectionImage.Source = new BitmapImage(new Uri(value.Direction == StopDirection.Unspecified ? "ms-appx:///Assets/Icons/BusBase20.png" : "ms-appx:///Assets/Icons/BusDirection" + value.Direction.ToString() + "20.png"));
            await typedSender.RefreshArrivals(false);
        }
        #endregion

        private CancellationTokenSource MasterCancellationTokenSource = new CancellationTokenSource();

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await RefreshArrivals(true);
        }

        public async Task RefreshArrivals(bool forceOnline)
        {
            ProgressIndicator.IsActive = true;
            bool checkOnline = !SettingsManager.GetSetting("LimitedData.DelayDownloadingArrivals", false, false); //#
            DataSourceDescriptor preferredSource = forceOnline ? DataSourceDescriptor.Cloud : (!checkOnline && BandwidthManager.EffectiveBandwidthOptions == BandwidthOptions.Low) ? DataSourceDescriptor.Local : DataSourceDescriptor.Cloud;
            var arrivals = await Data.GetArrivals(Stop.ID, new DataRetrievalOptions(preferredSource), MasterCancellationTokenSource.Token);
            var removals = MainStackPanel.Children.Where(child => !arrivals.Item1.Contains(((BusArrivalBox)child).Arrival));
            foreach (var item in removals)
                MainStackPanel.Children.Remove(item);
            if (arrivals.Item1 != null)
            {
                foreach (var item in arrivals.Item1)
                {
                    if (MainStackPanel.Children.Any(child => ((BusArrivalBox)child).Arrival == item))
                        ((BusArrivalBox)MainStackPanel.Children.First(child => ((BusArrivalBox)child).Arrival == item)).Arrival = item;
                    else
                        MainStackPanel.Children.Add(new BusArrivalBox() { Arrival = item });
                }
                LastRefreshBox.Text = "Last update: " + DateTime.Now.ToString("h:mm:ss");
            }
            if (arrivals.Item2.FinalSource == null)
            {
                if (preferredSource == DataSourceDescriptor.Local)
                {
                    MessageBlock.Text = "No schedules downloaded for this stop. Tap \"Refresh\" to download realtime arrival data from the cloud.";
                    MessageBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBlock.Text = "Couldn't download arrival data, and there are no schedules downloaded for this stop. Tap \"Refresh\" to try again.";
                    MessageBlock.Visibility = Visibility.Visible;
                }
            }
            else if (arrivals.Item2.FinalSource == DataSourceDescriptor.Local)
            {
                if (preferredSource == DataSourceDescriptor.Local)
                {
                    MessageBlock.Text = "Only showing arrivals in downloaded schedules. Tap \"Refresh\" to download realtime arrival data from the cloud.";
                    MessageBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBlock.Text = "Couldn't download arrival data; showing arrivals in downloaded schedules instead. Tap \"Refresh\" to try again.";
                    MessageBlock.Visibility = Visibility.Visible;
                }
            }
            else
            {
                MessageBlock.Visibility = Visibility.Collapsed;
            }
            ProgressIndicator.IsActive = false;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            ((App)App.Current).RootFrame.Navigate(typeof(StopViewPage), Stop.ID);
        }

        private void IntermediateCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize != e.PreviousSize && e.NewSize.Width > 0 && e.NewSize.Width != MainStackPanel.Width)
            {
                MainStackPanel.Width = e.NewSize.Width;
            }
        }

        private void MainStackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize != e.PreviousSize && e.NewSize.Height > 0 && e.NewSize.Height != IntermediateCanvas.Height)
            {
                IntermediateCanvas.Height = e.NewSize.Height;
            }
        }
    }
}
