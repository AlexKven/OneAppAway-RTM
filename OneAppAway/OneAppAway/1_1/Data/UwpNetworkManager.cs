using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace OneAppAway._1_1.Data
{
    public class UwpNetworkManager : NetworkManagerBase
    {
        internal UwpNetworkManager()
        {
            Windows.Networking.Connectivity.NetworkInformation.NetworkStatusChanged += NetworkInformation_NetworkStatusChanged;
            GetNetworkInfo();
        }

        private NetworkType _NetworkType = NetworkType.None;
        public override NetworkType NetworkType => _NetworkType;

        private bool _UnlimitedNetwork = true;
        public override bool UnlimitedNetwork => _UnlimitedNetwork;

        private async void NetworkInformation_NetworkStatusChanged(object sender)
        {
            if (Dispatcher == null) return;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            {
                GetNetworkInfo();
                OnNetworkTypeChanged();
            });
        }

        private void GetNetworkInfo()
        {
            var profile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
            if (profile == null)
                _NetworkType = NetworkType.None;
            else if (profile.GetConnectionCost().NetworkCostType == Windows.Networking.Connectivity.NetworkCostType.Unrestricted)
            {
                _NetworkType = NetworkType.Unlimited;
                _UnlimitedNetwork = true;
            }
            else
            {
                _NetworkType = NetworkType.Metered;
                _UnlimitedNetwork = false;
            }
        }

        public CoreDispatcher Dispatcher { set; get; }
    }
}
