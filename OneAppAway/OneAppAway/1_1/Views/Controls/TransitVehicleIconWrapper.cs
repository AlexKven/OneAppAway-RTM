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
    public class TransitVehicleIconWrapper
    {
        public MapIcon Element { get; }

        public TransitVehicleIconWrapper()
        {
            Element = new MapIcon { Visible = false, NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 0.5), CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible, Image = RandomAccessStreamReference.CreateFromUri(new Uri($"ms-appx:///Assets/Icons/TemporaryGreenBus.png")), ZIndex = 6 };
        }

        private RealTimeArrival _Arrival;
        public RealTimeArrival Arrival
        {
            get { return _Arrival; }
            set
            {
                _Arrival = value;
                Element.Visible = Arrival.PotentialVehicleLocation.HasValue;
                if (Arrival.PotentialVehicleLocation.HasValue)
                {
                    Element.Location = Arrival.PotentialVehicleLocation.Value.ToGeopoint();
                }
                Element.Title = Arrival.RouteName;
            }
        }
    }
}
