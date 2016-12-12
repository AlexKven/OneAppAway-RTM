using OneAppAway._1_1.Data;
using OneAppAway._1_1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls.Maps;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.Devices.Geolocation;

namespace OneAppAway._1_1.AddIns
{
    public class CurrentLocationAddIn : TransitMapAddInBase
    {
        private MapIcon LocationIcon = new MapIcon() { Image = RandomAccessStreamReference.CreateFromUri(new Uri($"ms-appx:///Assets/Icons/CurrentLocation.png")), Location = LatLon.Seattle.ToGeopoint(), NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 0.5), ZIndex = 10, Visible = false };

        public CurrentLocationAddIn()
        {
            SetLastKnownLocation();
            MapElementsShown.Add(LocationIcon);
            WeakEventListener<CurrentLocationAddIn, object, PositionChangedEventArgs> positionChangedEventListener = new WeakEventListener<CurrentLocationAddIn, object, PositionChangedEventArgs>(this);
            positionChangedEventListener.OnEventAction = (t, s, e) => t.LocationHelper_LocationChanged(s, e);
            LocationHelper.LocationChanged += positionChangedEventListener.OnEvent;
            WeakEventListener<CurrentLocationAddIn, object, StatusChangedEventArgs> statusChangedEventListener = new WeakEventListener<CurrentLocationAddIn, object, StatusChangedEventArgs>(this);
            statusChangedEventListener.OnEventAction = (t, s, e) => t.LocationHelper_StatusChanged(s, e);
            LocationHelper.StatusChanged += statusChangedEventListener.OnEvent;
        }

        private LatLon _Location = LatLon.Seattle;
        private LatLon Location
        {
            get { return _Location; }
            set
            {
                _Location = value;
                LocationIcon.Location = Location.ToGeopoint();
            }
        }

        private bool _Unsure = false;
        private bool Unsure
        {
            get { return _Unsure; }
            set
            {
                if (value != _Unsure)
                {
                    LocationIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri($"ms-appx:///Assets/Icons/{(value ? "CurrentLocationUnsure" : "CurrentLocation")}.png"));
                }
                _Unsure = value;
            }
        }

        private bool _Available = false;
        public bool Available
        {
            get { return _Available; }
            set
            {
                _Available = value;
                LocationIcon.Visible = Available;
            }
        }

        private void SetLastKnownLocation()
        {
            var loc = LocationHelper.GetLastKnownLocation();
            if (loc.HasValue)
            {
                Location = loc.Value;
            }
            Available = loc.HasValue;
            Unsure = true;
        }

        private void LocationHelper_LocationChanged(object sender, PositionChangedEventArgs e)
        {
            if (!Available)
                Available = true;
            if (Unsure)
                Unsure = false;
            Location = e.Position.Coordinate.Point.ToLatLon();
        }

        private void LocationHelper_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (e.Status == PositionStatus.NotAvailable || e.Status == PositionStatus.NoData || e.Status == PositionStatus.Disabled)
                SetLastKnownLocation();
        }
    }
}
