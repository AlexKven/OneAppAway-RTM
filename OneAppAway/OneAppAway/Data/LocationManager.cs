using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.UI.Core;

namespace OneAppAway
{
    public static class LocationManager
    {
        private static Geolocator Locator;
        static LocationManager()
        {
            Locator = new Geolocator();
            Locator.MovementThreshold = 5;
            Locator.ReportInterval = 1000;
            Locator.PositionChanged += Locator_PositionChanged;
            Locator.StatusChanged += Locator_StatusChanged;
        }

        private static async void Locator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            if (Dispatcher == null) return;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            {
                OnLocationChanged();
            });
        }

        private static async void Locator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            if (Dispatcher == null) return;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            {
                OnLocationChanged();
            });
        }

        public static async Task<BasicGeoposition?> GetPosition()
        {
            if (Locator.LocationStatus == PositionStatus.Ready)
            {
                var loc = await Locator.GetGeopositionAsync();
                return loc == null ? null : new BasicGeoposition?(new BasicGeoposition() { Latitude = loc.Coordinate.Point.Position.Latitude, Longitude = loc.Coordinate.Point.Position.Longitude });
            }
            return null;
        }

        private static void OnLocationChanged()
        {
            if (LocationChanged != null)
                LocationChanged(null, new EventArgs());
        }

        public static event EventHandler LocationChanged;

        public static CoreDispatcher Dispatcher { set; private get; }
    }
}
