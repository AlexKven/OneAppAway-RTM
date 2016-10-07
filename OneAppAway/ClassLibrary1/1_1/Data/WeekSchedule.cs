using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.Data
{
    public abstract class WeekSchedule
    {
        public abstract IEnumerable<ServiceDay> GetScheduleGroups();

        public abstract IEnumerable<ScheduledArrival> GetSchedule(ServiceDay day);

        public abstract void Union(WeekSchedule other);
    }
}
