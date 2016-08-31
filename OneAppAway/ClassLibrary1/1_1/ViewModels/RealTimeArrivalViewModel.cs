using MvvmHelpers;
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
    public class RealTimeArrivalViewModel : DependencyObject
    {
        private RealTimeArrival Arrival;

        public RealTimeArrivalViewModel(RealTimeArrival arrival)
        {
            RouteName = arrival.RouteName;
            RouteDestination = arrival.Destination;

            PredictedArrivalTimeText = arrival.PredictedArrivalTime?.ToString("h:mm tt") ?? "Unknown";
            if (arrival.ScheduledArrivalTime == null)
                ScheduledArrivalTimeText = "(Unscheduled)";
            else
                ScheduledArrivalTimeText = $"(Sched. {arrival.ScheduledArrivalTime?.ToString("h:mm tt")})";
            if (arrival.PredictedArrivalTime != null && arrival.ScheduledArrivalTime != null)
            {
                var minsLate = (int)(arrival.PredictedArrivalTime.Value - arrival.ScheduledArrivalTime.Value).TotalMinutes;
                if (minsLate > 0)
                    TimlinessDescription = $"{minsLate}m late";
                else if (minsLate == 0)
                    TimlinessDescription = "on time";
                else
                    TimlinessDescription = $"{-minsLate}m early";
                IsEarly = minsLate < 0;
            }

            if (arrival.PredictedArrivalTime == null && arrival.ScheduledArrivalTime == null)
                MinutesAway = 0;
            else
                BindingOperations.SetBinding(this, MinutesAwayProperty, new Binding() { Mode = BindingMode.OneWay, Source = TimeDetails.Instance, Path = new PropertyPath("Now"), Converter = Converters.MinutesUntilArrivalConverter.Instance, ConverterParameter = arrival.PredictedArrivalTime ?? arrival.ScheduledArrivalTime.Value });
        }

        public bool IsEarly { get; }
        public string TimlinessDescription { get; }
        public bool IsCancelled { get; }
        public bool HasAlert { get; }
        public string RouteName { get; }
        public string RouteDestination { get; }
        public string PredictedArrivalTimeText { get; }
        public string ScheduledArrivalTimeText { get; }

        public int MinutesAway
        {
            get { return (int)GetValue(MinutesAwayProperty); }
            set { SetValue(MinutesAwayProperty, value); }
        }
        public static readonly DependencyProperty MinutesAwayProperty =
            DependencyProperty.Register("MinutesAway", typeof(int), typeof(RealTimeArrivalViewModel), new PropertyMetadata(0));
    }
}
