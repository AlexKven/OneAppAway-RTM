using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OneAppAway
{
    [Flags]
    public enum ServiceDay : byte
    {
        None = 0,
        Monday = 1,
        Tuesday = 2,
        Wednesday = 4,
        Thursday = 8,
        Friday = 16,
        Saturday = 32,
        Sunday = 64,
        ReducedWeekday = 128,
        Weekdays= 31,
        AllWeekdays = 159,
        Weekends = 96,
        All = 255
    }

    public class WeekSchedule
    {
        private List<ServiceDay> Days = new List<ServiceDay>();
        private List<DaySchedule> DaySchedules = new List<DaySchedule>();
        private List<ServiceDay> TechnicalDays = new List<ServiceDay>();

        public WeekSchedule() { }

        public string Stop { get; set; }

        public void AddServiceDay(ServiceDay day, DaySchedule schedule)
        {
            if (schedule == null || schedule.IsEmpty) return;
            ServiceDay totalDay = day;
            List<int> psuedoIdenticalDays = new List<int>();
            for (int i = 0; i < DaySchedules.Count; i++)
            {
                if (DaySchedules[i].IsIdenticalToByTripId(schedule))
                {
                    TechnicalDays[i] |= day;
                    Days[i] |= day;
                    return;
                }
                else if (DaySchedules[i].IsIdenticalToByTime(schedule))
                {
                    totalDay |= TechnicalDays[i];
                    psuedoIdenticalDays.Add(i);
                }
            }
            foreach (var ind in psuedoIdenticalDays)
                Days[ind] = totalDay;
            TechnicalDays.Add(day);
            Days.Add(totalDay);
            DaySchedules.Add(schedule);
        }

        public DaySchedule this[ServiceDay day]
        {
            get
            {
                for (int i = 0; i < TechnicalDays.Count; i++)
                {
                    if (TechnicalDays[i].HasFlag(day) || day.HasFlag(TechnicalDays[i]))
                        return DaySchedules[i];
                }
                return null;
            }
        }

        public ServiceDay[] DayGroups
        {
            get
            {
                List<ServiceDay> result = new List<ServiceDay>();
                foreach (var day in Days)
                {
                    if (!result.Contains(day))
                        result.Add(day);
                }
                return result.ToArray();
            }
        }

        public ServiceDay[] TechnicalDayGroups
        {
            get { return TechnicalDays.ToArray(); }
        }

        public void FilterByRoutes(params string[] routeIds)
        {
            foreach (var sch in DaySchedules)
                sch?.FilterByRoutes(routeIds);
        }

        public void RemoveRoutes(params string[] routeIds)
        {
            foreach (var sch in DaySchedules)
                sch?.RemoveRoutes(routeIds);
        }

        public void Format(CompactFormatWriter formatter)
        {
            for (int i = 0; i < DaySchedules.Count; i++)
            {
                formatter.WriteInt((int)Days[i]);
                formatter.WriteInt((int)TechnicalDays[i]);
                formatter.OpenParens();
                DaySchedules[i].Format(formatter);
                formatter.CloseParens();
                formatter.NextItem();
            }
        }

        public void Deformat(CompactFormatReader reader)
        {
            Days.Clear();
            TechnicalDays.Clear();
            DaySchedules.Clear();
            CompactFormatReader[] items;
            while ((items = reader.Next()) != null)
            {
                ServiceDay nextDay = (ServiceDay)items[0].ReadInt();
                ServiceDay nextTechnicalDay = (ServiceDay)items[1].ReadInt();
                DaySchedule schedule = new DaySchedule();
                schedule.Deformat(items[2]);
                Days.Add(nextDay);
                TechnicalDays.Add(nextTechnicalDay);
                DaySchedules.Add(schedule);
            }
        }

        public void MergeByRoute(WeekSchedule other)
        {
            ServiceDay[] days = { ServiceDay.Monday, ServiceDay.Tuesday, ServiceDay.Wednesday, ServiceDay.Thursday, ServiceDay.Friday, ServiceDay.Saturday, ServiceDay.Sunday, ServiceDay.ReducedWeekday };
            DaySchedule[] sch1 = new DaySchedule[8];
            DaySchedule[] sch2 = new DaySchedule[8];
            for (int i = 0; i < 8; i++)
            {
                sch1[i] = this[days[i]]?.Clone();
                sch2[i] = other[days[i]];
                if (sch1[i] == null)
                {
                    if (sch2[i] != null)
                        sch1[i] = sch2[i].Clone();
                }
                else
                {
                    if (sch2[i] != null)
                        sch1[i].MergeByRoute(sch2[i]);
                }
            }
            this.Days.Clear();
            this.DaySchedules.Clear();
            this.TechnicalDays.Clear();
            for (int i = 0; i < 8; i++)
            {
                this.AddServiceDay(days[i], sch1[i]);
            }
        }

        public string[] Routes
        {
            get
            {
                List<string> result = new List<string>();
                foreach (var sch in DaySchedules)
                {
                    if (sch != null)
                        result.AddRange(sch.Routes);
                }
                return result.ToArray();
            }
        }

        public bool IsEmpty
        {
            get
            {
                foreach (var sch in DaySchedules)
                {
                    if (sch != null && !sch.IsEmpty)
                        return false;
                }
                return true;
            }
        }
    }

    public class DaySchedule : IEnumerable<ScheduledArrival>
    {
        private Tuple<string, string, Tuple<short, string, short?>[]>[] Data;
        private string[] TripIds;
        private string[] NamesAndTimes;
        public string Stop { get; set; }

        public DaySchedule() { }

        public DaySchedule(string stop)
        {
            Stop = stop;
        }

        public bool IsEmpty
        {
            get { return Data == null || Data.Length == 0; }
        }

        public void LoadFromVerboseString(string str, string stop)
        {
            Stop = stop;
            TripIds = null;
            NamesAndTimes = null;
            List<string> routesInOrder = new List<string>();
            Func<ScheduledArrival, ScheduledArrival, int> comparison = (left, right) =>
            {
                int result;
                if (routesInOrder.IndexOf(left.Route) < routesInOrder.IndexOf(right.Route))
                    return -1;
                else if (routesInOrder.IndexOf(left.Route) > routesInOrder.IndexOf(right.Route))
                    return 1;
                else if ((result = string.CompareOrdinal(left.Destination, right.Destination)) != 0)
                    return result;
                else if (left.ScheduledDepartureTime < right.ScheduledDepartureTime)
                    return -1;
                else if (left.ScheduledDepartureTime > right.ScheduledDepartureTime)
                    return 1;
                return 0;
            };
            SortedSet<ScheduledArrival> sortedArrivals = new SortedSet<ScheduledArrival>(Comparer<ScheduledArrival>.Create(new Comparison<ScheduledArrival>(comparison)));
            try
            {
                StringReader reader = new StringReader(str);
                XDocument xDoc = XDocument.Load(reader);

                XElement el = xDoc.Element("response").Element("data").Element("entry").Element("stopRouteSchedules");
                foreach (var el2 in el.Elements("stopRouteSchedule"))
                {
                    string route = el2.Element("routeId")?.Value;
                    routesInOrder.Add(route);
                    foreach (var el3 in el2.Element("stopRouteDirectionSchedules").Elements("stopRouteDirectionSchedule"))
                    {
                        string sign = el3.Element("tripHeadsign")?.Value;
                        foreach (var el4 in el3.Element("scheduleStopTimes")?.Elements("scheduleStopTime"))
                        {
                            string arrivalTimeStr = (el4.Element("arrivalEnabled")?.Value == "true") ? el4.Element("arrivalTime")?.Value : null;
                            string departureTimeStr = (el4.Element("departureEnabled")?.Value == "true") ? el4.Element("departureTime")?.Value : null;
                            if (arrivalTimeStr == departureTimeStr)
                                arrivalTimeStr = null;
                            if (departureTimeStr == null && arrivalTimeStr != null)
                            {
                                departureTimeStr = arrivalTimeStr;
                                arrivalTimeStr = null;
                            }
                            string tripId = el4.Element("tripId")?.Value;
                            //DateTime time = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromMilliseconds(long.Parse(arrivalTimeStr))).ToLocalTime();
                            var arrivalTime = arrivalTimeStr.GetDateTimeFromEpochTime();
                            var departureTime = departureTimeStr.GetDateTimeFromEpochTime();
                            if (departureTime.HasValue)
                                sortedArrivals.Add(Correction.Correct(new ScheduledArrival() { Route = route, Stop = this.Stop, Trip = tripId, Destination = sign, ScheduledDepartureTime = departureTime.Value, ScheduledArrivalTime = arrivalTime }));
                        }
                    }
                }

                List<Tuple<string, string, Tuple<short, string, short?>[]>> data = new List<Tuple<string, string, Tuple<short, string, short?>[]>>();
                string lastRoute = null;
                string lastDestination = null;
                List<Tuple<short, string, short?>> trips = null;
                foreach (var item in sortedArrivals)
                {
                    if (item.Destination != lastDestination || item.Route != lastRoute)
                    {
                        if (trips != null)
                            data.Add(new Tuple<string, string, Tuple<short, string, short?>[]>(lastRoute, lastDestination, trips.ToArray()));
                        lastRoute = item.Route;
                        lastDestination = item.Destination;
                        trips = new List<Tuple<short, string, short?>>();
                    }
                    trips.Add(new Tuple<short, string, short?>(item.ScheduledDepartureTime.GetShortTime(), item.Trip, item.ScheduledArrivalTime?.GetShortTime()));
                }
                if (trips != null)
                    data.Add(new Tuple<string, string, Tuple<short, string, short?>[]>(lastRoute, lastDestination, trips.ToArray()));
                
                Data = data.ToArray();
            }
            catch (Exception) { }
        }

        public void FilterByRoutes(params string[] routeIds)
        {
            NamesAndTimes = null;
            TripIds = null;
            Data = Data.Where(item => routeIds.Contains(item.Item1)).ToArray();
        }

        public void RemoveRoutes(params string[] routeIds)
        {
            NamesAndTimes = null;
            TripIds = null;
            Data = Data.Where(item => !routeIds.Contains(item.Item1)).ToArray();
        }

        public bool IsIdenticalToByTime(DaySchedule other)
        {
            List<string> ids = GetNamesAndTimes(true);
            List<string> otherIds = other.GetNamesAndTimes(true);
            int lastInd = ids.Count - 1;
            while (lastInd >= 0)
            {
                if (!otherIds.Remove(ids[lastInd]))
                    return false;
                ids.RemoveAt(lastInd);
                lastInd--;
            }
            return otherIds.Count == 0;
        }

        public bool IsIdenticalToByTripId(DaySchedule other)
        {
            List<string> ids = GetTripIds(true);
            List<string> otherIds = other.GetTripIds(true);
            int lastInd = ids.Count - 1;
            while (lastInd >= 0)
            {
                if (!otherIds.Remove(ids[lastInd]))
                    return false;
                ids.RemoveAt(lastInd);
                lastInd--;
            }
            return otherIds.Count == 0;
        }

        private List<string> GetTripIds(bool save)
        {
            if (TripIds == null)
            {
                List<string> result = new List<string>();
                foreach (var item0 in Data)
                {
                    foreach (var item1 in item0.Item3)
                    {
                        result.Add(item1.Item2);
                    }
                }
                if (save)
                    TripIds = result.ToArray();
                return result;
            }
            else
                return TripIds.ToList();
        }

        private List<string> GetNamesAndTimes(bool save)
        {
            if (NamesAndTimes == null)
            {
                List<string> result = new List<string>();
                foreach (var item0 in Data)
                {
                    foreach (var item1 in item0.Item3)
                    {
                        result.Add(item0.Item1 + item0.Item2 + item1.Item1);
                    }
                }
                if (save)
                    NamesAndTimes = result.ToArray();
                return result;
            }
            else
                return NamesAndTimes.ToList();
        }

        public IEnumerator<ScheduledArrival> GetEnumerator()
        {
            return new ScheduleEnumerator(Stop, Data);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Format(CompactFormatWriter formatter)
        {
            string curRoute = "";
            foreach (var item in Data)
            {
                if (curRoute == "")
                {
                    curRoute = item.Item1;
                    formatter.WriteString(curRoute);
                    formatter.OpenParens();
                }
                else if (item.Item1 != curRoute)
                {
                    curRoute = item.Item1;
                    formatter.CloseParens();
                    formatter.NextItem();
                    formatter.WriteString(curRoute);
                    formatter.OpenParens();
                }
                formatter.WriteQuotedString(item.Item2);
                formatter.OpenParens();
                foreach (var subItem in item.Item3)
                {
                    formatter.WriteInt(subItem.Item1);
                    formatter.WriteString(subItem.Item2);
                    if (subItem.Item3 != null)
                        formatter.WriteInt(subItem.Item3.Value);
                    formatter.NextItem();
                }
                formatter.CloseParens();
                formatter.NextItem();
            }
            formatter.CloseParens();
        }

        public void Deformat(CompactFormatReader reader)
        {
            List<Tuple<string, string, Tuple<short, string, short?>[]>> data = new List<Tuple<string, string, Tuple<short, string, short?>[]>>();

            CompactFormatReader[] curRouteReader;
            while ((curRouteReader = reader.Next()) != null)
            {
                string curRoute = curRouteReader[0].ReadString();
                CompactFormatReader[] curDirectionReader;
                while ((curDirectionReader = curRouteReader[1].Next()) != null)
                {
                    string sign = curDirectionReader[0].ReadString();
                    List<Tuple<short, string, short?>> trips = new List<Tuple<short, string, short?>>();
                    CompactFormatReader[] curTripReader;
                    while ((curTripReader = curDirectionReader[1].Next()) != null)
                    {
                        
                        trips.Add(new Tuple<short, string, short?>((short)curTripReader[0].ReadInt(), curTripReader[1].ReadString(), (curTripReader.Length > 2) ? new short?((short)curTripReader[2].ReadInt()) : null));
                    }
                    data.Add(new Tuple<string, string, Tuple<short, string, short?>[]>(curRoute, sign, trips.ToArray()));
                }
            }
            Data = data.ToArray();
        }

        public void MergeByRoute(DaySchedule other)
        {
            var newData = Data?.ToList();
            if (newData == null)
                newData = new List<Tuple<string, string, Tuple<short, string, short?>[]>>();
            foreach (var item in other.Data)
            {
                if (!newData.Any(itm => itm.Item1 == item.Item1 && itm.Item2 == item.Item2))
                    newData.Add(item);
            }
            Data = newData.ToArray();
            TripIds = null;
            NamesAndTimes = null;
        }

        public DaySchedule Clone()
        {
            DaySchedule result = new DaySchedule();
            result.MergeByRoute(this);
            return result;
        }

        public string[] Routes
        {
            get
            {
                if (Data == null) return new string[0];
                List<string> result = new List<string>();
                foreach (var item in Data)
                {
                    if (!result.Contains(item.Item1))
                        result.Add(item.Item1);
                }
                return result.ToArray();
            }
        }
    }

    public class ScheduleEnumerator : IEnumerator<ScheduledArrival>
    {
        private Tuple<string, string, Tuple<short, string, short?>[]>[] Data;
        private string Stop;
        private int curRouteDirection = -1;
        private int curTrip = -1;

        public ScheduleEnumerator(string stop, Tuple<string, string, Tuple<short, string, short?>[]>[] data)
        {
            Data = data;
            Stop = stop;
        }

        public ScheduledArrival Current
        {
            get
            {
                if (curRouteDirection == -1)
                    throw new IndexOutOfRangeException("Run MoveNext() before accessing Current.");
                if (curRouteDirection >= Data.Length)
                    throw new IndexOutOfRangeException("You have moved past the last element.");
                var item1 = Data[curRouteDirection];
                var item2 = item1.Item3[curTrip];
                DateTime departureTime = DateTime.Now;
                departureTime = new DateTime(departureTime.Year, departureTime.Month, departureTime.Day, item2.Item1 / 60, item2.Item1 % 60, 0);
                DateTime? arrivalTime = DateTime.Now;
                arrivalTime = item2.Item3 == null ? null : new DateTime?(new DateTime(arrivalTime.Value.Year, arrivalTime.Value.Month, arrivalTime.Value.Day, item2.Item3.Value / 60, item2.Item3.Value % 60, 0));
                return new ScheduledArrival() { Destination = item1.Item2, Route = item1.Item1, ScheduledDepartureTime = departureTime, Stop = this.Stop, Trip = item2.Item2, ScheduledArrivalTime = arrivalTime };
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public void Dispose() { }

        public bool MoveNext()
        {
            if (curRouteDirection == -1)
                curRouteDirection = 0;
            var item1 = Data[curRouteDirection];
            curTrip++;
            if (curTrip >= item1.Item3.Length)
            {
                curTrip = 0;
                curRouteDirection++;
            }
            return curRouteDirection < Data.Length;
        }

        public void Reset()
        {
            curTrip = 0;
            curRouteDirection = 0;
        }
    }
}
