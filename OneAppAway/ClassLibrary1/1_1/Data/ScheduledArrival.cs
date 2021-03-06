﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.Data
{
    public struct ScheduledArrival
    {
        public string Route { get; set; }
        public string Trip { get; set; }
        public string Stop { get; set; }
        public DateTime? ScheduledArrivalTime { get; set; }
        public DateTime ScheduledDepartureTime { get; set; }
        public string Destination { get; set; }
        public string[] Alerts { get; set; }
        public AlertStatus Status { get; set; }
        public bool IsDropOffOnly { get; set; }



        //public override bool Equals(object obj)
        //{
        //    if (obj == null) return false;
        //    if (!(obj is ScheduledArrival)) return false;
        //    return this == (ScheduledArrival)obj;
        //}

        //public override int GetHashCode()
        //{
        //    return (Trip + Stop).GetHashCode();
        //}
    }
}
