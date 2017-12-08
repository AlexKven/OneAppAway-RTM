using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebivewTest
{
    //params
    //Trip, Stop, Route, PrevRoute, RouteName, PrevRouteName, ScheduledArrivalTime, PredictedArrivalTime, Vehicle, Destination, FrequencyMinutes, ScheduledVehicleLocation, KnownVehicleLocation, Orientation, Confidence, DropOffOnly
    public struct RealTimeArrival
    {
        public string Route { get; set; }
        public string PrevRoute { get; set; }
        public string Trip { get; set; }
        public string Stop { get; set; }
        public string RouteName { get; set; }
        public string PrevRouteName { get; set; }
        //public string RouteLongName { get; set; }
        public DateTime? ScheduledArrivalTime { get; set; }
        public DateTime? PredictedArrivalTime { get; set; }
        public DateTime? BestKnownArrivalTime => PredictedArrivalTime ?? ScheduledArrivalTime;
        public string Vehicle { get; set; }
        public string Destination { get; set; }
        public string[] Alerts { get; set; }
        //public AlertStatus Status { get; set; }
        public double? FrequencyMinutes { get; set; }
        public LatLon? ScheduledVehicleLocation { get; set; }
        public LatLon? KnownVehicleLocation { get; set; }
        public double? Orientation { get; set; }
        public double DegreeOfConfidence { get; set; }
        public bool IsDropOffOnly { get; set; }

    }
}
