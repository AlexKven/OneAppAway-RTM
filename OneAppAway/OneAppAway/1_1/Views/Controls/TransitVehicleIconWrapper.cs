using OneAppAway._1_1.Data;
using OneAppAway._1_1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media.Imaging;

namespace OneAppAway._1_1.Views.Controls
{
    public class TransitVehicleIconWrapper : DependencyObject
    {
        public MapIcon Element { get; }

        public TransitVehicleIconWrapper()
        {
            Element = new MapIcon { Visible = false, NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 0.5), CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible, Image = RandomAccessStreamReference.CreateFromUri(new Uri($"ms-appx:///Assets/Icons/BusClosed20.png")), ZIndex = 4 };
        }

        public RealTimeArrival Arrival
        {
            get { return (RealTimeArrival)GetValue(ArrivalProperty); }
            set { SetValue(ArrivalProperty, value); }
        }
        public static readonly DependencyProperty ArrivalProperty =
            DependencyProperty.Register("Arrival", typeof(RealTimeArrival), typeof(TransitVehicleIconWrapper), new PropertyMetadata(new RealTimeArrival()));
        static void OnArrivalChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var typedSender = sender as TransitVehicleIconWrapper;
            var arrival = (RealTimeArrival)e.NewValue;
            if (typedSender != null)
            {
                typedSender.Element.Visible = arrival.KnownVehicleLocation.HasValue;
                if (arrival.KnownVehicleLocation.HasValue)
                {
                    typedSender.Element.Location = typedSender.Arrival.KnownVehicleLocation.Value.ToGeopoint();
                }
            }
        }
    }
}
