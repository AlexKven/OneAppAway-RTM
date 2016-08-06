using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.Data
{
    public struct LatLon
    {
        public static LatLon Seattle => new LatLon(47.6062100, -122.3320700);

        public double Latitude { get; }
        public double Longitude { get; }

        public bool IsZero => Latitude == 0 && Longitude == 0;

        public LatLon(double latitude, double longitude)
        {
            while (latitude < -180)
                latitude += 360;
            while (latitude >= 180)
                latitude -= 360;
            if (latitude < -90)
            {
                latitude = -180 - latitude;
                longitude += 180;
            }
            if (latitude >= 90)
            {
                latitude = 180 - latitude;
                longitude += 180;
            }
            while (longitude < -180)
                longitude += 360;
            while (longitude >= 180)
                longitude -= 360;
            Latitude = latitude;
            Longitude = longitude;
        }

        public static LatLon operator -(LatLon value)
        {
            return new LatLon(-value.Latitude, -value.Longitude);
        }

        public static LatLon operator +(LatLon left, LatLon right)
        {
            return new LatLon(left.Latitude + right.Latitude, left.Longitude + right.Longitude);
        }

        public static LatLon operator -(LatLon left, LatLon right)
        {
            return left +-right;
        }
    }
}
