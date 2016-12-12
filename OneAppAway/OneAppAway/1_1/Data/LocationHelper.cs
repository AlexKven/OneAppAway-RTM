using OneAppAway._1_1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace OneAppAway._1_1.Data
{
    internal static class LocationHelper
    {
        private static Geolocator Locator = new Geolocator();

        static LocationHelper()
        {
            Locator.ReportInterval = 15000;
            Locator.PositionChanged += Locator_PositionChanged;
            Locator.StatusChanged += Locator_StatusChanged;
        }

        private static void Locator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            App.Dispatcher?.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                StatusChanged?.Invoke(null, args);
            });
        }
        
        private static void Locator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            App.Dispatcher?.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                LocationChanged?.Invoke(null, args);
            });
        }

        public static async Task<Geoposition> GetGeoposition(PositionAccuracy acc, TimeSpan timeout)
        {
            var loc = new Geolocator();
            loc.DesiredAccuracy = acc;
            var result = (await loc.GetGeopositionAsync(TimeSpan.FromSeconds(30), timeout));
            SettingsManager.SetSetting<double[]>("LastLocation", true, new double[] { result.Coordinate.Point.Position.Latitude, result.Coordinate.Point.Position.Longitude });
            return result;
        }

        public static async Task<LatLon?> GetCurrentLocation(PositionAccuracy acc, TimeSpan timeout)
        {
            return (await GetGeoposition(acc, timeout)).Coordinate.Point.ToLatLon();
        }

        public static PositionStatus Status => Locator.LocationStatus;

        public static LatLon? GetLastKnownLocation()
        {
            double[] data = SettingsManager.GetSetting<double[]>("LastLocation", true);
            if (data == null) return null;
            return new LatLon(data[0], data[1]);
        }

        public static async Task ProgressivelyAcquireLocation(Action<LatLon> OnLocationFound, TimeSpan timeout)
        {
            var task = GetCurrentLocation(PositionAccuracy.Default, timeout);
            var loc = GetLastKnownLocation();
            if (loc != null && !task.IsCompleted)
                OnLocationFound(loc.Value);
            loc = await task;
            if (loc != null)
                OnLocationFound(loc.Value);
        }

        public static event EventHandler<PositionChangedEventArgs> LocationChanged;
        public static event EventHandler<StatusChangedEventArgs> StatusChanged;
    }
}
