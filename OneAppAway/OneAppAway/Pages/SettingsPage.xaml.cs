using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace OneAppAway
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : NavigationFriendlyPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }
        
        private bool IsLoading = true;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await Task.Delay(100);
            DelayDownloadingStopsLowBandwidthSwitch.IsOn = !SettingsManager.GetSetting("LimitedData.ManuallyDownloadStops", false, false);
            DelayDownloadingArrivalsLowBandwidthSwitch.IsOn = !SettingsManager.GetSetting("LimitedData.DelayDownloadingArrivals", false, false);
            DelayDownloadingSchedulesLowBandwidthSwitch.IsOn = !SettingsManager.GetSetting("LimitedData.DelayDownloadingSchedules", false, true);
            CancelOnBandwidthChangedLowBandwidthSwitch.IsOn = SettingsManager.GetSetting("CancelDownloadsOnBandwidthChanged", false, true);
            WarnOnDownloadLowBandwidthSwitch.IsOn = SettingsManager.GetSetting("LimitedData.WarnOnDownload", false, true);
            TechnicalModeSwitch.IsOn = SettingsManager.GetSetting("TechnicalMode", false, false);
            switch (SettingsManager.GetSetting("LaunchPage", false, 0))
            {
                case 0:
                    MapRadioButton.IsChecked = true;
                    break;
                case 1:
                    FavoritesRadioButton.IsChecked = true;
                    break;
                case 2:
                    RoutesRadioButton.IsChecked = true;
                    break;
            }
            switch (SettingsManager.GetSetting("LimitedData.LaunchPage", false, 0))
            {
                case 0:
                    LimitedDataMapRadioButton.IsChecked = true;
                    break;
                case 1:
                    LimitedDataFavoritesRadioButton.IsChecked = true;
                    break;
                case 2:
                    LimitedDataRoutesRadioButton.IsChecked = true;
                    break;
            }
            IsLoading = false;
        }

        private void DelayDownloadingStopsLowBandwidthSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!IsLoading)
            {
                SettingsManager.SetSetting("LimitedData.ManuallyDownloadStops", false, !DelayDownloadingStopsLowBandwidthSwitch.IsOn);
            }
        }

        private void MapRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsLoading)
            {
                SettingsManager.SetSetting("LaunchPage", false, 0);
            }
        }

        private void FavoritesRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsLoading)
            {
                SettingsManager.SetSetting("LaunchPage", false, 1);
            }
        }

        private void RoutesRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsLoading)
            {
                SettingsManager.SetSetting("LaunchPage", false, 2);
            }
        }

        private void LimitedDataMapRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsLoading)
            {
                SettingsManager.SetSetting("LimitedData.LaunchPage", false, 0);
            }
        }

        private void LimitedDataFavoritesRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsLoading)
            {
                SettingsManager.SetSetting("LimitedData.LaunchPage", false, 1);
            }
        }

        private void LimitedDataRoutesRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsLoading)
            {
                SettingsManager.SetSetting("LimitedData.LaunchPage", false, 2);
            }
        }

        private void TechnicalModeSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!IsLoading)
            {
                SettingsManager.SetSetting("TechnicalMode", false, TechnicalModeSwitch.IsOn);
            }
        }

        private void DelayDownloadingArrivalsLowBandwidthSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!IsLoading)
            {
                SettingsManager.SetSetting("LimitedData.DelayDownloadingArrivals", false, !DelayDownloadingArrivalsLowBandwidthSwitch.IsOn);
            }
        }

        private void DelayDownloadingSchedulesLowBandwidthSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!IsLoading)
            {
                SettingsManager.SetSetting("LimitedData.DelayDownloadingSchedules", false, !DelayDownloadingSchedulesLowBandwidthSwitch.IsOn);
            }
        }

        private void CancelOnBandwidthChangedLowBandwidthSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!IsLoading)
            {
                SettingsManager.SetSetting("CancelDownloadsOnBandwidthChanged", false, CancelOnBandwidthChangedLowBandwidthSwitch.IsOn);
            }
        }

        private void WarnOnDownloadLowBandwidthSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!IsLoading)
            {
                SettingsManager.SetSetting("LimitedData.WarnOnDownload", false, WarnOnDownloadLowBandwidthSwitch.IsOn);
            }
        }

        internal override void OnRefreshTitleBarControls(OuterFrame mainFrame, double totalWidth)
        {
            if (mainFrame.TitleContent.Content == null)
                mainFrame.TitleContent.Content = new TextBlock() { RequestedTheme = ElementTheme.Dark, VerticalAlignment = VerticalAlignment.Center, FontSize = 32, Text = "Settings" };
        }
    }
}
