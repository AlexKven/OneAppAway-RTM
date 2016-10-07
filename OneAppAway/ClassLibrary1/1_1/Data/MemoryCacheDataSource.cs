using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OneAppAway._1_1.Data
{
    internal class MemoryCacheDataSource : DataSource
    {
        #region Properties
        public override bool IsMemoryCache => true;
        public override bool IsOfflineSource => true;

        public override DataSourceFunctionType GetTransitRouteFunction => DataSourceFunctionType.None;
        public override DataSourceFunctionType GetTransitStopFunction => DataSourceFunctionType.Provision;
        public override DataSourceFunctionType GetRealTimeArrivalFunction => DataSourceFunctionType.None;
        public override bool CanGetTransitStopsForArea => true;
        public override bool CanGetRealTimeArrivalsForStop => false;
        #endregion

        #region Not Implemented
        public override bool CanGetScheduleForStop => false;
        public override Task<RetrievedData<WeekSchedule>> GetScheduleForStop(string stopId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        #endregion

        public override Task<RetrievedData<RealTimeArrival>> CorrectRealTimeArrival(RealTimeArrival arrival, CancellationToken cancellationToken)
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

        public override Task<RetrievedData<TransitRoute>> CorrectTransitRoute(TransitRoute route, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task<RetrievedData<TransitStop>> CorrectTransitStop(TransitStop stop, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task<RetrievedData<TransitRoute>> GetTransitRoute(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task<RetrievedData<TransitStop>> GetTransitStop(string id, CancellationToken cancellationToken)
        {
            var result = MemoryCache.GetStop(id);
            if (result.HasValue)
                return Task.FromResult(new RetrievedData<TransitStop>(result.Value));
            else
                return Task.FromResult(new RetrievedData<TransitStop>());
        }

        public override Task<RetrievedData<IEnumerable<TransitStop>>> GetTransitStopsForArea(LatLonRect area, CancellationToken cancellationToken)
        {
            var result = MemoryCache.QueryStops(stop => area.ContainsLatLon(stop.Position));
            return Task.FromResult(new RetrievedData<IEnumerable<TransitStop>>(result));
        }
    }
}
