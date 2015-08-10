using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OneAppAway
{
    public sealed partial class BusArrivalBox : UserControl
    {
        public BusArrivalBox()
        {
            this.InitializeComponent();
        }

        private RealtimeArrival _Arrival;

        public RealtimeArrival Arrival
        {
            get { return _Arrival; }
            set
            {
                _Arrival = value;
                bool longForm = value.RouteName.Length > 8;
                NumberColumn.Width = new GridLength(longForm ? 10 : 45);
                LongNameRow.Height = longForm ? GridLength.Auto : new GridLength(0);
                RouteNumberBlock.Text = value.RouteName;
                Grid.SetColumnSpan(RouteNumberBlock, longForm ? 3 : 1);
                Grid.SetRowSpan(RouteNumberBlock, longForm ? 1 : 2);
                Grid.SetColumn(RouteNumberBlock, longForm ? 1 : 0);
                Grid.SetRow(RouteNumberBlock, longForm ? 0 : 1);
                if (longForm)
                {
                    RouteNumberBlock.FontSize = 24;
                }
                else
                {
                    switch (value.RouteName.Length)
                    {
                        case 1:
                            RouteNumberBlock.FontSize = 27;
                            break;
                        case 2:
                            RouteNumberBlock.FontSize = 24;
                            break;
                        case 3:
                            RouteNumberBlock.FontSize = 20;
                            break;
                        case 4:
                            RouteNumberBlock.FontSize = 18;
                            break;
                        case 5:
                            RouteNumberBlock.FontSize = 17;
                            break;
                        case 6:
                            RouteNumberBlock.FontSize = 14;
                            break;
                        default:
                            RouteNumberBlock.FontSize = 13;
                            break;
                    }
                }
                ScheduledTimeBlock.Text = "(sched. " + value.ScheduledArrivalTime.ToString("h:mm") + ")";
                PredictedTimeBlock.Text = value.PredictedArrivalTime == null ? "Unknown" : (value.PredictedArrivalTime.Value.ToString("h:mm") + ", " + value.TimelinessDescription);
                MinutesAwayBlock.SetBinding(TextBlock.TextProperty, new Binding() { Mode = BindingMode.OneWay, Source = App.Current.Resources["Settings"], Path = new PropertyPath("Now"), Converter = (IValueConverter)App.Current.Resources["MinutesUntilArrivalConverter"], ConverterParameter = value.PredictedArrivalTime == null ? value.ScheduledArrivalTime : value.PredictedArrivalTime.Value });
                //MinutesAwayBlock.Text = value.PredictedArrivalTime == null ? (value.ScheduledArrivalTime - DateTime.Now).TotalMinutes.ToString("F0") : (value.PredictedArrivalTime.Value - DateTime.Now).TotalMinutes.ToString("F0");
                
                if (value.PredictedArrivalTime == null)
                {
                    MinutesAwayBlock.Foreground = new SolidColorBrush(Colors.White);
                }
                else if (value.MinutesLate < 0)
                {
                    MinutesAwayBlock.Foreground = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    Binding binding = new Binding() { Source = PredictedTimeBlock, Path = new PropertyPath("Foreground") };
                    MinutesAwayBlock.SetBinding(TextBlock.ForegroundProperty, binding);
                }
                DestinationBlock.Text = value.Destination;
            }
        }
    }
}
