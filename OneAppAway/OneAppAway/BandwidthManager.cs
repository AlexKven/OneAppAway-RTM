using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway
{
    public static class BandwidthManager
    {
        private static BandwidthOptions _AutoBandwidthOptions;

        public static BandwidthOptions EffectiveBandwidthOptions
        {
            get
            {
                return ApplicationSettings.BandwidthSettingStatic == BandwidthOptions.Auto ? _AutoBandwidthOptions : ApplicationSettings.BandwidthSettingStatic;
            }
        }

        static BandwidthManager()
        {
            ApplicationSettings.BandwidthSettingsChanged += ApplicationSettings_BandwidthSettingsChanged;
            Windows.Networking.Connectivity.NetworkInformation.NetworkStatusChanged += NetworkInformation_NetworkStatusChanged;
            GetNetworkInfo();
        }

        private static void ApplicationSettings_BandwidthSettingsChanged(object sender, EventArgs e)
        {
            OnEffectiveBandwidthOptionsChanged();
        }

        private static void NetworkInformation_NetworkStatusChanged(object sender)
        {
            GetNetworkInfo();
            if (ApplicationSettings.BandwidthSettingStatic == BandwidthOptions.Auto)
                OnEffectiveBandwidthOptionsChanged();
        }

        private static void GetNetworkInfo()
        {
            var profile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
            if (profile == null)
                _AutoBandwidthOptions = BandwidthOptions.None;
            else if (profile.GetConnectionCost().NetworkCostType == Windows.Networking.Connectivity.NetworkCostType.Unrestricted)
                _AutoBandwidthOptions = BandwidthOptions.Normal;
            else
                _AutoBandwidthOptions = BandwidthOptions.Low;
        }

        private static void OnEffectiveBandwidthOptionsChanged()
        {
            if (EffectiveBandwidthOptionsChanged != null)
                EffectiveBandwidthOptionsChanged(null, new EventArgs());
        }

        public static event EventHandler EffectiveBandwidthOptionsChanged;
    }
}
