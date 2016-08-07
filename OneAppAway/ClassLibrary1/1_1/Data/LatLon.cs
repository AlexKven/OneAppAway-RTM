using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.Data
{
    public struct LatLon : IEquatable<LatLon>
    {
        public static LatLon Seattle => new LatLon(47.6062100, -122.3320700);

        public double Latitude { get; }
        public double Longitude { get; }

        public bool IsZero => Latitude == 0 && Longitude == 0;
        public bool IsNotALocation
        {
            get
            {
                return double.IsNaN(Latitude) || double.IsNaN(Longitude);
            }
        }

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

        public override string ToString()
        {
            return $"{Latitude}°, {Longitude}°";
        }

        public override bool Equals(Object obj)
        {
            return obj is LatLon && this == (LatLon)obj;
        }
        public override int GetHashCode()
        {
            return Latitude.GetHashCode() ^ Longitude.GetHashCode();
        }

        public bool Equals(LatLon other)
        {
            return this == other;
        }

        public static bool operator ==(LatLon left, LatLon right)
        {
            return left.Latitude == right.Latitude && left.Longitude == right.Longitude;
        }
        public static bool operator !=(LatLon x, LatLon y)
        {
            return !(x == y);
        }
    }
}
