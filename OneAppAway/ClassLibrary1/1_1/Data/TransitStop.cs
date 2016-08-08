using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.Data
{
    public struct TransitStop
    {
        public StopDirection Direction { get; set; }
        public LatLon Position { get; set; }
        public string Path { get; set; }
        public string ID { get; set; }
        public string Provider { get; set; }
        public string ProviderID { get; set; }
        public string Parent { get; set; }
        public string[] Children { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        //public int LocationType { get; set; }
        public string[] Routes { get; set; }

        //public static bool operator ==(BusStop lhs, BusStop rhs)
        //{
        //    return lhs.ID == rhs.ID;
        //}

        //public static bool operator !=(BusStop lhs, BusStop rhs)
        //{
        //    return !(lhs == rhs);
        //}

        //public override bool Equals(object obj)
        //{
        //    if (obj == null) return false;
        //    if (!(obj is BusStop)) return false;
        //    return this == (BusStop)obj;
        //}

        //public override int GetHashCode()
        //{
        //    return ID.GetHashCode();
        //}
    }
}
