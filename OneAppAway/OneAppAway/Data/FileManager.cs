using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Search;
using static OneAppAway.CompactFormatter;
using static System.Math;

namespace OneAppAway
{
    public static class FileManager
    {
        static FileManager()
        {
            for (int i = 0; i < 17; i++)
                SavedStopCache[i] = new WeakReference<List<BusStop>>(null);
        }

        private static GeoboundingBox DowntownSeattleZone = new GeoboundingBox(new BasicGeoposition() { Latitude = 47.628, Longitude = -122.374 }, new BasicGeoposition() { Latitude = 47.589, Longitude = -122.300 });

        #region Cached Objects
        private static WeakReference<List<BusStop>>[] SavedStopCache = new WeakReference<List<BusStop>>[17];
        private static WeakReference<List<Tuple<BusRoute, string[], string[]>>> SavedRouteCache = new WeakReference<List<Tuple<BusRoute, string[], string[]>>>(null);
        private static WeakReference<List<Tuple<TransitAgency, string[]>>> SavedAgencyCache = new WeakReference<List<Tuple<TransitAgency, string[]>>>(null);
        private static List<List<string>> PendingDownloadsCache = new List<List<string>>();
        #endregion

        #region Private/Internal Methods
        public static async Task EnsureFolders()
        {
            if (!(await ApplicationData.Current.LocalCacheFolder.GetFoldersAsync()).Any(folder => folder.Name == "SavedSchedules"))
            {
                await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("SavedSchedules");
            }
            for (int i = 0; i < 17; i++)
            {
                if (!(await ApplicationData.Current.LocalCacheFolder.GetFilesAsync()).Any(file => file.Name == "StopCache" + i.ToString() + ".txt"))
                {
                    await ApplicationData.Current.LocalCacheFolder.CreateFileAsync("StopCache" + i.ToString() + ".txt");
                }
            }
            if (!(await ApplicationData.Current.LocalCacheFolder.GetFilesAsync()).Any(file => file.Name == "RouteCache.txt"))
            {
                await ApplicationData.Current.LocalCacheFolder.CreateFileAsync("RouteCache.txt");
            }
            if (!(await ApplicationData.Current.LocalCacheFolder.GetFilesAsync()).Any(file => file.Name == "AgencyCache.txt"))
            {
                await ApplicationData.Current.LocalCacheFolder.CreateFileAsync("AgencyCache.txt");
            }
            if (!(await ApplicationData.Current.LocalCacheFolder.GetFilesAsync()).Any(file => file.Name == "PendingDownloadsCache.txt"))
            {
                await ApplicationData.Current.LocalCacheFolder.CreateFileAsync("PendingDownloadsCache.txt");
            }
        }

        internal static async Task AccessStopCache(int hash, Func<List<BusStop>, bool> action)
        {
            List<BusStop> stops;
            var file = await ApplicationData.Current.LocalCacheFolder.GetFileAsync("StopCache" + hash.ToString() + ".txt");
            if (!SavedStopCache[hash].TryGetTarget(out stops))
            {
                string encodedText = await FileIO.ReadTextAsync(file);
                CompactFormatReader reader = new CompactFormatReader(encodedText);
                stops = DeformatStops(reader);
            }
            if (action(stops))
            {
                CompactFormatWriter writer = new CompactFormatWriter();
                FormatStops(stops, writer);
                await FileIO.WriteTextAsync(file, writer.ToString());
                SavedStopCache[hash].SetTarget(stops);
            }
        }

        internal static async Task AccessRouteCache(Func<List<Tuple<BusRoute, string[], string[]>>, bool> action)
        {
            List<Tuple<BusRoute, string[], string[]>> routes;
            var file = await ApplicationData.Current.LocalCacheFolder.GetFileAsync("RouteCache.txt");
            if (!SavedRouteCache.TryGetTarget(out routes))
            {
                string encodedText = await FileIO.ReadTextAsync(file);
                CompactFormatReader reader = new CompactFormatReader(encodedText);
                routes = DeformatRoutes(reader);
            }
            if (action(routes))
            {
                CompactFormatWriter writer = new CompactFormatWriter();
                FormatRoutes(routes, writer);
                await FileIO.WriteTextAsync(file, writer.ToString());
                SavedRouteCache.SetTarget(routes);
            }
        }

        internal static async Task AccessAgencyCache(Func<List<Tuple<TransitAgency, string[]>>, bool> action)
        {
            List<Tuple<TransitAgency, string[]>> agencies;
            var file = await ApplicationData.Current.LocalCacheFolder.GetFileAsync("AgencyCache.txt");
            if (!SavedAgencyCache.TryGetTarget(out agencies))
            {
                string encodedText = await FileIO.ReadTextAsync(file);
                CompactFormatReader reader = new CompactFormatReader(encodedText);
                agencies = DeformatAgencies(reader);
            }
            if (action(agencies))
            {
                CompactFormatWriter writer = new CompactFormatWriter();
                FormatAgencies(agencies, writer);
                await FileIO.WriteTextAsync(file, writer.ToString());
                SavedAgencyCache.SetTarget(agencies);
            }
        }
        #endregion

        #region Public Methods
        public static async Task SaveScheduleAsync(WeekSchedule schedule, BusStop stop)
        {
            WeekSchedule baseSchedule = (await LoadSchedule(stop.ID)) ?? new WeekSchedule();
            baseSchedule.RemoveRoutes(schedule.Routes);
            baseSchedule.MergeByRoute(schedule);
            await OverwriteScheduleAsync(baseSchedule, stop);
        }

        public static async Task<WeekSchedule> LoadSchedule(string stopId)
        {
            WeekSchedule result = new WeekSchedule();
            var file = (await (await ApplicationData.Current.LocalCacheFolder.GetFolderAsync("SavedSchedules")).GetFilesAsync()).FirstOrDefault(item => item.Name == stopId + ".txt");
            if (file != null)
            {
                string text = await FileIO.ReadTextAsync(file);
                CompactFormatReader reader = new CompactFormatReader(text);
                result.Deformat(reader);
                return result;
            }
            else
                return null;
        }

        private static async Task DeleteSchedule(BusStop stop, StorageFile file, params string[] routes)
        {
            if (routes == null || routes.Length == 0)
            {
                await file.DeleteAsync();
            }
            else
            {
                WeekSchedule baseSchedule = (await LoadSchedule(stop.ID)) ?? new WeekSchedule();
                baseSchedule.RemoveRoutes(routes);
                if (baseSchedule.IsEmpty)
                    await DeleteSchedule(stop, file);
                else
                    await OverwriteScheduleAsync(baseSchedule, stop);
            }
        }

        public static async Task DeleteSchedules(BusStop[] stops, params string[] routes)
        {
            Dictionary<string, StorageFile> files = new Dictionary<string, StorageFile>();
            var allFiles = await (await ApplicationData.Current.LocalCacheFolder.GetFolderAsync("SavedSchedules")).GetFilesAsync();
            foreach (var file in allFiles)
            {
                if (stops.Any(stop => stop.Name + ".txt" == file.Name))
                {
                    var curStop = stops.First(stop => stop.Name + ".txt" == file.Name);
                    files.Add(curStop.ID, file);
                }
            }
            foreach (var stop in stops)
            {
                if (files.Any(kvp => kvp.Key == stop.ID))
                {
                    var file = files.First(kvp => kvp.Key == stop.ID).Value;
                    await DeleteSchedule(stop, file, routes);
                }
            }
        }

        public static async Task OverwriteScheduleAsync(WeekSchedule schedule, BusStop stop)
        {
            int hash = HashLocation(stop.Position);
            var file = await (await ApplicationData.Current.LocalCacheFolder.GetFolderAsync("SavedSchedules")).CreateFileAsync(stop.ID.ToString() + ".txt", CreationCollisionOption.ReplaceExisting);
            CompactFormatWriter encoder = new CompactFormatWriter();
            schedule.Format(encoder);
            encoder.TrimDelimiter();
            await FileIO.WriteTextAsync(file, encoder.ToString());
            await AccessStopCache(hash, delegate (List<BusStop> cache)
            {
                if (!cache.Any(item => item == stop))
                {
                    cache.Add(stop);
                    return true;
                }
                return false;
            }
                );
        }

        public static async Task SaveRoute(BusRoute route, string[] stops, string[] shapes)
        {
            await AccessRouteCache(delegate (List<Tuple<BusRoute, string[], string[]>> routes)
            {
                if (!routes.Any(item => item.Item1 == route))
                {
                    routes.Add(new Tuple<BusRoute, string[], string[]>(route, stops, shapes));
                    return true;
                }
                return false;
            });
        }

        public static async Task SaveAgency(TransitAgency agency, string[] routes)
        {
            await AccessAgencyCache(delegate (List<Tuple<TransitAgency, string[]>> agencies)
            {
                if (!agencies.Any(item => item.Item1 == agency))
                {
                    agencies.Add(new Tuple<TransitAgency, string[]>(agency, routes));
                    return true;
                }
                return false;
            });
        }

        public static async Task<bool> IsRouteCached(BusRoute route)
        {
            bool result = false;
            await AccessRouteCache(delegate (List<Tuple<BusRoute, string[], string[]>> routeData)
            {
                result = routeData.Any(item => item.Item1 == route);
                return false;
            });
            return result;
        }

        public static int HashLocation(double latitude, double longitude)
        {
            if (DowntownSeattleZone.ContainsPoint(new BasicGeoposition() { Latitude = latitude, Longitude = longitude }))
                return 16;
            int latHash = (int)(Abs(latitude) / 0.1) % 4;
            int lonHash = (int)(Abs(longitude) / 0.2) % 4;
            return latHash + 4 * lonHash;
        }

        public static int HashLocation(BasicGeoposition location)
        {
            return HashLocation(location.Latitude, location.Longitude);
        }

        public static async Task LoadPendingDownloads()
        {
            var file = await ApplicationData.Current.LocalCacheFolder.GetFileAsync("PendingDownloadsCache.txt");
            string encodedText = await FileIO.ReadTextAsync(file);
            CompactFormatReader reader = new CompactFormatReader(encodedText);
            PendingDownloadsCache.Clear();
            CompactFormatReader[] routeReader;
            while ((routeReader = reader.Next()) != null)
            {
                List<string> item = new List<string>();
                foreach (var subReader in routeReader)
                {
                    item.Add(subReader.ReadString());
                }
                PendingDownloadsCache.Add(item);
            }
        }

        public static async Task SavePendingDownloads()
        {
            CompactFormatWriter writer = new CompactFormatWriter();
            foreach (var route in PendingDownloadsCache)
            {
                foreach (var item in route)
                {
                    writer.WriteString(item);
                }
                writer.NextItem();
            }
            var file = await ApplicationData.Current.LocalCacheFolder.GetFileAsync("PendingDownloadsCache.txt");
            await FileIO.WriteTextAsync(file, writer.ToString());
        }

        public static List<List<string>> PendingDownloads
        {
            get { return PendingDownloadsCache; }
        }

        public static async Task<string[]> GetCachedStopIdsForRoute(BusRoute route)
        {
            string[] result = null;
            await AccessRouteCache(delegate (List<Tuple<BusRoute, string[], string[]>> rte)
                {
                    var curRoute = rte.FirstOrDefault(item => item.Item1 == route);
                    if (curRoute != null)
                    {
                        result = curRoute.Item2;
                    }
                    return false;
                });
            return result ?? new string[0];
        }

        public static async Task<Tuple<BusStop[], string[]>> GetStopsAndShapesForRouteFromCache(string id)
        {
            //Tuple<BusStop[], string[]> result = null;
            Tuple<BusRoute, string[], string[]> route = null;
            await AccessRouteCache(delegate (List<Tuple<BusRoute, string[], string[]>> rtes)
            {
                route = rtes.FirstOrDefault(rte => rte.Item1.ID == id);
                return false;
            });
            if (route != null)
            {
                List<BusStop> stops = new List<BusStop>();
                foreach (var stp in route.Item2)
                {
                    BusStop? stop;
                    if ((stop = await Data.GetBusStop(stp, new CancellationToken())) != null)
                        stops.Add(stop.Value);
                }
                return new Tuple<BusStop[], string[]>(stops.ToArray(), route.Item3);
            }
            return null;
        }

        public static async Task<BusStop?> GetStopFromCache(string id)
        {
            BusStop? result = null;
            for (int i = 0; i < 17; i++)
            {
                await AccessStopCache(i, delegate (List<BusStop> stops)
                {
                    if (stops.Any(route => route.ID == id))
                    {
                        result = stops.First(route => route.ID == id);
                    }
                    return false;
                });
            }
            return result;
        }

        public static async Task<BusRoute?> GetRouteFromCache(string id)
        {
            BusRoute? result = null;
            await AccessRouteCache(delegate (List<Tuple<BusRoute, string[], string[]>> routes)
            {
                if (routes.Any(route => route.Item1.ID == id))
                {
                    result = routes.First(route => route.Item1.ID == id).Item1;
                }
                return false;
            });
            return result;
        }

        public static async Task<TransitAgency[]> GetAgenciesFromCache()
        {
            TransitAgency[] result = null;
            await AccessAgencyCache(delegate (List<Tuple<TransitAgency, string[]>> agencies)
            {
                if (agencies.Count > 0)
                    result = (from agency in agencies select agency.Item1).ToArray();
                return false;
            });
            return result;
        }

        public static async Task<BusRoute[]> GetBusRoutesForAgencyFromCache(string agencyId)
        {
            string[] routeIds = null;
            await AccessAgencyCache(delegate (List<Tuple<TransitAgency, string[]>> agencies)
            {
                routeIds = agencies.FirstOrDefault(agency => agency.Item1.ID == agencyId)?.Item2;
                return false;
            });
            if (routeIds != null)
            {
                List<BusRoute> result = new List<BusRoute>();
                BusRoute? route;
                foreach (var str in routeIds)
                {
                    if ((route = await Data.GetRoute(str, new CancellationToken())) != null)
                        result.Add(route.Value);
                }
                return result.ToArray();
            }
            return null;
        }

        public static async Task<TransitAgency?> GetAgencyFromCache(string id)
        {
            TransitAgency? result = null;
            await AccessAgencyCache(delegate (List<Tuple<TransitAgency, string[]>> agencies)
            {
                if (agencies.Any(route => route.Item1.ID == id))
                {
                    result = agencies.First(route => route.Item1.ID == id).Item1;
                }
                return false;
            });
            return result;
        }

        public static async Task RemoveRoutesFromCache(params string[] routeIds)
        {
            await AccessRouteCache(delegate (List<Tuple<BusRoute, string[], string[]>> rte)
            {
                rte.RemoveAll(itm => routeIds.Contains(itm.Item1.ID));
                return true;
            });
        }

        public static async Task<BusStop[]> GetStopsForArea(BasicGeoposition topLeft, BasicGeoposition bottomRight, CancellationToken cancellationToken)
        {
            List<int> hashes = new List<int>();
            GeoboundingBox box = new GeoboundingBox(topLeft, bottomRight);
            Rect intersection = box.ToRect();
            intersection.Intersect(DowntownSeattleZone.ToRect());
            if (intersection.Width > 0 && intersection.Height > 0)
                hashes.Add(16);
            if (intersection != box.ToRect())
            {
                double minLat = bottomRight.Latitude;
                double minLon = topLeft.Longitude;
                double maxLat = topLeft.Latitude;
                double maxLon = bottomRight.Longitude;
                for (double lat = minLat; lat < maxLat + 0.1; lat += 0.1)
                {
                    if (lat > maxLat)
                        lat = maxLat;
                    for (double lon = minLon; lon < maxLon + 0.2; lon += 0.2)
                    {
                        if (lon > maxLon)
                            lon = maxLon;
                        int hash = HashLocation(lat, lon);
                        if (!hashes.Contains(hash))
                            hashes.Add(hash);
                    }
                }
            }
            cancellationToken.ThrowIfCancellationRequested();

            List<BusStop> result = new List<BusStop>();

            foreach (var hash in hashes)
            {
                await AccessStopCache(hash, delegate (List<BusStop> stops)
                {
                    foreach (var stop in stops)
                    {
                        if (box.ContainsPoint(stop.Position) && !result.Contains(stop))
                            result.Add(stop);
                    }
                    return false;
                });
                cancellationToken.ThrowIfCancellationRequested();
            }

            return result.ToArray();
        }

        public static async Task<RealtimeArrival[]> GetScheduledArrivals(string stopId)
        {
            DateTime minTime = DateTime.Now - TimeSpan.FromMinutes(5);
            DateTime maxTime = DateTime.Now + TimeSpan.FromMinutes(45);
            ServiceDay day = DateTime.Now.GetServiceDay();
            var weekSched = await LoadSchedule(stopId);
            if (weekSched == null)
                return null;
            var daySched = weekSched[day];
            SortedSet<RealtimeArrival> result = new SortedSet<RealtimeArrival>(Comparer<RealtimeArrival>.Create((sa1, sa2) => sa1.ScheduledArrivalTime < sa2.ScheduledArrivalTime ? -1 : sa1.ScheduledArrivalTime > sa2.ScheduledArrivalTime ? 1 : 0));
            string curRouteId = null;
            string curRouteName = "";
            if (daySched == null || daySched.IsEmpty)
                return result.ToArray();
            foreach (var item in daySched)
            {
                if (curRouteId != item.Route)
                {
                    curRouteId = item.Route;
                    await AccessRouteCache(delegate (List<Tuple<BusRoute, string[], string[]>> routes)
                    {
                        curRouteName = routes.First(rte => rte.Item1.ID == item.Route).Item1.Name;
                        return false;
                    });
                }
                if (item.ScheduledDepartureTime >= minTime && item.ScheduledDepartureTime < maxTime)
                    result.Add(new RealtimeArrival() { Route = item.Route, RouteName = curRouteName, Destination = item.Destination, LastUpdateTime = DateTime.Now, PredictedArrivalTime = null, ScheduledArrivalTime = item.ScheduledDepartureTime, Stop = item.Stop, Trip = item.Trip });
            }
            return result.ToArray();
        }
        #endregion
    }
}
