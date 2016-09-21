using MvvmHelpers;
using OneAppAway._1_1.Abstract;
using OneAppAway._1_1.Converters;
using OneAppAway._1_1.Data;
using OneAppAway.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace OneAppAway._1_1.ViewModels
{
    public class RealTimeArrivalViewModel : BaseViewModel
    {
        #region Static
        internal static List<WeakReference<RealTimeArrivalViewModel>> Instances = new List<WeakReference<RealTimeArrivalViewModel>>();

        private static IntervalExecuterBase _IntervalExecuter;
        public static IntervalExecuterBase IntervalExecuter
        {
            get { return _IntervalExecuter; }
            set
            {
                if (IntervalExecuter != null)
                    IntervalExecuter.DeregisterTask(IntervalExecuterCommand);
                _IntervalExecuter = value;
                if (IntervalExecuter != null)
                    IntervalExecuter.RegisterTask(IntervalExecuterCommand, TimeSpan.FromSeconds(10));
            }
        }

        private static RelayCommand IntervalExecuterCommand = new RelayCommand((obj) =>
        {
            for (int i = 0; i < Instances.Count; i++)
            {
                var instance = Instances[i];
                RealTimeArrivalViewModel reference;
                if (instance.TryGetTarget(out reference))
                {
                    reference.RefreshMinutesAway();
                }
                else
                {
                    Instances.Remove(instance);
                    i--;
                }
            }
        });
        #endregion

        public RealTimeArrival Arrival { get; }

        public RealTimeArrivalViewModel(RealTimeArrival arrival)
        {
            if (arrival.FrequencyMinutes == null)
                Instances.Add(new WeakReference<RealTimeArrivalViewModel>(this));
            HasLongRouteName = arrival.RouteName.Length >= 8;
            Arrival = arrival;
            RouteName = arrival.RouteName;
            RouteDestination = arrival.Destination;
            DegreeOfConfidence = arrival.DegreeOfConfidence;
            IsPredicted = arrival.PredictedArrivalTime.HasValue;
            HasAlert = arrival.Status == AlertStatus.Alert;
            IsCancelled = arrival.Status == AlertStatus.Cancelled;
            IsDropOffOnly = arrival.IsDropOffOnly;
            IsFrequencyBased = arrival.FrequencyMinutes != null;

            if (arrival.FrequencyMinutes == null)
            {
                PredictedArrivalTimeText = arrival.PredictedArrivalTime?.ToString("h:mm tt") ?? "Unknown";
                if (arrival.ScheduledArrivalTime == null)
                    ScheduledArrivalTimeText = "(Unscheduled)";
                else
                    ScheduledArrivalTimeText = $"(Sched. {arrival.ScheduledArrivalTime?.ToString("h:mm")})";
                if (arrival.PredictedArrivalTime != null && arrival.ScheduledArrivalTime != null)
                {
                    var minsLate = (int)(arrival.PredictedArrivalTime.Value - arrival.ScheduledArrivalTime.Value).TotalMinutes;
                    if (minsLate > 0)
                    {
                        PredictedArrivalTimeText += $", {minsLate}m late";
                    }
                    else if (minsLate == 0)
                    {
                        PredictedArrivalTimeText += ", on time";
                    }
                    else
                    {
                        PredictedArrivalTimeText += $", {-minsLate}m early";
                    }
                    IsEarly = minsLate < 0;
                }
                RefreshMinutesAway();
            }
            else
            {
                Frequency = (int)arrival.FrequencyMinutes.Value;
            }
        }

        private void RefreshMinutesAway()
        {
            if (Arrival.PredictedArrivalTime == null && Arrival.ScheduledArrivalTime == null)
            {
                MinutesAway = 0;
            }
            else
            {
                MinutesAway = (int)((Arrival.PredictedArrivalTime ?? Arrival.ScheduledArrivalTime.Value) - DateTime.Now).TotalMinutes;
            }
        }

        private bool? _IsEarly;
        public bool? IsEarly
        {
            get { return _IsEarly; }
            set { SetProperty(ref _IsEarly, value); }
        }
        public bool IsCancelled { get; }
        public bool HasAlert { get; }
        public bool IsDropOffOnly { get; }
        public bool IsFrequencyBased { get; }
        public bool HasLongRouteName { get; }
        public string RouteName { get; }
        public string RouteDestination { get; }
        public string PredictedArrivalTimeText { get; }
        public string ScheduledArrivalTimeText { get; }
        public bool IsPredicted { get; }
        public double DegreeOfConfidence { get; }

        private int _MinutesAway = 0;
        private int Frequency = 0;
        public int MinutesAway
        {
            get { return _MinutesAway; }
            set
            {
                SetProperty(ref _MinutesAway, value);
                OnPropertyChanged("MinutesDisplayed");
            }
        }

        public string MinutesDisplayed
        {
            get
            {
                if (IsFrequencyBased)
                    return $"Every {Frequency} mins";
                else
                    return MinutesAway == 0 ? "NOW" : MinutesAway.ToString();
            }
        }
    }
}
