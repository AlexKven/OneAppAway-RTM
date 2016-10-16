using OneAppAway._1_1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace OneAppAway._.Data
{
    public class InterimObaScheduleSource : DataSource
    {
        #region Not Implemented
        public override bool CanGetRealTimeArrivalsForStop => false;

        public override bool CanGetTransitStopsForArea => false;

        public override DataSourceFunctionType GetRealTimeArrivalFunction => DataSourceFunctionType.None;

        public override DataSourceFunctionType GetTransitRouteFunction => DataSourceFunctionType.None;

        public override DataSourceFunctionType GetTransitStopFunction => DataSourceFunctionType.None;

        public override Task<RetrievedData<RealTimeArrival>> CorrectRealTimeArrival(RealTimeArrival arrival, CancellationToken cancellationToken)
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

        public override Task<RetrievedData<TransitStop>> GetTransitStop(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Task<RetrievedData<IEnumerable<TransitStop>>> GetTransitStopsForArea(LatLonRect area, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        #endregion

        public override bool IsMemoryCache => false;

        public override bool IsOfflineSource => false;

        public override bool CanGetScheduleForStop => true;

        public override async Task<RetrievedData<_1_1.Data.WeekSchedule>> GetScheduleForStop(string stopId, CancellationToken cancellationToken)
        {
            try
            {
                if (stopId == null)
                {
                    return new RetrievedData<_1_1.Data.WeekSchedule>();
                }
                var schedule = await ApiLayer.GetScheduleForStop(stopId, cancellationToken);
                return new RetrievedData<_1_1.Data.WeekSchedule>(schedule);
            }
            catch (OperationCanceledException)
            {
                return new RetrievedData<_1_1.Data.WeekSchedule>();
            }
            catch (Exception ex)
            {
                return new RetrievedData<_1_1.Data.WeekSchedule>(ex);
            }
        }
    }
}
