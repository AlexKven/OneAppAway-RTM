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
        private WeekSchedule Schedule;
        private CancellationTokenSource TokenSource;

        public ScheduleControlViewModel()
        {
            TokenSource = new CancellationTokenSource();
        }

        public ObservableRangeCollection<ServiceDay> DayGroups { get; } = new ObservableRangeCollection<ServiceDay>();

        private bool _HasSchedule = false;
        public bool HasSchedule
        {
            get { return _HasSchedule; }
            set { SetProperty(ref _HasSchedule, false); }
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
                    DayGroups.AddRange(Schedule.GetScheduleGroups());
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
