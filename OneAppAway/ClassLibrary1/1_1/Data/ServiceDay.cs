using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.Data
{
    [Flags]
    public enum ServiceDay : byte
    {
        None = 0,
        Monday = 1,
        Tuesday = 2,
        Wednesday = 4,
        Thursday = 8,
        Friday = 16,
        Saturday = 32,
        Sunday = 64,
        ReducedWeekday = 128,
        Weekdays = 31,
        AllWeekdays = 159,
        Weekends = 96,
        All = 255
    }
}
