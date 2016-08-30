using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace OneAppAway._1_1.Data
{
    public struct LatLonRect : IEquatable<LatLonRect>
    {
        public LatLon NE { get; }
        public LatLon SW { get; }
        public LatLon Span => NE - SW;
        public LatLon Center => SW + Span / 2;
        public LatLon NW => new LatLon(NE.Latitude, SW.Longitude);
        public LatLon SE => new LatLon(SW.Latitude, NE.Longitude);

        public bool IsNotAnArea => NE.IsNotALocation || SW.IsNotALocation;
        public static LatLonRect NotAnArea => new LatLonRect(LatLon.NotALocation, LatLon.NotALocation);

        public LatLonRect(LatLon ne, LatLon sw)
        {
            NE = ne;
            SW = sw;
        }

        public LatLonRect(double neLat, double neLong, double swLat, double swLong)
            : this(new LatLon(neLat, neLong), new LatLon(swLat, swLong)) { }

        public static LatLonRect FromPointAndSpan(LatLon sw, LatLon span)
        {
            return new LatLonRect(sw + span, sw);
        }

        public static LatLonRect FromPointAndSpan(double swLat, double swLong, double spanLat, double spanLong)
        {
            return FromPointAndSpan(new LatLon(swLat, swLong), new LatLon(spanLat, spanLong));
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

        public static bool TryParse(string str, out LatLonRect result)
        {
            result = new LatLonRect();
            if (str == null)
                return false;
            var parts = str.Split(',');
            if (parts.Length != 4)
                return false;
            parts = parts.Select(part => part.Trim(' ', '°')).ToArray();
            double neLat, neLon, swLat, swLon;
            if (!double.TryParse(parts[0], out neLat))
                return false;
            if (!double.TryParse(parts[1], out neLon))
                return false;
            if (!double.TryParse(parts[2], out swLat))
                return false;
            if (!double.TryParse(parts[3], out swLon))
                return false;
            result = new LatLonRect(neLat, neLon, swLat, swLon);
            return true;
        }

        public static LatLonRect Parse(string str)
        {
            LatLonRect result;
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
            double top;
            subset.Apply(ref width, ref height, out left, out top);
            var nw = NW + new LatLon(-top, left);
            return FromNWSE(nw, nw + new LatLon(-height, width));
            //return FromPointAndSpan(SW + new LatLon(top, left), new LatLon(height, width));
        }

        public bool ContainsLatLon(LatLon point) => point.Latitude >= SW.Latitude && point.Latitude <= NE.Latitude && point.Longitude >= SW.Longitude && point.Longitude <= NE.Longitude;

        public IEnumerable<LatLonRect> Miniaturize(LatLon maxSpan)
        {
            return new LatLonRectMiniaturizationEnumerable(this, maxSpan);
        }

        public IEnumerable<LatLonRect> GetNewRegion(LatLonRect movedTo)
        {
            if (IsNotAnArea)
            {
                yield return movedTo;
                yield break;
            }
            double northExt = movedTo.NE.Latitude - NE.Latitude;
            double eastExt = movedTo.NE.Longitude - NE.Longitude;
            double southExt = SW.Latitude - movedTo.SW.Latitude;
            double westExt = SW.Longitude - movedTo.SW.Longitude;
            if (northExt > 0)
                yield return new LatLonRect(NE.Latitude + northExt, Min(NE.Longitude, movedTo.NE.Longitude), NE.Latitude, Max(SW.Longitude, movedTo.SW.Longitude));
            if (eastExt > 0)
                yield return new LatLonRect(Min(NE.Latitude, movedTo.NE.Latitude), NE.Longitude + eastExt, Max(SW.Latitude, movedTo.SW.Latitude), NE.Longitude);
            if (southExt > 0)
                yield return new LatLonRect(SW.Latitude, Min(NE.Longitude, movedTo.NE.Longitude), SW.Latitude - southExt, Max(SW.Longitude, movedTo.SW.Longitude));
            if (westExt > 0)
                yield return new LatLonRect(Min(NE.Latitude, movedTo.NE.Latitude), SW.Longitude, Max(SW.Latitude, movedTo.SW.Latitude), SW.Longitude - westExt);
            if (northExt > 0 && eastExt > 0)
                yield return new LatLonRect(NE + new LatLon(northExt, eastExt), NE);
            if (southExt > 0 && eastExt > 0)
                yield return LatLonRect.FromNWSE(SE, SE + new LatLon(-southExt, eastExt));
            if (southExt > 0 && westExt > 0)
                yield return new LatLonRect(SW, SW - new LatLon(southExt, westExt));
            if (northExt > 0 && westExt > 0)
                yield return LatLonRect.FromNWSE(NW + new LatLon(northExt, -westExt), NW);
        }
    }

    internal class LatLonRectMiniaturizationEnumerable : IEnumerable<LatLonRect>
    {
        private LatLonRect FullRect;
        private LatLon MaxSpan;

        public LatLonRectMiniaturizationEnumerable(LatLonRect fullRect, LatLon maxSpan)
        {
            FullRect = fullRect;
            MaxSpan = maxSpan;
        }

        public IEnumerator<LatLonRect> GetEnumerator()
        {
            return new LatLonRectMiniaturizationEnumerator(FullRect, MaxSpan);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal class LatLonRectMiniaturizationEnumerator : IEnumerator<LatLonRect>
    {
        private LatLon BasePoint;
        private LatLon DeltaSpan;
        private int TotalRows;
        private int TotalColumns;
        private int CurRow = -1;
        private int CurColumn = -1;

        public LatLonRectMiniaturizationEnumerator(LatLonRect fullRect, LatLon maxSpan)
        {
            BasePoint = fullRect.SW;
            var span = fullRect.Span;
            TotalRows = (int)Ceiling(Abs(span.Latitude) / maxSpan.Latitude);
            TotalColumns = (int)Ceiling(Abs(span.Longitude) / maxSpan.Longitude);
            DeltaSpan = new LatLon(span.Latitude / TotalRows, span.Longitude / TotalColumns);
        }

        public LatLonRect Current
        {
            get
            {
                if (CurRow < 0 || CurColumn < 0 || CurRow >= TotalRows || CurColumn >= TotalColumns)
                    throw new InvalidOperationException("Enumeration has either not started or has moved past the end.");
                return LatLonRect.FromPointAndSpan(new LatLon(BasePoint.Latitude + DeltaSpan.Latitude * CurRow, BasePoint.Longitude + DeltaSpan.Longitude * CurColumn), DeltaSpan);
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public void Dispose() { }

        public bool MoveNext()
        {
            if (CurRow == -1)
            {
                CurRow = 0;
                CurColumn = 0;
            }
            else
            {
                CurRow++;
                if (CurRow == TotalRows)
                {
                    CurRow = 0;
                    CurColumn++;
                    if (CurColumn == TotalColumns)
                        return false;
                }
            }
            return true;
        }

        public void Reset()
        {
            CurRow = -1;
            CurColumn = -1;
        }
    }
}
