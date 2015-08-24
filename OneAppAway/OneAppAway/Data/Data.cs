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
using static OneAppAway.DataSourceDescriptor;

namespace OneAppAway
{
    public static class Data
    {
        public const double MAX_LAT_RANGE = 0.01;
        public const double MAX_LON_RANGE = 0.015;

        public static async Task<BusStop[]> GetBusStopsForArea(GeoboundingBox bounds, Action<BusStop[], GeoboundingBox> stopsLoadedCallback, CancellationToken cancellationToken, bool offlineOnly)
        {
            List<BusStop> totalFoundStops = new List<BusStop>();
            try
            {
                var offlineStops = await FileManager.GetStopsForArea(bounds.NorthwestCorner, bounds.SoutheastCorner, cancellationToken);
                totalFoundStops.AddRange(offlineStops);
                foreach (var stop in offlineStops)
                {
                    if (!CachedStops.ContainsKey(stop.ID))
                        CachedStops.Add(stop.ID, stop);
                }
                if (stopsLoadedCallback != null)
                {
                    stopsLoadedCallback(offlineStops, null);
                }
                cancellationToken.ThrowIfCancellationRequested();
                if (!offlineOnly)
                {
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
                            BusStop[] foundStops = await ApiLayer.GetBusStopsForArea(new BasicGeoposition() { Latitude = bounds.SoutheastCorner.Latitude + (i + .5) * smallLatRange, Longitude = bounds.NorthwestCorner.Longitude + (j + .5) * smallLonRange }, smallLatRange, smallLonRange, cancellationToken);
                            foreach (var item in foundStops)
                            {
                                if (!totalFoundStops.Contains(item))
                                    totalFoundStops.Add(item);
                                if (!CachedStops.ContainsKey(item.ID))
                                    CachedStops.Add(item.ID, item);
                            }
                            if (stopsLoadedCallback != null)
                            {
                                stopsLoadedCallback(foundStops, null);
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException) { }
            return totalFoundStops.ToArray();
        }

        private static async Task<Tuple<T, DataRetrievalMessage>> CloudOrLocal<T>(Func<Task<T>> cloudCallback, Func<Task<T>> localCallback, DataRetrievalOptions options, params Type[] dontCatch)
        {
            T result = default(T);
            bool attempted = false;
            bool successful = false;
            bool fallbackAttempted = false;
            bool fallbackSuccessful = false;
            while (!attempted || (attempted && !successful && options.AllowFallback && !fallbackAttempted))
            {
                Action<bool> setSuccessful = (s) =>
                {
                    if (fallbackAttempted)
                        fallbackSuccessful = s;
                    else
                        successful = s;
                };
                try
                {
                    if (attempted)
                        fallbackAttempted = true;
                    else
                        attempted = true;
                    if (((options.PreferredSource == Cloud) ^ fallbackAttempted) && BandwidthManager.EffectiveBandwidthOptions == BandwidthOptions.None)
                        setSuccessful(false);
                    else
                    {
                        result = await (((options.PreferredSource == Cloud) ^ fallbackAttempted) ? cloudCallback() : localCallback());
                        setSuccessful(result != null);
                    }
                }
                catch (Exception ex) when (!dontCatch.Contains(ex.GetType()))
                {
                    setSuccessful(false);
                }
            }
            return new Tuple<T, DataRetrievalMessage>(result, new DataRetrievalMessage() { AttemptedSource = options.PreferredSource, AttemptSucceeded = successful, FallbackAttempted = fallbackAttempted, FallbackSucceeded = fallbackSuccessful });
        }

        public static async Task<Tuple<RealtimeArrival[], DataRetrievalMessage>> GetArrivals(string stopId, DataRetrievalOptions options, CancellationToken cancellationToken)
        {
            return await CloudOrLocal(
                () => ApiLayer.GetBusArrivals(stopId, cancellationToken)
                ,
                () => FileManager.GetScheduledArrivals(stopId)
                ,
                options,
                typeof(OperationCanceledException)
                );
        }

        public static async Task<Tuple<WeekSchedule, DataRetrievalMessage>> GetScheduleForStop(string id, DataRetrievalOptions options, CancellationToken cancellationToken)
        {
            return await CloudOrLocal(() => ApiLayer.GetScheduleForStop(id, cancellationToken), () => FileManager.LoadSchedule(id), options, typeof(OperationCanceledException));
        }

        public static async Task<Tuple<Tuple<BusStop[], string[]>, DataRetrievalMessage>> GetStopsAndShapesForRoute(string routeId, DataRetrievalOptions options, CancellationToken cancellationToken)
        {
            return await CloudOrLocal(() => ApiLayer.GetStopsAndShapesForRoute(routeId, cancellationToken), () => FileManager.GetStopsAndShapesForRouteFromCache(routeId), options, typeof(OperationCanceledException));
        }

        public static async Task<Tuple<TransitAgency[], DataRetrievalMessage>> GetTransitAgencies(DataRetrievalOptions options, CancellationToken cancellationToken)
        {
            return await CloudOrLocal(() => ApiLayer.GetTransitAgencies(cancellationToken), () => FileManager.GetAgenciesFromCache(), options, typeof(OperationCanceledException));
        }

        public static async Task<Tuple<BusRoute[], DataRetrievalMessage>> GetBusRoutesForAgency(string agencyId, DataRetrievalOptions options, CancellationToken cancellationToken)
        {
            return await CloudOrLocal(() => ApiLayer.GetBusRoutesForAgency(agencyId, cancellationToken), () => FileManager.GetBusRoutesForAgencyFromCache(agencyId), options, typeof(OperationCanceledException));
        }

        #region Cached Objects
        private static Dictionary<string, BusStop> CachedStops = new Dictionary<string, BusStop>();
        private static Dictionary<string, BusRoute> CachedRoutes = new Dictionary<string, BusRoute>();
        private static Dictionary<string, TransitAgency> CachedTransitAgencies = new Dictionary<string, TransitAgency>();
        #endregion

        #region Get Objects By ID
        public static async Task<BusStop?> GetBusStop(string id, CancellationToken cancellationToken)
        {
            if (CachedStops.ContainsKey(id))
                return CachedStops[id];
            var result = await CloudOrLocal(() => ApiLayer.GetBusStop(id, cancellationToken), () => FileManager.GetStopFromCache(id), new DataRetrievalOptions(DataSourceDescriptor.Local), typeof(OperationCanceledException));
            if (!CachedStops.ContainsKey(id) && result.Item2.FinalSource != null)
                CachedStops.Add(id, result.Item1.Value);
            return result.Item1;
        }

        public static async Task<TransitAgency?> GetTransitAgency(string id, CancellationToken cancellationToken)
        {
            if (CachedTransitAgencies.ContainsKey(id))
                return CachedTransitAgencies[id];
            var result = await CloudOrLocal(() => ApiLayer.GetTransitAgency(id, cancellationToken), () => FileManager.GetAgencyFromCache(id), new DataRetrievalOptions(DataSourceDescriptor.Local), typeof(OperationCanceledException));
            if (!CachedTransitAgencies.ContainsKey(id) && result.Item2.FinalSource != null)
                CachedTransitAgencies.Add(id, result.Item1.Value);
            return result.Item1;
        }

        public static async Task<BusRoute?> GetRoute(string id, CancellationToken cancellationToken)
        {
            if (CachedRoutes.ContainsKey(id))
                return CachedRoutes[id];
            var result = await CloudOrLocal(() => ApiLayer.GetBusRoute(id, cancellationToken), () => FileManager.GetRouteFromCache(id), new DataRetrievalOptions(DataSourceDescriptor.Local), typeof(OperationCanceledException));
            if (!CachedRoutes.ContainsKey(id) && result.Item2.FinalSource != null)
                CachedRoutes.Add(id, result.Item1.Value);
            return result.Item1;
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
