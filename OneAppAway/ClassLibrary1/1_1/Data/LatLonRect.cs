using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.Data
{
    public struct LatLonRect
    {
        public LatLon NE { get; }
        public LatLon SW { get; }
        public LatLon Span => NE - SW;
        public LatLon Center => SW + Span / 2;

        public bool IsNotAnArea => NE.IsNotALocation || SW.IsNotALocation;
        public static LatLonRect NotAnArea => new LatLonRect(LatLon.NotALocation, LatLon.NotALocation);

        public LatLonRect(LatLon ne, LatLon sw)
        {
            NE = ne;
            SW = sw;
        }

        public LatLonRect(double neLat, double neLong, double swLat, double swLong)
            : this(new LatLon(neLat, neLong), new LatLon(swLat, swLong)) { }

        public static LatLonRect FromPointAndSpan(LatLon ne, LatLon size)
        {
            return new LatLonRect(ne, ne + size);
        }

        public static LatLonRect FromPointAndSpan(double neLat, double neLong, double spanLat, double spanLong)
        {
            return FromPointAndSpan(new LatLon(neLat, neLong), new LatLon(spanLat, spanLong));
        }

        public static LatLonRect FromNWSE(LatLon nw, LatLon se) => new LatLonRect(nw.Latitude, se.Longitude, se.Latitude, nw.Longitude);

        public override string ToString()
        {
            return $"{NE.Latitude}°, {NE.Longitude}°, {SW.Latitude}°, {SW.Longitude}°";
        }

        public string ToString(string format)
        {
            return $"{NE.Latitude.ToString(format)}°, {NE.Longitude.ToString(format)}°, {SW.Latitude.ToString(format)}°, {SW.Longitude.ToString(format)}°";
        }

        public static bool TryParse(string str, out LatLon result)
        {
            result = new LatLon();
            if (str == null)
                return false;
            var parts = str.Split(',');
            if (parts.Length != 2)
                return false;
            parts = parts.Select(part => part.Trim(' ', '°')).ToArray();
            double lat;
            double lon;
            if (!double.TryParse(parts[0], out lat))
                return false;
            if (!double.TryParse(parts[1], out lon))
                return false;
            result = new LatLon(lat, lon);
            return true;
        }

        public static LatLon Parse(string str)
        {
            LatLon result;
            if (!TryParse(str, out result))
                throw new ArgumentException($"{str} is not a valid LatLonRect.");
            return result;
        }

        public override bool Equals(Object obj)
        {
            return obj is LatLonRect && this == (LatLonRect)obj;
        }
        public override int GetHashCode()
        {
            return NE.GetHashCode() ^ SW.GetHashCode();
        }

        public bool Equals(LatLonRect other)
        {
            return this == other;
        }

        public static bool operator ==(LatLonRect left, LatLonRect right)
        {
            return left.NE == right.NE && left.SW == right.SW;
        }
        public static bool operator !=(LatLonRect x, LatLonRect y)
        {
            return !(x == y);
        }

        public LatLonRect ApplySubset(RectSubset subset)
        {
            var span = Span;
            var width = span.Longitude;
            var height = span.Latitude;
            double left;
            double right;
            subset.Apply(ref width, ref height, out left, out right);
            return FromPointAndSpan(NE + new LatLon(left, right), new LatLon(height, width));
        }
    }
}
