using MvvmHelpers;
using OneAppAway._1_1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OneAppAway._1_1.ViewModels
{
    public class ScheduleControlViewModel : BaseViewModel
    {
        public class RouteScheduleGroup
        {
            public string RouteAndDestination { get; set; }
            public IEnumerable<Tuple<string, bool>> Times { get; set; }
        }

        private WeekSchedule Schedule;
        private CancellationTokenSource TokenSource;

        public ScheduleControlViewModel()
        {
            TokenSource = new CancellationTokenSource();
        }

        public ObservableRangeCollection<ServiceDay> DayGroups { get; } = new ObservableRangeCollection<ServiceDay>();

        private ServiceDay _SelectedDayGroup = ServiceDay.None;
        public ServiceDay SelectedDayGroup
        {
            get { return _SelectedDayGroup; }
            set
            {
                SetProperty(ref _SelectedDayGroup, value);
                LoadSelectedSchedule();
            }
        }

        private async Task<IEnumerable<Tuple<ScheduledArrival, string>>> LoadRouteNames(IEnumerable<ScheduledArrival> arrivals)
        {
            List<Tuple<ScheduledArrival, string>> result = new List<Tuple<ScheduledArrival, string>>();
            string currentRouteId = null;
            string currentRouteName = null;
            foreach (var arrival in arrivals)
            {
                if (currentRouteId != arrival.Route)
                {
                    currentRouteName = null;
                    currentRouteId = arrival.Route;
                }
                if (currentRouteName == null)
                {
                    currentRouteName = (await DataSource.GetTransitRouteAsync(arrival.Route, DataSourcePreference.All, TokenSource.Token))?.Data.Name ?? "";
                }
                result.Add(new Tuple<ScheduledArrival, string>(arrival, currentRouteName));
            }
            return result;
        }

        private async void LoadSelectedSchedule()
        {
            SelectedSchedule.Clear();
            if (SelectedDayGroup != ServiceDay.None)
            {
                var sched = await LoadRouteNames(Schedule.GetSchedule(SelectedDayGroup));
                bool military = SettingsManagerBase.Instance?.GetSetting("MilitaryTime", false, false) ?? false;
                SelectedSchedule.AddRange(sched.GroupBy(a => $"{a.Item2} to {a.Item1.Destination}").Select(g => new RouteScheduleGroup() { RouteAndDestination = g.Key, Times = g.Select(t => new Tuple<string, bool>(GetDisplayTimeForArrival(t.Item1, military), military ? false : (t.Item1.ScheduledDepartureTime.Hour >= 12))) }));
            }
        }

        private static string GetDisplayTimeForArrival(ScheduledArrival arrival, bool military)
        {
            StringBuilder result = new StringBuilder();
            if (arrival.ScheduledArrivalTime != null)
                result.Append($"({(int)(arrival.ScheduledDepartureTime - arrival.ScheduledArrivalTime.Value).TotalMinutes})");
            result.Append(arrival.ScheduledDepartureTime.ToString(military ? "HH:mm" : "h:mm"));
            if (arrival.IsDropOffOnly)
                result.Append("D");
            return result.ToString();
        }

        public ObservableRangeCollection<RouteScheduleGroup> SelectedSchedule { get; } = new ObservableRangeCollection<RouteScheduleGroup>();

        private bool _HasSchedule = false;
        public bool HasSchedule
        {
            get { return _HasSchedule; }
            set { SetProperty(ref _HasSchedule, value); }
        }

        private TransitStop _Stop;
        public TransitStop Stop
        {
            get { return _Stop; }
            set
            {
                SetProperty(ref _Stop, value);
                LoadSchedule();
            }
        }

        private async void LoadSchedule()
        {
            try
            {
                IsBusy = true;
                if (Stop.ID == null)
                {
                    Subtitle = "No stop is selected.";
                    HasSchedule = false;
                }
                var retrievedSchedule = await DataSource.GetScheduleForStopAsync(Stop.ID, DataSourcePreference.All, TokenSource.Token);
                if (retrievedSchedule.HasData)
                {
                    Schedule = retrievedSchedule.Data;
                    DayGroups.Clear();
                    var groups = Schedule.GetScheduleGroups().ToArray();
                    if (groups.Length > 0)
                        DayGroups.AddRange(groups);
                    HasSchedule = true;
                }
            }
            catch (OperationCanceledException)
            {
                Subtitle = "Schedule retrieval was cancelled.";
                HasSchedule = false;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
