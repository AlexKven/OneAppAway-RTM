using MvvmHelpers;
using OneAppAway._1_1.Data;
using OneAppAway._1_1.Helpers;
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

        public class DayScheduleGroup
        {
            public ServiceDay DayGroup { get; }
            public string DayGroupDescription { get; }
            public bool ScheduleLoaded { get; set; } = false;
            public ObservableRangeCollection<RouteScheduleGroup> Schedule { get; } = new ObservableRangeCollection<RouteScheduleGroup>();

            public DayScheduleGroup(ServiceDay dayGroup)
            {
                DayGroup = dayGroup;
            }

            public override string ToString()
            {
                return DayGroup.GetFriendlyName();
            }
        }

        private WeekSchedule Schedule;
        private CancellationTokenSource TokenSource;

        public ScheduleControlViewModel()
        {
            TokenSource = new CancellationTokenSource();
        }

        private void CancelTasks()
        {
            TokenSource.Cancel();
            TokenSource = new CancellationTokenSource();
        }

        public ObservableRangeCollection<DayScheduleGroup> DayGroups { get; } = new ObservableRangeCollection<DayScheduleGroup>();

        private DayScheduleGroup _SelectedDayGroup;
        public DayScheduleGroup SelectedDayGroup
        {
            get { return _SelectedDayGroup; }
            set
            {
                SetProperty(ref _SelectedDayGroup, value);
                LoadSelectedSchedule().ToString();
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

        public async Task LoadSelectedSchedule()
        {
            IsBusy = true;
            if (SelectedDayGroup != null)
            {
                if (!SelectedDayGroup.ScheduleLoaded)
                {
                    SelectedDayGroup.Schedule.Clear();
                    if (SelectedDayGroup.DayGroup != ServiceDay.None)
                    {
                        var sched = await LoadRouteNames(Schedule.GetSchedule(SelectedDayGroup.DayGroup));
                        bool military = SettingsManagerBase.Instance?.GetSetting("MilitaryTime", false, false) ?? false;
                        SelectedDayGroup.Schedule.AddRange(sched.GroupBy(a => $"{a.Item2} to {a.Item1.Destination}").Select(g => new RouteScheduleGroup() { RouteAndDestination = g.Key, Times = g.Select(t => new Tuple<string, bool>(GetDisplayTimeForArrival(t.Item1, military), military ? false : (t.Item1.ScheduledDepartureTime.Hour >= 12))) }));
                        SelectedDayGroup.ScheduleLoaded = true;
                    }
                }
                SelectedSchedule.ReplaceRange(SelectedDayGroup.Schedule);
            }
            else
                SelectedSchedule.Clear();
            IsBusy = false;
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
                var oldStop = _Stop;
                SetProperty(ref _Stop, value);
                if (_Stop != oldStop)
                {
                    CancelTasks();
                    LoadSchedule();
                }
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
                else
                {
                    Subtitle = null;
                    var retrievedSchedule = await DataSource.GetScheduleForStopAsync(Stop.ID, DataSourcePreference.All, TokenSource.Token);
                    if (retrievedSchedule.HasData)
                    {
                        Schedule = retrievedSchedule.Data;
                        DayGroups.Clear();
                        var groups = Schedule.GetScheduleGroups().ToArray();
                        foreach (var group in groups)
                        {
                            DayGroups.Add(new DayScheduleGroup(group));
                        }
                        //if (groups.Length > 0)
                        //    DayGroups.AddRange(groups);
                        HasSchedule = true;
                        var today = DateTime.Today.DayOfWeek.ToServiceDay();
                        var todaysGroup = DayGroups.FirstOrDefault(dg => (dg.DayGroup & today) == today);
                        if (todaysGroup == null)
                            SelectedDayGroup = DayGroups[0];
                        else
                            SelectedDayGroup = todaysGroup;
                    }
                    //LoadSelectedSchedule();
                }
            }
            catch (OperationCanceledException)
            {
                Subtitle = "Schedule retrieval was cancelled.";
                HasSchedule = false;
            }
            catch (Exception ex)
            {
                Subtitle = "Error has occured: " + ex.Message;
                HasSchedule = false;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
