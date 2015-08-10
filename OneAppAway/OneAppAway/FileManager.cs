using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.Storage.Search;

namespace OneAppAway
{
    public static class FileManager
    {
        private static WeakReference<List<BusStop>> SavedStopCache = new WeakReference<List<BusStop>>(null);
        private static WeakReference<List<Tuple<BusRoute, string[], string[]>>> SavedRouteCache = new WeakReference<List<Tuple<BusRoute, string[], string[]>>>(null);
        private static WeakReference<List<Tuple<TransitAgency, string[]>>> SavedAgencyCache = new WeakReference<List<Tuple<TransitAgency, string[]>>>(null);
        private static int count = 0;

        private static async Task EnsureFolders()
        {
            if (!(await ApplicationData.Current.LocalCacheFolder.GetFoldersAsync()).Any(folder => folder.Name == "SavedSchedules"))
            {
                await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("SavedSchedules");
            }
            if (!(await ApplicationData.Current.LocalCacheFolder.GetFilesAsync()).Any(file => file.Name == "StopCache.txt"))
            {
                await ApplicationData.Current.LocalCacheFolder.CreateFileAsync("StopCache.txt");
            }
            if (!(await ApplicationData.Current.LocalCacheFolder.GetFilesAsync()).Any(file => file.Name == "RouteCache.txt"))
            {
                await ApplicationData.Current.LocalCacheFolder.CreateFileAsync("RouteCache.txt");
            }
            if (!(await ApplicationData.Current.LocalCacheFolder.GetFilesAsync()).Any(file => file.Name == "AgencyCache.txt"))
            {
                await ApplicationData.Current.LocalCacheFolder.CreateFileAsync("AgencyCache.txt");
            }
        }

        private static async Task ModifyStopCache(Action<List<BusStop>> action)
        {
            List<BusStop> stops;
            var file = await ApplicationData.Current.LocalCacheFolder.GetFileAsync("StopCache.txt");
            if (!SavedStopCache.TryGetTarget(out stops))
            {
                string encodedText = await FileIO.ReadTextAsync(file);
                CompactFormatReader reader = new CompactFormatReader(encodedText);
                stops = DeformatStops(reader);
            }
            int oldCount = count;
            action(stops); 
            count = stops.Count;
            CompactFormatWriter writer = new CompactFormatWriter();
            FormatStops(stops, writer);
            await FileIO.WriteTextAsync(file, writer.ToString());
            SavedStopCache.SetTarget(stops);
        }

        private static async Task ModifyRouteCache(Action<List<Tuple<BusRoute, string[], string[]>>> action)
        {
            List<Tuple<BusRoute, string[], string[]>> routes;
            var file = await ApplicationData.Current.LocalCacheFolder.GetFileAsync("RouteCache.txt");
            if (!SavedRouteCache.TryGetTarget(out routes))
            {
                string encodedText = await FileIO.ReadTextAsync(file);
                CompactFormatReader reader = new CompactFormatReader(encodedText);
                routes = DeformatRoutes(reader);
            }
            action(routes);
            CompactFormatWriter writer = new CompactFormatWriter();
            FormatRoutes(routes, writer);
            await FileIO.WriteTextAsync(file, writer.ToString());
            SavedRouteCache.SetTarget(routes);
        }

        private static async Task ModifyAgencyCache(Action<List<Tuple<TransitAgency, string[]>>> action)
        {
            List<Tuple<TransitAgency, string[]>> agencies;
            var file = await ApplicationData.Current.LocalCacheFolder.GetFileAsync("AgencyCache.txt");
            if (!SavedAgencyCache.TryGetTarget(out agencies))
            {
                string encodedText = await FileIO.ReadTextAsync(file);
                CompactFormatReader reader = new CompactFormatReader(encodedText);
                agencies = DeformatAgencies(reader);
            }
            action(agencies);
            CompactFormatWriter writer = new CompactFormatWriter();
            FormatAgencies(agencies, writer);
            await FileIO.WriteTextAsync(file, writer.ToString());
            SavedAgencyCache.SetTarget(agencies);
        }

        public static async Task SaveScheduleAsync(WeekSchedule schedule, BusStop stop)
        {
            await EnsureFolders();
            var file = await (await ApplicationData.Current.LocalCacheFolder.GetFolderAsync("SavedSchedules")).CreateFileAsync(stop.ID.ToString() + ".txt", CreationCollisionOption.ReplaceExisting);
            CompactFormatWriter encoder = new CompactFormatWriter();
            schedule.Format(encoder);
            encoder.TrimDelimiter();
            await FileIO.WriteTextAsync(file, encoder.ToString());
            await ModifyStopCache(delegate (List<BusStop> cache)
            {
                    if (!cache.Any(item => item == stop))
                        cache.Add(stop);
            }
                );
        }

        public static async Task SaveRoute(BusRoute route, string[] stops, string[] shapes)
        {
            await EnsureFolders();
            await ModifyRouteCache(delegate (List<Tuple<BusRoute, string[], string[]>> routes)
            {
                routes.RemoveAll(item => item.Item1 == route);
                routes.Add(new Tuple<BusRoute, string[], string[]>(route, stops, shapes));
            });
        }

        public static async Task SaveAgency(TransitAgency agency, string[] routes)
        {
            await EnsureFolders();
            await ModifyAgencyCache(delegate (List<Tuple<TransitAgency, string[]>> agencies)
            {
                agencies.RemoveAll(item => item.Item1 == agency);
                agencies.Add(new Tuple<TransitAgency, string[]>(agency, routes));
            });
        }

        public static List<BusStop> DeformatStops(CompactFormatReader reader)
        {
            List<BusStop> result = new List<BusStop>();
            CompactFormatReader[] stopReader;
            List<string> routes;
            while ((stopReader = reader.Next()) != null)
            {
                BusStop stop = new BusStop();
                stop.ID = stopReader[0].ReadString();
                stop.Direction = (StopDirection)stopReader[1].ReadInt();
                stop.Position = new BasicGeoposition() { Latitude = double.Parse(stopReader[2].ReadString()), Longitude = double.Parse(stopReader[3].ReadString()) };
                stop.Name = stopReader[4].ReadString();
                stop.Code = stopReader[5].ReadString();
                stop.LocationType = stopReader[6].ReadInt();
                routes = new List<string>();
                CompactFormatReader[] routeReader;
                while ((routeReader = stopReader[7].Next()) != null)
                {
                    routes.Add(routeReader[0].ReadString());
                }
                stop.Routes = routes.ToArray();
                result.Add(stop);
            }
            return result;
        }

        public static void FormatStops(List<BusStop> stops, CompactFormatWriter writer)
        {
            for (int i = 0; i < stops.Count; i++)
            {
                var stop = stops[i];
                writer.WriteString(stop.ID);
                writer.WriteInt((int)stop.Direction);
                writer.WriteString(stop.Position.Latitude.ToString());
                writer.WriteString(stop.Position.Longitude.ToString());
                writer.WriteQuotedString(stop.Name);
                writer.WriteString(stop.Code);
                writer.WriteInt(stop.LocationType);
                writer.OpenParens();
                foreach (var route in stop.Routes)
                {
                    writer.WriteString(route);
                    writer.NextItem();
                }
                writer.CloseParens();
                writer.NextItem();
            }
        }

        public static void FormatRoutes(List<Tuple<BusRoute, string[], string[]>> routes, CompactFormatWriter writer)
        {
            while (routes.Count > 0)
            {
                var route = routes[0];
                writer.WriteString(route.Item1.ID);
                writer.WriteQuotedString(route.Item1.Name);
                writer.WriteString(route.Item1.ID);
                writer.WriteString(route.Item1.Agency);
                writer.OpenParens();
                foreach (var stop in route.Item2)
                {
                    writer.WriteString(stop);
                    writer.NextItem();
                }
                writer.CloseParens();
                writer.OpenParens();
                foreach (var shape in route.Item3)
                {
                    writer.WriteQuotedString(shape);
                    writer.NextItem();
                }
                writer.CloseParens();
                writer.NextItem();
                routes.RemoveAt(0);
            }
        }

        public static List<Tuple<BusRoute, string[], string[]>> DeformatRoutes(CompactFormatReader reader)
        {
            List<Tuple<BusRoute, string[], string[]>> result = new List<Tuple<BusRoute, string[], string[]>>();
            CompactFormatReader[] routeReader;
            List<string> stops;
            List<string> shapes;
            while ((routeReader = reader.Next()) != null)
            {
                BusRoute route = new BusRoute();
                route.ID = routeReader[0].ReadString();
                route.Name = routeReader[1].ReadString();
                route.Description = routeReader[2].ReadString();
                route.Agency = routeReader[3].ReadString();
                stops = new List<string>();
                shapes = new List<string>();
                CompactFormatReader[] subReader;
                while ((subReader = routeReader[4].Next()) != null)
                {
                    stops.Add(subReader[0].ReadString());
                }
                while ((subReader = routeReader[5].Next()) != null)
                {
                    shapes.Add(subReader[0].ReadString());
                }
                result.Add(new Tuple<BusRoute, string[], string[]>(route, stops.ToArray(), shapes.ToArray()));
            }
            return result;
        }

        public static void FormatAgencies(List<Tuple<TransitAgency, string[]>> agencies, CompactFormatWriter writer)
        {
            while (agencies.Count > 0)
            {
                var agency = agencies[0];
                writer.WriteString(agency.Item1.ID);
                writer.WriteQuotedString(agency.Item1.Name);
                writer.WriteQuotedString(agency.Item1.Url);
                writer.OpenParens();
                foreach (var route in agency.Item2)
                {
                    writer.WriteString(route);
                    writer.NextItem();
                }
                writer.CloseParens();
                writer.NextItem();
                agencies.RemoveAt(0);
            }
        }

        public static List<Tuple<TransitAgency, string[]>> DeformatAgencies(CompactFormatReader reader)
        {
            List<Tuple<TransitAgency, string[]>> result = new List<Tuple<TransitAgency, string[]>>();
            CompactFormatReader[] agencyReader;
            List<string> routes;
            while ((agencyReader = reader.Next()) != null)
            {
                TransitAgency agency = new TransitAgency();
                agency.ID = agencyReader[0].ReadString();
                agency.Name = agencyReader[1].ReadString();
                agency.Url = agencyReader[2].ReadString();
                routes = new List<string>();
                CompactFormatReader[] routeReader;
                while ((routeReader = agencyReader[3].Next()) != null)
                {
                    routes.Add(routeReader[0].ReadString());
                }
                result.Add(new Tuple<TransitAgency, string[]>(agency, routes.ToArray()));
            }
            return result;
        }
    }
}
