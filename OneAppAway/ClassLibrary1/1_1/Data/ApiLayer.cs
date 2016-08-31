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

namespace OneAppAway._1_1.Data
{
    public static class ApiLayer
    {
        private static TransitStop ParseTransitStop(XElement element)
        {
            string lat = element.Element("lat")?.Value;
            string lon = element.Element("lon")?.Value;
            string direction = element.Element("direction")?.Value;
            string name = element.Element("name")?.Value;
            string code = element.Element("code")?.Value;
            string id = element.Element("id")?.Value;
            //string locationType = element.Element("locationType")?.Value;
            List<string> routeIds = new List<string>();
            foreach (XElement el2 in element.Element("routeIds").Elements("string"))
            {
                routeIds.Add(el2?.Value);
            }
            return new TransitStop() { Position = new LatLon(double.Parse(lat), double.Parse(lon)), Direction = direction == null ? StopDirection.Unspecified : (StopDirection)Enum.Parse(typeof(StopDirection), direction), Name = name, ID = id, Code = code, Routes = routeIds.ToArray() };
        }

        private static TransitRoute ParseTransitRoute(XElement element)
        {
            string routeId = element.Element("id")?.Value;
            var elDescription = element.Element("description");
            var elShortName = element.Element("shortName");
            var elLongName = element.Element("longName");
            string routeName = elShortName == null ? elLongName == null ? "(No Name)" : elLongName.Value : elShortName.Value;
            string routeDescription = elDescription == null ? elLongName == null ? elShortName == null ? "No Description" : elShortName.Value : elLongName.Value : elDescription.Value;
            string routeAgency = element.Element("agencyId")?.Value;
            return new TransitRoute() { ID = routeId, Name = routeName, Description = routeDescription, Agency = routeAgency };
        }

        private static RealTimeArrival ParseRealTimeArrival(XElement element)
        {
            string routeName = element.Element("routeShortName")?.Value ?? element.Element("routeLongName")?.Value;
            string routeId = element.Element("routeId")?.Value;
            string tripId = element.Element("tripId")?.Value;
            string stopId = element.Element("stopId")?.Value;
            string predictedArrivalTime = element.Element("predictedArrivalTime")?.Value;
            string scheduledArrivalTime = element.Element("scheduledArrivalTime")?.Value;
            string lastUpdateTime = element.Element("lastUpdateTime")?.Value;
            string destination = element.Element("tripHeadsign")?.Value;
            long predictedArrivalLong = long.Parse(predictedArrivalTime);
            long scheduledArrivalLong = long.Parse(scheduledArrivalTime);
            long? lastUpdateLong = lastUpdateTime == null ? null : new long?(long.Parse(scheduledArrivalTime));
            DateTime? predictedArrival = predictedArrivalLong == 0 ? null : new DateTime?((new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromMilliseconds(predictedArrivalLong)).ToLocalTime());
            DateTime scheduledArrival = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromMilliseconds(scheduledArrivalLong)).ToLocalTime();
            DateTime? lastUpdate = lastUpdateLong == null ? null : new DateTime?((new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromMilliseconds(lastUpdateLong.Value)).ToLocalTime());
            return new RealTimeArrival() { RouteName = routeName, PredictedArrivalTime = predictedArrival, ScheduledArrivalTime = scheduledArrival, LastUpdateTime = lastUpdate, Route = routeId, Trip = tripId, Stop = stopId, Destination = destination };
        }

        public static async Task<string> SendRequest(string compactRequest, Dictionary<string, string> parameters, bool includeReferences, CancellationToken cancellationToken)
        {
            string request = "http://api.pugetsound.onebusaway.org/api/where/" + compactRequest + ".xml?key=" + Keys.ObaKey + parameters?.Aggregate("", (acc, item) => acc + "&" + item.Key + "=" + item.Value) ?? "" + "includeReferences=" + (includeReferences ? "true" : "false");
            HttpResponseMessage resp;
            HttpClient client;
            using (client = new HttpClient())
            {
                resp = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, request), cancellationToken);
            }
            if (cancellationToken.IsCancellationRequested) return null;
            return await resp.Content.ReadAsStringAsync();
        }

        public static async Task<TransitStop[]> GetTransitStopsForArea(LatLonRect area, CancellationToken cancellationToken)
        {
            if (area.Span.Latitude > 1 || area.Span.Longitude > 1)
                return null;
            List<TransitStop> result = new List<TransitStop>();
            var responseString = await SendRequest("stops-for-location", new Dictionary<string, string>() { ["lat"] = area.Center.Latitude.ToString(), ["lon"] = area.Center.Longitude.ToString(), ["latSpan"] = area.Span.Latitude.ToString(), ["lonSpan"] = area.Span.Longitude.ToString() }, false, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                return null;

            StringReader reader = new StringReader(responseString);
            XDocument xDoc = XDocument.Load(reader);

            //XElement el = (XElement)xDoc.Nodes().First(d => d.NodeType == XmlNodeType.Element && ((XElement)d).Name.LocalName == "response");
            XElement el = xDoc.Element("response").Element("data");
            XElement elList = el.Element("list");

            foreach (XElement el1 in elList.Elements("stop"))
            {
                result.Add(ParseTransitStop(el1));
            }
            return result.ToArray();
        }

        public static async Task<TransitStop?> GetTransitStop(string id, CancellationToken cancellationToken)
        {
            StringReader reader = new StringReader(await SendRequest("stop/" + id, null, false, cancellationToken));
            if (cancellationToken.IsCancellationRequested)
                return new TransitStop();

            XDocument xDoc = XDocument.Load(reader);

            XElement el = xDoc.Element("response").Element("data").Element("entry");

            return ParseTransitStop(el);
        }

        public static async Task<TransitRoute?> GetTransitRoute(string id, CancellationToken cancellationToken)
        {

            StringReader reader = new StringReader(await SendRequest("route/" + id, null, false, cancellationToken));
            if (cancellationToken.IsCancellationRequested)
                return null;

            XDocument xDoc = XDocument.Load(reader);

            XElement el = xDoc.Element("response").Element("data").Element("entry");
            return ParseTransitRoute(el);
        }

        public static async Task<RealTimeArrival[]> GetBusArrivals(string id, CancellationToken cancellationToken)
        {
            List<RealTimeArrival> result = new List<RealTimeArrival>();
            StringReader reader = new StringReader(await SendRequest("arrivals-and-departures-for-stop/" + id, null, false, cancellationToken));
            if (cancellationToken.IsCancellationRequested)
                return null;

            XDocument xDoc = XDocument.Load(reader);

            XElement el = xDoc.Element("response");
            el = el?.Element("data");
            el = el?.Element("entry");
            el = el?.Element("arrivalsAndDepartures");

            if (el == null) return null;

            foreach (XElement el1 in el.Elements("arrivalAndDeparture"))
                result.Add(ParseRealTimeArrival(el1));

            return result.ToArray();
        }
    }
}
