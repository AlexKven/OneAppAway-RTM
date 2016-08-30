using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.Data
{
    public struct TransitRoute
    {
        private static TransitRouteSqlProvider _SqlProvider = new TransitRouteSqlProvider();
        public static TransitRouteSqlProvider SqlProvider => _SqlProvider;

        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Agency { get; set; }
        public string Provider { get; set; }
        public string ProviderID { get; set; }
    }

    //public struct BusRoute
    //{
    //    public string ID { get; set; }

    //    public string Name { get; set; }

    //    public string Description { get; set; }

    //    public string Agency { get; set; }

    //    public static bool operator ==(BusRoute lhs, BusRoute rhs)
    //    {
    //        return lhs.ID == rhs.ID;
    //    }

    //    public static bool operator !=(BusRoute lhs, BusRoute rhs)
    //    {
    //        return !(lhs == rhs);
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        if (obj == null) return false;
    //        if (!(obj is BusRoute)) return false;
    //        return this == (BusRoute)obj;
    //    }

    //    public override int GetHashCode()
    //    {
    //        return ID.GetHashCode();
    //    }
    //}
}
