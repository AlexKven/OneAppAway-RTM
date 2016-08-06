using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneAppAway._1_1.Data;
using Windows.Devices.Geolocation;

namespace OneAppAway._1_1
{
    public static class Helpers
    {
        public static BasicGeoposition ToBasicGeoposition(this LatLon value)
        {
            return new BasicGeoposition() { Latitude = value.Latitude, Longitude = value.Longitude };
        }

        public static Geopoint ToGeopoint(this LatLon value)
        {
            return new Geopoint(value.ToBasicGeoposition());
        }

        public static LatLon ToLatLon(this BasicGeoposition value)
        {
            return new LatLon(value.Latitude, value.Longitude);
        }

        public static LatLon ToLatLon(this Geopoint value)
        {
            return value.Position.ToLatLon();
        }
    }
}
