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

namespace DatabaseArchiver
{
    public static class ApiLayer
    {
        public static async Task<string> SendRequest(string compactRequest, Dictionary<string, string> parameters, bool includeReferences, CancellationToken cancellationToken)
        {
            HttpClient client = new HttpClient();
            string request = "http://api.pugetsound.onebusaway.org/api/where/" + compactRequest + ".xml?key=" + Keys.ObaKey + parameters?.Aggregate("", (acc, item) => acc + "&" + item.Key + "=" + item.Value) ?? "" + "includeReferences=" + (includeReferences ? "true" : "false");
            var resp = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, request), cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                throw new OperationCanceledException();
            return await resp.Content.ReadAsStringAsync();
        }

        public static async Task<Agency[]> GetTransitAgencies(CancellationToken cancellationToken)
        {
            List<Agency> result = new List<Agency>();
            StringReader reader = new StringReader(await SendRequest("agencies-with-coverage", null, true, cancellationToken));
            if (cancellationToken.IsCancellationRequested)
                throw new OperationCanceledException();

            XDocument xDoc = XDocument.Load(reader);
            
            foreach (var el in xDoc.Element("response").Element("data").Element("references").Element("agencies").Elements("agency"))
            {
                string agencyId = el.Element("id")?.Value;
                string agencyName = el.Element("name")?.Value;
                string agencyUrl = el.Element("url")?.Value;
                result.Add(new Agency() { AgencyID = agencyId, Name = agencyName, URL = agencyUrl });
            }

            return result.ToArray();
        }

        public static async Task<string[]> GetRouteIDsForAgency(string agencyID, CancellationToken cancellationToken)
        {
            List<string> result = new List<string>();
            StringReader reader = new StringReader(await SendRequest("route-ids-for-agency/" + agencyID, null, false, cancellationToken));
            if (cancellationToken.IsCancellationRequested)
                throw new OperationCanceledException();

            XDocument xDoc = XDocument.Load(reader);

            foreach (var el in xDoc.Element("response").Element("data").Element("list").Elements("string"))
            {
                result.Add(el.Value);
            }

            return result.ToArray();
        }

        public static async Task<Route[]> GetBusRoutesForAgency(string agencyId, CancellationToken cancellationToken)
        {
            List<Route> result = new List<Route>();
            StringReader reader = new StringReader(await SendRequest("routes-for-agency/" + agencyId, null, false, cancellationToken));
            if (cancellationToken.IsCancellationRequested)
                throw new OperationCanceledException();

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
                result.Add(new Route() { RouteID = routeId, Name = routeName, Description = routeDescription, AgencyID = routeAgency });
            }
            return result.ToArray();
        }

        public static async Task<string[]> GetBusStop(string stopID, CancellationToken cancellationToken)
        {
            StringReader reader = new StringReader(await SendRequest("stop/" + stopID, new Dictionary<string, string>() {["includeReferences"] = "false" }, false, cancellationToken));
            if (cancellationToken.IsCancellationRequested)
                throw new OperationCanceledException();

            XDocument xDoc = XDocument.Load(reader);

            var el = xDoc.Element("response").Element("data").Element("entry");
            return new string[] { stopID, el.Element("code").Value, el.Element("name").Value, el.Element("lat").Value, el.Element("lon").Value, el.Element("direction")?.Value, el.Element("locationType")?.Value };
        }
        
        public static async Task<Tuple<string[], Tuple<string, int>[]>> GetStopIDsAndShapesForRoute(string route, CancellationToken cancellationToken)
        {
            var stopResult = new List<string>();
            var shapeResult = new List<Tuple<string, int>>();

            string responseString = await SendRequest("stops-for-route/" + route, new Dictionary<string, string>() { ["includeReferences"] = "false" }, true, cancellationToken);
            if (cancellationToken.IsCancellationRequested)
                throw new OperationCanceledException();

            StringReader reader = new StringReader(responseString);
            XDocument xDoc = XDocument.Load(reader);
            foreach (XElement el in xDoc.Element("response")?.Element("data")?.Element("entry")?.Element("stopIds")?.Elements("string"))
            {
                stopResult.Add(el.Value);
            }

            foreach (XElement el in xDoc.Element("response")?.Element("data").Element("entry").Element("polylines").Elements("encodedPolyline"))
            {
                shapeResult.Add(new Tuple<string, int>(el.Element("points").Value, int.Parse(el.Element("length").Value)));
            }

            return new Tuple<string[], Tuple<string, int>[]>(stopResult.ToArray(), shapeResult.ToArray());
        }
    }
}
