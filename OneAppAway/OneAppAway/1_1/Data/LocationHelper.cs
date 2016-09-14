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
        public static async Task<BasicGeoposition?> GetCurrentLocation(PositionAccuracy acc, TimeSpan timeout)
        {
            var loc = new Geolocator();
            loc.DesiredAccuracy = acc;
            var result = (await loc.GetGeopositionAsync(TimeSpan.FromSeconds(30), timeout)).Coordinate.Point.Position;
            SettingsManager.SetSetting<double[]>("LastLocation", true, new double[] { result.Latitude, result.Longitude });
            return result;
        }

        public static BasicGeoposition? GetLastKnownLocation()
        {
            double[] data = SettingsManager.GetSetting<double[]>("LastLocation", true);
            if (data == null) return null;
            return new BasicGeoposition() { Latitude = data[0], Longitude = data[1] };
        }

        public static async Task ProgressivelyAcquireLocation(Action<BasicGeoposition> OnLocationFound, TimeSpan timeout)
        {
            var task = GetCurrentLocation(PositionAccuracy.Default, timeout);
            var loc = GetLastKnownLocation();
            if (loc != null && !task.IsCompleted)
                OnLocationFound(loc.Value);
            loc = await task;
            if (loc != null)
                OnLocationFound(loc.Value);
        }
    }
}
