using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OneAppAway._1_1.Data
{
    public class TransitCenterDataSource : DataSource
    {
        private readonly TransitStop[] Stops;
        public TransitCenterDataSource()
        {
            var assembly = Assembly.Load(new AssemblyName("CommonClasses"));
            XmlSerializer deserializer = new XmlSerializer(typeof(TransitStop[]), new XmlAttributeOverrides(), TransitStop.GetSerializationTypes().ToArray(), new XmlRootAttribute("TransitStops"), null);
            using (var stream = assembly.GetManifestResourceStream(@"OneAppAway._1_1.XmlDataSources.TransitCenterData.xml"))
            {
                Stops = (TransitStop[])deserializer.Deserialize(stream);
            }
        }

        #region Properties
        public override bool IsMemoryCache => true;
        public override bool IsOfflineSource => true;

        public override bool CanGetRealTimeArrivalsForStop => false;
        public override bool CanGetTransitStopsForArea => true;
        public override DataSourceFunctionType GetRealTimeArrivalFunction => DataSourceFunctionType.None;
        public override DataSourceFunctionType GetTransitRouteFunction => DataSourceFunctionType.None;
        public override DataSourceFunctionType GetTransitStopFunction => DataSourceFunctionType.Both;
        #endregion

        #region Unused
        public override Task<RetrievedData<RealTimeArrival>> CorrectRealTimeArrival(RealTimeArrival arrival, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task<RetrievedData<TransitRoute>> CorrectTransitRoute(TransitRoute route, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task<RetrievedData<RealTimeArrival>> GetRealTimeArrival(string stopId, string tripId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task<RetrievedData<IEnumerable<RealTimeArrival>>> GetRealTimeArrivalsForStop(string stopId, int minsBefore, int minsAfter, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task<RetrievedData<TransitRoute>> GetTransitRoute(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Abstract Implementations
        public override Task<RetrievedData<TransitStop>> CorrectTransitStop(TransitStop stop, CancellationToken cancellationToken)
        {
            stop.Parent = Stops.FirstOrDefault(st => st.Children?.Contains(stop.ID) ?? false).ID;
            return Task.FromResult(new RetrievedData<TransitStop>(stop));
        }

        public override Task<RetrievedData<TransitStop>> GetTransitStop(string id, CancellationToken cancellationToken)
        {
            return Task.FromResult(new RetrievedData<TransitStop>(Stops.FirstOrDefault(stop => stop.ID == id)));
        }

        public override Task<RetrievedData<IEnumerable<TransitStop>>> GetTransitStopsForArea(LatLonRect area, CancellationToken cancellationToken)
        {
            return Task.FromResult(new RetrievedData<IEnumerable<TransitStop>>(Stops.Where(stop => area.ContainsLatLon(stop.Position))));
        }
        #endregion
    }
}
