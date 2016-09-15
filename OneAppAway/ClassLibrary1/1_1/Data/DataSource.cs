using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage.Streams;
using static OneAppAway._1_1.AsyncHelpers;

namespace OneAppAway._1_1.Data
{
    public abstract class DataSource
    {
        #region Static
        private static List<DataSource> Sources = new List<DataSource>();

        static DataSource()
        {
            Sources.Add(new MemoryCacheDataSource());
            Sources.Add(new OneBusAwayDataSource());
            Sources.Add(new TransitCenterDataSource());
        }

        //private Task<RetrievedData<T>> GenericGetDataAsync<T, F>(string id, DataSourcePreference preference, CancellationToken cancellationToken, Func<Func<Task<RetrievedData<T>>>> runFunction,  )
        //{
        //    var result = new RetrievedData<T>();
        //    foreach (var source in Sources)
        //    {
        //        if (result.HasData)
        //            continue;
        //        if ((preference & DataSourcePreference.MemoryCache) == 0 && source.IsMemoryCache)
        //            continue;
        //        if ((preference & DataSourcePreference.Offline) == 0 && source.IsOfflineSource)
        //            continue;
        //        if ((preference & DataSourcePreference.Online) == 0 && !source.IsOfflineSource)
        //            continue;
        //        if ((source.GetTransitStopFunction & DataSourceFunctionType.Provision) == DataSourceFunctionType.Provision)
        //            result = await source.GetTransitStop(id, cancellationToken);
        //    }
        //    if (result.HasData)
        //    {
        //        foreach (var source in Sources)
        //        {
        //            if (!result.HasData)
        //                continue;
        //            if ((preference & DataSourcePreference.MemoryCache) == 0 && source.IsMemoryCache)
        //                continue;
        //            if ((preference & DataSourcePreference.Offline) == 0 && source.IsOfflineSource)
        //                continue;
        //            if ((preference & DataSourcePreference.Online) == 0 && !source.IsOfflineSource)
        //                continue;
        //            if ((source.GetTransitStopFunction & DataSourceFunctionType.Correction) == DataSourceFunctionType.Correction)
        //                result = await source.CorrectTransitStop(result.Data, cancellationToken);
        //        }
        //    }
        //    return result;
        //}
        
        public static async Task<RetrievedData<TransitStop>> GetTransitStopAsync(string id, DataSourcePreference preference, CancellationToken cancellationToken)
        {
            var result = new RetrievedData<TransitStop>();
            foreach (var source in Sources)
            {
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException();
                if (result.HasData || !source.IsQualified(preference))
                    continue;
                if ((source.GetTransitStopFunction & DataSourceFunctionType.Provision) == DataSourceFunctionType.Provision)
                    result = await source.GetTransitStop(id, cancellationToken);
            }
            if (result.HasData)
            {
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException();
                foreach (var source in Sources)
                {
                    if (!result.HasData || !source.IsQualified(preference))
                        continue;
                    if ((source.GetTransitStopFunction & DataSourceFunctionType.Correction) == DataSourceFunctionType.Correction)
                        result = await source.CorrectTransitStop(result.Data, cancellationToken);
                }
            }
            return result;
        }

        public static async Task<RetrievedData<TransitRoute>> GetTransitRouteAsync(string id, DataSourcePreference preference, CancellationToken cancellationToken)
        {
            var result = new RetrievedData<TransitRoute>();
            foreach (var source in Sources)
            {
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException();
                if (result.HasData || !source.IsQualified(preference))
                    continue;
                if ((source.GetTransitRouteFunction & DataSourceFunctionType.Provision) == DataSourceFunctionType.Provision)
                    result = await source.GetTransitRoute(id, cancellationToken);
            }
            if (result.HasData)
            {
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException();
                foreach (var source in Sources)
                {
                    if (!result.HasData || !source.IsQualified(preference))
                        continue;
                    if ((source.GetTransitRouteFunction & DataSourceFunctionType.Correction) == DataSourceFunctionType.Correction)
                        result = await source.CorrectTransitRoute(result.Data, cancellationToken);
                }
            }
            return result;
        }

        public static async Task<RetrievedData<RealTimeArrival>> GetRealTimeArrivalAsync(string stopId, string tripId, DataSourcePreference preference, CancellationToken cancellationToken)
        {
            var result = new RetrievedData<RealTimeArrival>();
            foreach (var source in Sources)
            {
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException();
                if (result.HasData || !source.IsQualified(preference))
                    continue;
                if ((source.GetRealTimeArrivalFunction & DataSourceFunctionType.Provision) == DataSourceFunctionType.Provision)
                    result = await source.GetRealTimeArrival(stopId, tripId, cancellationToken);
            }
            if (result.HasData)
            {
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException();
                foreach (var source in Sources)
                {
                    if (!result.HasData || !source.IsQualified(preference))
                        continue;
                    if ((source.GetRealTimeArrivalFunction & DataSourceFunctionType.Correction) == DataSourceFunctionType.Correction)
                        result = await source.CorrectRealTimeArrival(result.Data, cancellationToken);
                }
            }
            return result;
        }

        public static async Task<RetrievedData<IEnumerable<TransitStop>>> GetTransitStopsForAreaAsync(LatLonRect area, DataSourcePreference preference, CancellationToken cancellationToken)
        {
            IEnumerable<TransitStop> resultStops = null;
            List<string> resultErrors = new List<string>();
            List<Exception> resultExceptions = new List<Exception>();
            foreach (var source in Sources)
            {
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException();
                if (!source.IsQualified(preference))
                    continue;
                if (source.CanGetTransitStopsForArea)
                {
                    var sResult = await source.GetTransitStopsForArea(area, cancellationToken);
                    if (sResult.HasData)
                    {
                        if (resultStops == null)
                            resultStops = sResult.Data;
                        else
                            resultStops = resultStops.Union(sResult.Data);
                    }
                    if (sResult.ErrorMessage != null)
                        resultErrors.Add(sResult.ErrorMessage);
                    if (sResult.CaughtException != null)
                        resultExceptions.Add(sResult.CaughtException);
                }
            }
            if (resultStops == null)
                resultStops = new TransitStop[] { };
            return new RetrievedData<IEnumerable<TransitStop>>(resultStops, resultErrors.Count == 0 ? null : resultErrors.Aggregate("", (acc, err) => (acc == "" ? "" : ", ") + err), resultExceptions.Count == 0 ? null : new AggregateException(resultExceptions));
        }

        public static async Task<RetrievedData<IEnumerable<RealTimeArrival>>> GetRealTimeArrivalsForStopAsync(string stopId, int minsBefore, int minsAfter, DataSourcePreference preference, CancellationToken cancellationToken)
        {
            IEnumerable<RealTimeArrival> resultArrivals = null;
            List<string> resultErrors = new List<string>();
            List<Exception> resultExceptions = new List<Exception>();
            foreach (var source in Sources)
            {
                if (!source.IsQualified(preference))
                    continue;

                if (source.CanGetRealTimeArrivalsForStop)
                {
                    var sResult = await source.GetRealTimeArrivalsForStop(stopId, minsBefore, minsAfter, cancellationToken);
                    if (sResult.HasData)
                    {
                        if (resultArrivals == null)
                            resultArrivals = sResult.Data;
                        else
                            resultArrivals = resultArrivals.Union(sResult.Data);
                    }
                    if (sResult.ErrorMessage != null)
                        resultErrors.Add(sResult.ErrorMessage);
                    if (sResult.CaughtException != null)
                        resultExceptions.Add(sResult.CaughtException);
                }
            }
            if (resultArrivals == null)
                resultArrivals = new RealTimeArrival[] { };
            return new RetrievedData<IEnumerable<RealTimeArrival>>(resultArrivals, resultErrors.Count == 0 ? null : resultErrors.Aggregate("", (acc, err) => (acc == "" ? "" : ", ") + err), resultExceptions.Count == 0 ? null : new AggregateException(resultExceptions));
        }

        public static RetrievedData<TransitStop> GetTransitStop(string id, DataSourcePreference preference) => RunSync(() => GetTransitStopAsync(id, preference, CancellationToken.None));
        public static RetrievedData<TransitRoute> GetTransitRoute(string id, DataSourcePreference preference) => RunSync(() => GetTransitRouteAsync(id, preference, CancellationToken.None));
        public static RetrievedData<RealTimeArrival> GetRealTimeArrival(string stopId, string tripId, DataSourcePreference preference) => RunSync(() => GetRealTimeArrivalAsync(stopId, tripId, preference, CancellationToken.None));
        public static RetrievedData<IEnumerable<TransitStop>> GetTransitStopsForArea(LatLonRect area, DataSourcePreference preference) => RunSync(() => GetTransitStopsForAreaAsync(area, preference, CancellationToken.None));
        public static RetrievedData<IEnumerable<RealTimeArrival>> GetRealTimeArrivalsForStop(string stopId, int minsBefore, int minsAfter, DataSourcePreference preference) => RunSync(() => GetRealTimeArrivalsForStopAsync(stopId, minsBefore, minsAfter, preference, CancellationToken.None));
        #endregion

        #region Instance
        private bool IsQualified(DataSourcePreference preference)
        {
            if (preference == DataSourcePreference.MemoryCacheOnly)
                return IsMemoryCache;
            if ((preference & DataSourcePreference.MemoryCache) == 0 && IsMemoryCache)
                return false;
            if ((preference & DataSourcePreference.Offline) == 0 && IsOfflineSource)
                return false;
            if ((preference & DataSourcePreference.Online) == 0 && !IsOfflineSource)
                return false;
            return true;
        }

        public abstract bool IsOfflineSource { get; }
        public abstract bool IsMemoryCache { get; }

        public abstract DataSourceFunctionType GetTransitStopFunction { get; }
        public abstract Task<RetrievedData<TransitStop>> GetTransitStop(string id, CancellationToken cancellationToken);
        public abstract Task<RetrievedData<TransitStop>> CorrectTransitStop(TransitStop stop, CancellationToken cancellationToken);

        public abstract DataSourceFunctionType GetTransitRouteFunction { get; }
        public abstract Task<RetrievedData<TransitRoute>> GetTransitRoute(string id, CancellationToken cancellationToken);
        public abstract Task<RetrievedData<TransitRoute>> CorrectTransitRoute(TransitRoute route, CancellationToken cancellationToken);

        public abstract DataSourceFunctionType GetRealTimeArrivalFunction { get; }
        public abstract Task<RetrievedData<RealTimeArrival>> GetRealTimeArrival(string stopId, string tripId, CancellationToken cancellationToken);
        public abstract Task<RetrievedData<RealTimeArrival>> CorrectRealTimeArrival(RealTimeArrival arrival, CancellationToken cancellationToken);

        public abstract bool CanGetTransitStopsForArea { get; }
        public abstract Task<RetrievedData<IEnumerable<TransitStop>>> GetTransitStopsForArea(LatLonRect area, CancellationToken cancellationToken);

        public abstract bool CanGetRealTimeArrivalsForStop { get; }
        public abstract Task<RetrievedData<IEnumerable<RealTimeArrival>>> GetRealTimeArrivalsForStop(string stopId, int minsBefore, int minsAfter, CancellationToken cancellationToken);
        #endregion
    }
}
