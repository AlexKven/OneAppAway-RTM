using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage;

namespace OneAppAway
{
    public static class Data
    {
        public const double MAX_LAT_RANGE = 0.01;
        public const double MAX_LON_RANGE = 0.015;

        public static async Task<BusStop[]> GetBusStopsForArea(GeoboundingBox bounds, Action<BusStop[], GeoboundingBox> stopsLoadedCallback, CancellationToken cancellationToken)
        {
            List<BusStop> totalFoundStops = new List<BusStop>();
            double latRange = bounds.NorthwestCorner.Latitude - bounds.SoutheastCorner.Latitude;
            double lonRange = bounds.SoutheastCorner.Longitude - bounds.NorthwestCorner.Longitude;
            int latPieces = (int)Math.Ceiling(latRange / MAX_LAT_RANGE);
            int lonPieces = (int)Math.Ceiling(lonRange / MAX_LON_RANGE);
            double smallLatRange = latRange / (double)latPieces;
            double smallLonRange = lonRange / (double)lonPieces;
            for (int i = 0; i < latPieces; i++)
            {
                for (int j = 0; j < lonPieces; j++)
                {
                    BusStop[] foundStops = await ApiLayer.GetBusStops(new BasicGeoposition() { Latitude = bounds.SoutheastCorner.Latitude + (i + .5) * smallLatRange, Longitude = bounds.NorthwestCorner.Longitude + (j + .5) * smallLonRange }, smallLatRange, smallLonRange, cancellationToken);
                    foreach (var item in foundStops)
                    {
                        totalFoundStops.Add(item);
                        if (!CachedStops.ContainsKey(item.Code))
                            CachedStops.Add(item.Code, item);
                    }
                    if (stopsLoadedCallback != null)
                    {
                        stopsLoadedCallback(foundStops, null);
                    }
                }
            }
            return totalFoundStops.ToArray();
        }

        public static async Task<WeekSchedule> GetScheduleForStop(string id, CancellationToken cancellationToken)
        {
            WeekSchedule result = new WeekSchedule();
            for (int i = 0; i < 8; i++)
            {
                DaySchedule daySched = new DaySchedule();
                ServiceDay day = (ServiceDay)(int)Math.Pow(2, i);
                DateTime? date = HelperFunctions.DateForServiceDay(day);
                if (date.HasValue)
                {
                    daySched.LoadFromVerboseString(await ApiLayer.SendRequest("schedule-for-stop/" + id, new Dictionary<string, string>() {["date"] = date.Value.ToString("yyyy-MM-dd") }, cancellationToken));
                    result.AddServiceDay(day, daySched);
                }
            }
            return result;
        }

        #region Cached Objects
        private static Dictionary<string, BusStop> CachedStops = new Dictionary<string, BusStop>();
        private static Dictionary<string, BusRoute> CachedRoutes = new Dictionary<string, BusRoute>();
        private static Dictionary<string, TransitAgency> CachedTransitAgencies = new Dictionary<string, TransitAgency>();
        #endregion

        #region Get Objects By ID
        public static async Task<BusStop> GetBusStop(string id, CancellationToken cancellationToken)
        {
            if (CachedStops.ContainsKey(id))
                return CachedStops[id];
            var result = await ApiLayer.GetBusStop(id, cancellationToken);
            if (CachedStops.ContainsKey(id))
                CachedStops.Add(id, result);
            return result;
        }

        public static async Task<TransitAgency> GetTransitAgency(string id, CancellationToken cancellationToken)
        {
            if (CachedTransitAgencies.ContainsKey(id))
                return CachedTransitAgencies[id];
            var result = await ApiLayer.GetTransitAgency(id, cancellationToken);
            if (!CachedTransitAgencies.ContainsKey(id))
                CachedTransitAgencies.Add(id, result);
            return result;
        }

        public static async Task<BusRoute> GetRoute(string id, CancellationToken cancellationToken)
        {
            if (CachedRoutes.ContainsKey(id))
                return CachedRoutes[id];
            var result = await ApiLayer.GetBusRoute(id, cancellationToken);
            if (!CachedRoutes.ContainsKey(id))
                CachedRoutes.Add(id, result);
            return result;
        }
        #endregion

        #region Location
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
        #endregion
    }
}
