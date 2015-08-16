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
            this.InitializeComponent();
        }

        private bool IsLoading = true;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            IsLoading = true;
            DelayDownloadingStopsNormalBandwidthSwitch.IsOn = !SettingsManager.GetSetting<bool>("NormalBandwidth.ManuallyDownloadStops", false, false);
            DelayDownloadingStopsLowBandwidthSwitch.IsOn = !SettingsManager.GetSetting<bool>("LowBandwidth.ManuallyDownloadStops", false, false);
            IsLoading = false;
        }

        private void DelayDownloadingStopsNormalBandwidthSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!IsLoading)
            {
                SettingsManager.SetSetting<bool>("NormalBandwidth.ManuallyDownloadStops", false, !DelayDownloadingStopsNormalBandwidthSwitch.IsOn);
            }
        }

        private void DelayDownloadingStopsLowBandwidthSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!IsLoading)
            {
                SettingsManager.SetSetting<bool>("LowBandwidth.ManuallyDownloadStops", false, !DelayDownloadingStopsLowBandwidthSwitch.IsOn);
            }
        }
    }
}
