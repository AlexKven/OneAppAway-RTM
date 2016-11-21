using OneAppAway._1_1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OneAppAway._1_1.Data.ServiceDay;
using static System.Math;

namespace OneAppAway._1_1.Helpers
{
    public static class GeneralExtensionMethods
    {
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
            else if (day.HasFlag(Weekdays))
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
            else if (day.HasFlag(ReducedWeekday))
            {
                return "Reduced Weekday";
            }
            else
            {
                return "None";
            }
        }

        public static ServiceDay ToServiceDay(this DayOfWeek day)
        {
            return (ServiceDay)((int)Pow(2, ((int)day + 6) % 7));
        }

        public static ServiceDay GetServiceDay(this DateTime date)
        {
            return (date - TimeSpan.FromHours(4)).DayOfWeek.ToServiceDay();
        }
    }
}
