using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace OneAppAway._1_1.Data
{
    internal static class LocationHelper
    {
        public static async Task<BasicGeoposition?> GetCurrentLocation(PositionAccuracy acc)
        {
            var loc = new Geolocator();
            try
            {
                loc.DesiredAccuracy = acc;
                var result = (await loc.GetGeopositionAsync()).Coordinate.Point.Position;
                SettingsManager.SetSetting<double[]>("LastLocation", true, new double[] { result.Latitude, result.Longitude });
                return result;
            }
            catch (Exception) { }
            return null;
        }

        public static BasicGeoposition? GetLastKnownLocation()
        {
            double[] data = SettingsManager.GetSetting<double[]>("LastLocation", true);
            if (data == null) return null;
            return new BasicGeoposition() { Latitude = data[0], Longitude = data[1] };
        }

        public static async Task ProgressivelyAcquireLocation(Action<BasicGeoposition> OnLocationFound)
        {
            var task = GetCurrentLocation(PositionAccuracy.Default);
            var loc = GetLastKnownLocation();
            if (loc != null && !task.IsCompleted)
                OnLocationFound(loc.Value);
            loc = await task;
            if (loc != null)
                OnLocationFound(loc.Value);
        }
    }
}
