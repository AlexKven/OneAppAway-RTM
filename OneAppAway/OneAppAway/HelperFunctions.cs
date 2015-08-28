﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using static OneAppAway.ServiceDay;
using static System.Math;

namespace OneAppAway
{
    public static class HelperFunctions
    {
        public static GeoboundingBox Intersect(this GeoboundingBox bound1, params GeoboundingBox[] boundn)
        {
            Rect result = bound1.ToRect();
            foreach (GeoboundingBox bound in boundn)
            {
                result.Intersect(bound.ToRect());
            }
            return result.ToGeoboundingBox();
        }

        public static Rect ToRect(this GeoboundingBox bounds)
        {
            return new Rect(new Point(bounds.NorthwestCorner.Longitude, bounds.SoutheastCorner.Latitude), new Point(bounds.SoutheastCorner.Longitude, bounds.NorthwestCorner.Latitude));
        }

        public static GeoboundingBox ToGeoboundingBox(this Rect bounds)
        {
            return new GeoboundingBox(new BasicGeoposition() { Latitude = bounds.Bottom, Longitude = bounds.Left }, new BasicGeoposition() { Latitude = bounds.Top, Longitude = bounds.Right });
        }
        
        public static List<T> AllChildrenOfType<T>(DependencyObject parent) where T : FrameworkElement
        {
            var _List = new List<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var _Child = VisualTreeHelper.GetChild(parent, i);
                if (_Child is T)
                {
                    _List.Add(_Child as T);
                }
                _List.AddRange(AllChildrenOfType<T>(_Child));
            }
            return _List;
        }

        public static T FindControl<T>(DependencyObject parentContainer, string controlName) where T : FrameworkElement
        {
            var childControls = AllChildrenOfType<T>(parentContainer);
            var control = childControls.Where(x => x.Name.Equals(controlName)).Cast<T>().First();
            return control;
        }

        public static string GetFriendlyName(this ServiceDay day) => GetFriendlyName(day, false);

        private static string GetFriendlyName(this ServiceDay day, bool rec)
        {
            if (day == All)
                return "Every Day";
            else if (day == (Weekdays | Weekends))
                return "Every Day*";
            else if (day.HasFlag(Saturday) && day.HasFlag(Sunday))
            {
                ServiceDay others = day & ~Saturday & ~Sunday;
                if (others == None)
                    return "Weekends";
                else
                    return "Weekends, " + others.GetFriendlyName(true);
            }
            else if (day.HasFlag(Saturday))
            {
                ServiceDay others = day & ~Saturday;
                if (others == None)
                    return "Saturday";
                else
                    return "Sat, " + others.GetFriendlyName(true);
            }
            else if (day.HasFlag(Sunday))
            {
                ServiceDay others = day & ~Sunday;
                if (others == None)
                    return "Sunday";
                else
                    return "Sun, " + others.GetFriendlyName(true);
            }
            else  if (day.HasFlag(Weekdays))
            {
                string result = "Weekdays";
                ServiceDay others = day & ~Weekdays;
                if (others.HasFlag(ReducedWeekday))
                    others = others & ~ReducedWeekday;
                else
                    result += "*";
                if (others == None)
                    return result;
                else
                    return result + ", " + others.GetFriendlyName(true);
            }
            else if (day.HasFlag(Monday))
            {
                bool reduced = day.HasFlag(ReducedWeekday);
                ServiceDay others = day & ~Monday;
                if (reduced)
                    others = others & ~ReducedWeekday;
                if (others == None)
                    return rec ? reduced ? "Mon" : "Mon*" : reduced ? "Monday" : "Monday*";
                else
                    return (reduced ? "Mon" : "Mon*") + ", " + others.GetFriendlyName(true);
            }
            else if (day.HasFlag(Tuesday))
            {
                ServiceDay others = day & ~Tuesday;
                if (others == None)
                    return rec ? "Tue" : "Tuesday";
                else
                    return "Tue, " + others.GetFriendlyName(true);
            }
            else if (day.HasFlag(Wednesday))
            {
                ServiceDay others = day & ~Wednesday;
                if (others == None)
                    return rec ? "Wed" : "Wednesday";
                else
                    return "Wed, " + others.GetFriendlyName(true);
            }
            else if (day.HasFlag(Thursday))
            {
                ServiceDay others = day & ~Thursday;
                if (others == None)
                    return rec ? "Thu" : "Thursday";
                else
                    return "Thu, " + others.GetFriendlyName(true);
            }
            else if (day.HasFlag(Friday))
            {
                ServiceDay others = day & ~Friday;
                if (others == None)
                    return rec ? "Fri" : "Friday";
                else
                    return "Fri, " + others.GetFriendlyName(true);
            }
            else
            {
                return "None";
            }
        }

        public static BasicGeoposition[] DecodeShape(string encoded)
        {
            List<BasicGeoposition> result = new List<BasicGeoposition>();
            int index = 0;
            int latitude = 0;
            int longitude = 0;

            int length = encoded.Length;
            List<Point> pointList = new List<Point>();

            while (index < length)
            {
                latitude += DecodePoint(encoded, index, out index);
                longitude += DecodePoint(encoded, index, out index);

                BasicGeoposition point = new BasicGeoposition();
                point.Latitude = (latitude * 1e-5);
                point.Longitude = (longitude * 1e-5);

                result.Add(point);
            }

            return result.ToArray();
        }

        private static int DecodePoint(string encoded, int startindex, out int finishindex)
        {
            int b;
            int shift = 0;
            int result = 0;

            //magic google algorithm, see http://code.google.com/apis/maps/documentation/polylinealgorithm.html
            do
            {
                b = Convert.ToInt32(encoded[startindex++]) - 63;
                result |= (b & 0x1f) << shift;
                shift += 5;
            } while (b >= 0x20);
            //if negative flip
            int dlat = (((result & 1) > 0) ? ~(result >> 1) : (result >> 1));

            //set output index
            finishindex = startindex;

            return dlat;
        }

        public static DateTime? DateForServiceDay(ServiceDay day)
        {
            DateTime baseDate = DateTime.Today;// - TimeSpan.FromDays(7);
            while (baseDate.DayOfWeek != DayOfWeek.Monday)
                baseDate += TimeSpan.FromDays(1);
            if (day == ServiceDay.ReducedWeekday)
                return baseDate;
            double div = (double)(int)day;
            int log = 0;
            while (div > 1)
            {
                div /= 2;
                log++;
            }
            if (div == 1)
            {
                return baseDate + TimeSpan.FromDays(log);
            }
            return null;
        }

        public static ServiceDay GetServiceDay(this DateTime date)
        {
            return (ServiceDay)Enum.Parse(typeof(ServiceDay), date.DayOfWeek.ToString());
        }

        public static DataSourceDescriptor Invert(this DataSourceDescriptor original)
        {
            return original == DataSourceDescriptor.Cloud ? DataSourceDescriptor.Local : DataSourceDescriptor.Cloud;
        }

        public static bool ContainsPoint(this GeoboundingBox box, BasicGeoposition point) => point.Latitude < box.NorthwestCorner.Latitude && point.Latitude >= box.SoutheastCorner.Latitude && point.Longitude < box.SoutheastCorner.Longitude && point.Longitude >= box.NorthwestCorner.Longitude;

        public static DateTime? GetDateTimeFromEpochTime(this string epochTime)
        {
            if (epochTime == null) return null;
            long milliseconds;
            if (!long.TryParse(epochTime, out milliseconds))
                return null;
            return (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromMilliseconds(milliseconds)).ToLocalTime();
        }

        public static short GetShortTime(this DateTime time)
        {
            return (short)(60 * time.Hour + time.Minute);
        }

        public static double GetDistanceBetweenPoints(double lat1, double long1, double lat2, double long2)
        {
            //Code by beachwalker on StackOverflow
            //http://stackoverflow.com/questions/10175724/calculate-distance-between-two-points-in-bing-maps
            double distance = 0;

            double dLat = (lat2 - lat1) / 180 * Math.PI;
            double dLong = (long2 - long1) / 180 * Math.PI;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                        + Math.Cos(lat1 / 180 * Math.PI) * Math.Cos(lat2 / 180 * Math.PI)
                        * Math.Sin(dLong / 2) * Math.Sin(dLong / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            //Calculate radius of earth
            // For this you can assume any of the two points.
            double radiusE = 6378135; // Equatorial radius, in metres
            double radiusP = 6356750; // Polar Radius

            //Numerator part of function
            double nr = Math.Pow(radiusE * radiusP * Math.Cos(lat1 / 180 * Math.PI), 2);
            //Denominator part of the function
            double dr = Math.Pow(radiusE * Math.Cos(lat1 / 180 * Math.PI), 2)
                            + Math.Pow(radiusP * Math.Sin(lat1 / 180 * Math.PI), 2);
            double radius = Math.Sqrt(nr / dr);

            //Calculate distance in meters.
            distance = radius * c;
            return distance; // distance in meters
        }
    }
}
