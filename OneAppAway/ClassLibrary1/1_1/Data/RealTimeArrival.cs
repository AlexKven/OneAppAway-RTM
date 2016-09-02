using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.Data
{
    public struct RealTimeArrival
    {
        public string Route { get; set; }
        public string Trip { get; set; }
        public string Stop { get; set; }
        public string RouteName { get; set; }
        //public string RouteLongName { get; set; }
        public DateTime? ScheduledArrivalTime { get; set; }
        public DateTime? PredictedArrivalTime { get; set; }
        public string Destination { get; set; }
        public string[] Alerts { get; set; }
        public AlertStatus Status { get; set; }
        public double? FrequencyMinutes { get; set; }
        public LatLon? PotentialVehicleLocation { get; set; }
        public LatLon? KnownVehicleLocation { get; set; }
        public double? Orientation { get; set; }
        public double DegreeOfConfidence { get; set; }

        //public int MinutesLate
        //{
        //    get
        //    {
        //        return (int)(PredictedArrivalTime.Value - ScheduledArrivalTime).TotalMinutes;
        //    }
        //}

        //public string TimelinessDescription
        //{
        //    get
        //    {
        //        if (PredictedArrivalTime == null) return "Unknown";
        //        int late = MinutesLate;
        //        if (late == 0)
        //            return "On Time";
        //        else if (late > 0)
        //            return late.ToString() + "m Late";
        //        else
        //            return (-late).ToString() + "m Early";
        //    }
        //}
    }
}
