using OneAppAway._1_1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;

namespace OneAppAway._1_1.ViewModels
{
    class TransitMapPageUwpViewModel : TransitMapPageViewModel
    {
        public TransitMapPageUwpViewModel(MemoryCache cache)
            : base(cache) { }

        protected override bool MultiSelect
        {
            get
            {
                return (Window.Current.CoreWindow.GetKeyState(Windows.System.VirtualKey.Control) & Windows.UI.Core.CoreVirtualKeyStates.Down) == Windows.UI.Core.CoreVirtualKeyStates.Down;
            }
        }

        protected override async Task GetLocation(Action<LatLon> locationCallback)
        {
            await LocationHelper.ProgressivelyAcquireLocation(pos => locationCallback(pos.ToLatLon()));
        }
    }
}
