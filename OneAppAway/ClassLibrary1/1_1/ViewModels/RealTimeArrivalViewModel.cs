using MvvmHelpers;
using OneAppAway._1_1.Converters;
using OneAppAway._1_1.Data;
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
        internal static List<WeakReference<RealTimeArrivalViewModel>> Instances = new List<WeakReference<RealTimeArrivalViewModel>>();

        static RealTimeArrivalViewModel()
        {
            TimeDetails.Instance.RegisterPropertyChangedCallback(TimeDetails.NowProperty, (s, e) =>
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
        }

        public RealTimeArrival Arrival { get; }

        public RealTimeArrivalViewModel(RealTimeArrival arrival)
        {
            Instances.Add(new WeakReference<RealTimeArrivalViewModel>(this));
            Arrival = arrival;
            RouteName = arrival.RouteName;
            RouteDestination = arrival.Destination;
            DegreeOfConfidence = arrival.DegreeOfConfidence;
            IsPredicted = arrival.PredictedArrivalTime.HasValue;

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

        private void RefreshMinutesAway()
        {
            if (Arrival.PredictedArrivalTime == null && Arrival.ScheduledArrivalTime == null)
            {
                MinutesAway = 0;
            }
            else
            {
                MinutesAway = (int)MinutesUntilArrivalConverter.Instance.Convert(TimeDetails.Instance.Now, typeof(int), Arrival.PredictedArrivalTime ?? Arrival.ScheduledArrivalTime.Value, null);
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
        public string RouteName { get; }
        public string RouteDestination { get; }
        public string PredictedArrivalTimeText { get; }
        public string ScheduledArrivalTimeText { get; }
        public bool IsPredicted { get; }
        public double DegreeOfConfidence { get; }

        private int _MinutesAway = 0;
        public int MinutesAway
        {
            get { return _MinutesAway; }
            set { SetProperty(ref _MinutesAway, value); }
        }

    
    }
}
