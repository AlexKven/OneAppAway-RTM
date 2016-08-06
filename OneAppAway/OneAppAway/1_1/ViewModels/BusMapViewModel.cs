using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace OneAppAway._1_1.ViewModels
{
    public class BusMapViewModel : LightweightViewModelBase
    {
        private double _ZoomLevel = 20;
        public double ZoomLevel
        {
            get { return _ZoomLevel; }
            set
            {
                _ZoomLevel = value;
                OnPropertyChanged();
            }
        }

        private Geopoint _Center = new Geopoint(new BasicGeoposition() { Latitude = 47.6062100, Longitude = -122.3320700 });
        public Geopoint Center
        {
            get { return _Center; }
            set
            {
                _Center = value;
                OnPropertyChanged();
            }
        }
    }
}
