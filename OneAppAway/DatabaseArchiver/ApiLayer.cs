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
            if (cancellationToken.IsCancellationRequested) return null;
            return await resp.Content.ReadAsStringAsync();
        }

        public static async Task<Agency[]> GetTransitAgencies(CancellationToken cancellationToken)
        {
            List<Agency> result = new List<Agency>();
            StringReader reader = new StringReader(await SendRequest("agencies-with-coverage", null, true, cancellationToken));
            if (cancellationToken.IsCancellationRequested)
                return null;

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
                return null;

            XDocument xDoc = XDocument.Load(reader);

            foreach (var el in xDoc.Element("response").Element("data").Element("list").Elements("string"))
            {
                result.Add(el.Value);
            }

            return result.ToArray();
        }

        public static async Task<Bus_Route[]> GetBusRoutesForAgency(string agencyId, CancellationToken cancellationToken)
        {
            List<Bus_Route> result = new List<Bus_Route>();
            StringReader reader = new StringReader(await SendRequest("routes-for-agency/" + agencyId, null, false, cancellationToken));
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
                result.Add(new Bus_Route() { RouteID = routeId, Name = routeName, Description = routeDescription, AgencyID = routeAgency });
            }
            return result.ToArray();
        }
    }
}
