using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Math;

namespace OneAppAway._1_1.Data
{
    public class OneBusAwayDataSource : DataSource
    {
        #region Properties
        public override bool IsMemoryCache => false;
        public override bool IsOfflineSource => false;

        public override DataSourceFunctionType GetTransitRouteFunction => DataSourceFunctionType.Provision;
        public override DataSourceFunctionType GetTransitStopFunction => DataSourceFunctionType.Provision;
        public override DataSourceFunctionType GetRealTimeArrivalFunction => DataSourceFunctionType.Both;
        public override bool CanGetTransitStopsForArea => true;
        public override bool CanGetRealTimeArrivalsForStop => true;
        #endregion

        #region Not Implemented
        public override bool CanGetScheduleForStop => false;
        public override Task<RetrievedData<WeekSchedule>> GetScheduleForStop(string stopId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task<RetrievedData<TransitRoute>> CorrectTransitRoute(TransitRoute route, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        public override Task<RetrievedData<TransitStop>> CorrectTransitStop(TransitStop stop, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Base Class Implementations
        public override async Task<RetrievedData<TransitStop>> GetTransitStop(string id, CancellationToken cancellationToken)
        {
            DateTime time = DateTime.Now;
            try
            {
                StringReader reader = new StringReader(await SendRequest("stop/" + id, null, false, cancellationToken));
                if (cancellationToken.IsCancellationRequested)
                    return new Data.RetrievedData<Data.TransitStop>();

                XDocument xDoc = XDocument.Load(reader);

                XElement el = xDoc.Element("response").Element("data").Element("entry");

                return new RetrievedData<TransitStop>(ParseTransitStop(el));
            }
            catch (OperationCanceledException)
            {
                return new RetrievedData<TransitStop>();
            }
            catch (Exception ex)
            {
                return new RetrievedData<TransitStop>(ex);
            }
        }

        public override async Task<RetrievedData<TransitRoute>> GetTransitRoute(string id, CancellationToken cancellationToken)
        {
            try
            {
                StringReader reader = new StringReader(await SendRequest("route/" + id, null, false, cancellationToken));
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException();

                XDocument xDoc = XDocument.Load(reader);

                XElement el = xDoc.Element("response").Element("data").Element("entry");
                return new RetrievedData<TransitRoute>(ParseTransitRoute(el));
            }
            catch (OperationCanceledException)
            {
                return new RetrievedData<TransitRoute>();
            }
            catch (Exception ex)
            {
                return new RetrievedData<TransitRoute>(ex);
            }
        }

        public override async Task<RetrievedData<RealTimeArrival>> GetRealTimeArrival(string stopId, string tripId, CancellationToken cancellationToken)
        {
            try
            {
                StringReader reader = new StringReader(await SendRequest("arrival-and-departure-for-stop/" + stopId, new Dictionary<string, string>() { ["tripId"] = tripId }, true, cancellationToken));
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException();

                XDocument xDoc = XDocument.Load(reader);

                XElement el = xDoc.Element("response").Element("data").Element("entry").Element("arrivalAndDeparture");
                return new RetrievedData<RealTimeArrival>(ParseRealTimeArrival(el));
            }
            catch (OperationCanceledException)
            {
                return new RetrievedData<RealTimeArrival>();
            }
            catch (Exception ex)
            {
                return new RetrievedData<RealTimeArrival>(ex);
            }
        }

        public override Task<RetrievedData<RealTimeArrival>> CorrectRealTimeArrival(RealTimeArrival arrival, CancellationToken cancellationToken)
        {
            //Does nothing;
            return Task.FromResult(new RetrievedData<RealTimeArrival>(arrival));
        }

        public override async Task<RetrievedData<IEnumerable<TransitStop>>> GetTransitStopsForArea(LatLonRect area, CancellationToken cancellationToken)
        {
            try
            {
                if (area.Span.Latitude > 1 || area.Span.Longitude > 1)
                    return null;
                var responseString = await SendRequest("stops-for-location", new Dictionary<string, string>() { ["lat"] = area.Center.Latitude.ToString(), ["lon"] = area.Center.Longitude.ToString(), ["latSpan"] = area.Span.Latitude.ToString(), ["lonSpan"] = area.Span.Longitude.ToString() }, false, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException();

                StringReader reader = new StringReader(responseString);
                XDocument xDoc = XDocument.Load(reader);

                //XElement el = (XElement)xDoc.Nodes().First(d => d.NodeType == XmlNodeType.Element && ((XElement)d).Name.LocalName == "response");
                XElement el = xDoc.Element("response").Element("data");
                XElement elList = el.Element("list");

                return new RetrievedData<IEnumerable<TransitStop>>(ParseList(elList, "stop", ParseTransitStop));
            }
            catch (OperationCanceledException)
            {
                return new RetrievedData<IEnumerable<TransitStop>>();
            }
            catch (Exception ex)
            {
                return new RetrievedData<IEnumerable<TransitStop>>(ex);
            }
        }
        #endregion

        #region Static Parsing & HTTP Functions
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
            string freqSeconds = element?.Element("frequency")?.Element("headway")?.Value;
            string predictedArrivalTime = element.Element("predictedArrivalTime")?.Value;
            string scheduledArrivalTime = element.Element("scheduledArrivalTime")?.Value;
            string arrivalEnabled = element.Element("ArrivalEnabled")?.Value;
            string lastUpdateTime = null;
            string destination = element.Element("tripHeadsign")?.Value;
            long predictedArrivalLong = long.Parse(predictedArrivalTime);
            long scheduledArrivalLong = long.Parse(scheduledArrivalTime);
            long? lastUpdateLong;
            DateTime? predictedArrival = predictedArrivalLong == 0 ? null : new DateTime?((new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromMilliseconds(predictedArrivalLong)).ToLocalTime());
            DateTime scheduledArrival = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromMilliseconds(scheduledArrivalLong)).ToLocalTime();
            DateTime? lastUpdate;
            double frequency;
            if (double.TryParse(freqSeconds, out frequency))
            {
                frequency /= 60;
            }
            else
                frequency = -1;
            if ((element = element.Element("tripStatus")) != null)
            {
                if (element.Element("predicted")?.Value == "true")
                    lastUpdateTime = element.Element("lastUpdateTime")?.Value;
            }
            lastUpdateLong = lastUpdateTime == null ? null : new long?(long.Parse(scheduledArrivalTime));
            lastUpdate = lastUpdateLong == null ? null : new DateTime?((new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromMilliseconds(lastUpdateLong.Value)).ToLocalTime());
            double degreeOfConfidence = double.NaN;
            if (lastUpdate.HasValue && predictedArrival.HasValue)
            {
                if (lastUpdate.Value <= predictedArrival.Value)
                {
                    double minutes = (predictedArrival.Value - lastUpdate.Value).TotalMinutes;
                    degreeOfConfidence = 1.0 / (Pow(minutes + 1, 1.5) / 60 + 1);
                }
                else
                    degreeOfConfidence = -1;
            }
            bool isDropOffOnly = arrivalEnabled == "false";
            return new RealTimeArrival() { RouteName = routeName, PredictedArrivalTime = predictedArrival, ScheduledArrivalTime = scheduledArrival, DegreeOfConfidence = degreeOfConfidence, Route = routeId, Trip = tripId, Stop = stopId, Destination = destination, IsDropOffOnly = isDropOffOnly, FrequencyMinutes = frequency == -1 ? null : new double?(frequency) };
        }

        private static IEnumerable<T> ParseList<T>(XElement listElement, string elementName, Func<XElement, T> parser)
        {
            return listElement.Elements(elementName).Select(el => parser(el));
        }

        private static async Task<string> SendRequest(string compactRequest, Dictionary<string, string> parameters, bool includeReferences, CancellationToken cancellationToken)
        {
            string request = "http://api.pugetsound.onebusaway.org/api/where/" + compactRequest + ".xml?key=" + Keys.ObaKey + parameters?.Aggregate("", (acc, item) => acc + "&" + item.Key + "=" + item.Value) ?? "" + "includeReferences=" + (includeReferences ? "true" : "false");
            HttpResponseMessage resp;
            HttpClient client;
            try
            {
                using (client = new HttpClient())
                {
                    resp = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, request), cancellationToken);
                }
                if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException();
                return await resp.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Please check your internet connection. ({ex.Message})", ex);
            }
        }

        public override async Task<RetrievedData<IEnumerable<RealTimeArrival>>> GetRealTimeArrivalsForStop(string stopId, int minsBefore, int minsAfter, CancellationToken cancellationToken)
        {
            try
            {
                StringReader reader = new StringReader(await SendRequest("arrivals-and-departures-for-stop/" + stopId, new Dictionary<string, string>() { ["minutesBefore"] = minsBefore.ToString(), ["minutesAfter"] = minsAfter.ToString() }, false, cancellationToken));
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException();

                XDocument xDoc = XDocument.Load(reader);

                XElement el = xDoc.Element("response");
                el = el?.Element("data");
                el = el?.Element("entry");
                el = el?.Element("arrivalsAndDepartures");

                //if (el == null) return null;

                //foreach (XElement el1 in el.Elements("arrivalAndDeparture"))
                //    result.Add(ParseRealTimeArrival(el1));

                return new RetrievedData<IEnumerable<RealTimeArrival>>(ParseList(el, "arrivalAndDeparture", ParseRealTimeArrival));
            }
            catch (OperationCanceledException)
            {
                return new RetrievedData<IEnumerable<RealTimeArrival>>();
            }
            catch (Exception ex)
            {
                return new RetrievedData<IEnumerable<RealTimeArrival>>(ex);
            }
        }
        #endregion
    }
}
