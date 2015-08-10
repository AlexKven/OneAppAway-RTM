using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Devices.Geolocation;

namespace OneAppAway
{
    public static class ApiLayer
    {
        private static BusStop ParseBusStop(XElement element)
        {
            string lat = element.Element("lat")?.Value;
            string lon = element.Element("lon")?.Value;
            string direction = element.Element("direction")?.Value;
            string name = element.Element("name")?.Value;
            string code = element.Element("code")?.Value;
            string id = element.Element("id")?.Value;
            string locationType = element.Element("locationType")?.Value;
            List<string> routeIds = new List<string>();
            foreach (XElement el2 in element.Element("routeIds").Elements("string"))
            {
                routeIds.Add(el2?.Value);
            }
            return new BusStop() { Position = new BasicGeoposition() { Latitude = double.Parse(lat), Longitude = double.Parse(lon) }, Direction = direction == null ? StopDirection.Unspecified : (StopDirection)Enum.Parse(typeof(StopDirection), direction), Name = name, ID = id, Code = code, LocationType = int.Parse(locationType), Routes = routeIds.ToArray() };
        }

        public static async Task<string> SendRequest(string compactRequest, Dictionary<string, string> parameters, CancellationToken cancellationToken)
        {
            HttpClient client = new HttpClient();
            string request = "http://api.pugetsound.onebusaway.org/api/where/" + compactRequest + ".xml?key=" + Keys.ObaKey + parameters?.Aggregate("", (acc, item) => acc + "&" + item.Key + "=" + item.Value) ?? "";
            var resp = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, request), cancellationToken);
            if (cancellationToken.IsCancellationRequested) return null;
            return await resp.Content.ReadAsStringAsync();
        }

        public static async Task<BusStop[]> GetBusStops(BasicGeoposition center, double latSpan, double lonSpan, CancellationToken cancellationToken)
        {
            List<BusStop> result = new List<BusStop>();
            var responseString = await SendRequest("stops-for-location", new Dictionary<string, string>() { ["lat"] = center.Latitude.ToString(), ["lon"] = center.Longitude.ToString(), ["latSpan"] = latSpan.ToString(), ["lonSpan"] = lonSpan.ToString() }, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                return null;

            StringReader reader = new StringReader(responseString);
            XDocument xDoc = XDocument.Load(reader);

            //XElement el = (XElement)xDoc.Nodes().First(d => d.NodeType == XmlNodeType.Element && ((XElement)d).Name.LocalName == "response");
            XElement el = xDoc.Element("response").Element("data");
            XElement elList = el.Element("list");

            foreach (XElement el1 in elList.Elements("stop"))
            {
                result.Add(ParseBusStop(el1));
            }

            XElement elRoutes = el.Element("references").Element("routes");
            return result.ToArray();
        }

        public static async Task<RealtimeArrival[]> GetBusArrivals(string id, CancellationToken cancellationToken)
        {
            List<RealtimeArrival> result = new List<RealtimeArrival>();
            StringReader reader = new StringReader(await SendRequest("arrivals-and-departures-for-stop/" + id, null, cancellationToken));
            if (cancellationToken.IsCancellationRequested)
                return null;

            XDocument xDoc = XDocument.Load(reader);

            XElement el = xDoc.Element("response");
            el = el.Element("data");
            el = el.Element("entry");
            el = el.Element("arrivalsAndDepartures");

            foreach (XElement el1 in el.Elements("arrivalAndDeparture"))
            {
                string routeName = el1.Element("routeShortName") == null ? el1.Element("routeLongName").Value : el1.Element("routeShortName").Value;
                string routeId = el1.Element("routeId")?.Value;
                string tripId = el1.Element("tripId")?.Value;
                string stopId = el1.Element("stopId")?.Value;
                string predictedArrivalTime = el1.Element("predictedArrivalTime")?.Value;
                string scheduledArrivalTime = el1.Element("scheduledArrivalTime")?.Value;
                string lastUpdateTime = el1.Element("lastUpdateTime")?.Value;
                string destination = el1.Element("tripHeadsign")?.Value;
                long predictedArrivalLong = long.Parse(predictedArrivalTime);
                long scheduledArrivalLong = long.Parse(scheduledArrivalTime);
                long? lastUpdateLong = lastUpdateTime == null ? null : new long?(long.Parse(scheduledArrivalTime));
                DateTime? predictedArrival = predictedArrivalLong == 0 ? null : new DateTime?((new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromMilliseconds(predictedArrivalLong)).ToLocalTime());
                DateTime scheduledArrival = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromMilliseconds(scheduledArrivalLong)).ToLocalTime();
                DateTime? lastUpdate = lastUpdateLong == null ? null : new DateTime?((new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromMilliseconds(lastUpdateLong.Value)).ToLocalTime());
                RealtimeArrival arrival = new RealtimeArrival() { RouteName = routeName, PredictedArrivalTime = predictedArrival, ScheduledArrivalTime = scheduledArrival, LastUpdateTime = lastUpdate, Route = routeId, Trip = tripId, Stop = stopId, Destination = destination };
                result.Add(arrival);
            }

            return result.ToArray();
        }

        public static async Task<BusStop> GetBusStop(string id, CancellationToken cancellationToken)
        {
            StringReader reader = new StringReader(await SendRequest("stop/" + id, null, cancellationToken));
            if (cancellationToken.IsCancellationRequested)
                return new BusStop();

            XDocument xDoc = XDocument.Load(reader);

            XElement el = xDoc.Element("response").Element("data").Element("entry");

            string lat = el.Element("lat")?.Value;
            string lon = el.Element("lon")?.Value;
            string direction = el.Element("direction")?.Value;
            string name = el.Element("name")?.Value;
            string code = el.Element("code")?.Value;
            string locationType = el.Element("locationType")?.Value;
            List<string> routeIds = new List<string>();
            foreach (XElement el2 in el.Element("routeIds").Elements("string"))
            {
                routeIds.Add(el2?.Value);
            }
            return new BusStop() { Position = new BasicGeoposition() { Latitude = double.Parse(lat), Longitude = double.Parse(lon) }, Direction = direction == null ? StopDirection.Unspecified : (StopDirection)Enum.Parse(typeof(StopDirection), direction), Name = name, ID = id, Code = code, LocationType = int.Parse(locationType), Routes = routeIds.ToArray() };
        }

        public static async Task<BusRoute> GetBusRoute(string id, CancellationToken cancellationToken)
        {
            StringReader reader = new StringReader(await SendRequest("route/" + id, null, cancellationToken));
            if (cancellationToken.IsCancellationRequested)
                return new BusRoute();

            XDocument xDoc = XDocument.Load(reader);

            XElement el = xDoc.Element("response").Element("data").Element("entry");
            string routeId = el.Element("id")?.Value;
            var elDescription = el.Element("description");
            var elShortName = el.Element("shortName");
            var elLongName = el.Element("longName");
            string routeName = elShortName == null ? elLongName == null ? "(No Name)" : elLongName.Value : elShortName.Value;
            string routeDescription = elDescription == null ? elLongName == null ? elShortName == null ? "No Description" : elShortName.Value : elLongName.Value : elDescription.Value;
            string routeAgency = el.Element("agencyId")?.Value;
            return new BusRoute() { ID = routeId, Name = routeName, Description = routeDescription, Agency = routeAgency };
        }

        public static async Task<BusRoute[]> GetBusRoutes(string agencyId, CancellationToken cancellationToken)
        {
            List<BusRoute> result = new List<BusRoute>();
            StringReader reader = new StringReader(await SendRequest("routes-for-agency/" + agencyId, null, cancellationToken));
            if (cancellationToken.IsCancellationRequested)
                return null;

            XDocument xDoc = XDocument.Load(reader);

            foreach (var el in xDoc.Element("response").Element("data").Element("list").Elements("route"))
            {
                string routeId = el.Element("id")?.Value;
                var elDescription = el.Element("description");
                var elShortName = el.Element("shortName");
                var elLongName = el.Element("longName");
                string routeName = elShortName == null ? elLongName == null ? "(No Name)" : elLongName.Value : elShortName.Value;
                string routeDescription = elDescription == null ? elLongName == null ? elShortName == null ? "No Description" : elShortName.Value : elLongName.Value : elDescription.Value;
                string routeAgency = el.Element("agencyId")?.Value;
                result.Add(new BusRoute() { ID = routeId, Name = routeName, Description = routeDescription, Agency = routeAgency });
            }
            return result.ToArray();
        }

        public static async Task<TransitAgency> GetTransitAgency(string id, CancellationToken cancellationToken)
        {
            StringReader reader = new StringReader(await SendRequest("agency/" + id, null, cancellationToken));
            if (cancellationToken.IsCancellationRequested)
                return new TransitAgency();

            XDocument xDoc = XDocument.Load(reader);

            XElement el = xDoc.Element("response").Element("data").Element("entry");
            string agencyName = el.Element("name")?.Value;
            string agencyUrl = el.Element("url")?.Value;
            return new TransitAgency() { ID = id, Name = agencyName, Url = agencyUrl};
        }

        public static async Task<TransitAgency[]> GetTransitAgencies(CancellationToken cancellationToken)
        {
            List<TransitAgency> result = new List<TransitAgency>();
            StringReader reader = new StringReader(await SendRequest("agencies-with-coverage", null, cancellationToken));
            if (cancellationToken.IsCancellationRequested)
                return null;

            XDocument xDoc = XDocument.Load(reader);
            
            foreach (var el in xDoc.Element("response").Element("data").Element("references").Element("agencies").Elements("agency"))
            {
                string agencyId = el.Element("id")?.Value;
                string agencyName = el.Element("name")?.Value;
                string agencyUrl = el.Element("url")?.Value;
                result.Add(new TransitAgency() { ID = agencyId, Name = agencyName, Url = agencyUrl });
            }

            return result.ToArray();
        }

        public static async Task<Tuple<BusStop[], string[]>> GetStopsForRoute(string route, CancellationToken cancellationToken)
        {
            List<BusStop> result = new List<BusStop>();
            List<string> shapeResult = new List<string>();

            string responseString = await SendRequest("stops-for-route/" + route, null, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                return null;

            StringReader reader = new StringReader(responseString);
            XDocument xDoc = XDocument.Load(reader);
            foreach (XElement el in xDoc.Element("response")?.Element("data")?.Element("references")?.Element("stops")?.Elements("stop"))
            {
                result.Add(ParseBusStop(el));
            }

            foreach (XElement el in xDoc.Element("response")?.Element("data").Element("entry").Element("polylines").Elements("encodedPolyline"))
            {
                shapeResult.Add(el.Element("points").Value);
            }

            return new Tuple<BusStop[], string[]>(result.ToArray(), shapeResult.ToArray());
        }

        public static async Task<string> GetShape(string id, CancellationToken cancellationToken)
        {
            StringReader reader = new StringReader(await SendRequest("shape/" + id, null, cancellationToken));
            if (cancellationToken.IsCancellationRequested)
                return null;

            XDocument xDoc = XDocument.Load(reader);

            XElement el = xDoc.Element("response").Element("data").Element("entry");
            return el.Element("points")?.Value;
        }
    }
}
