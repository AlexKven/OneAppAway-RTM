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
    }
}
