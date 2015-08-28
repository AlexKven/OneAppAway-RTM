using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway
{
    [DataContract]
    public struct ContextLocation
    {
        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longitude { get; set; }
        [DataMember]
        public string City { get; set; }

        public static double Distance(ContextLocation start, ContextLocation end)
        {
            return HelperFunctions.GetDistanceBetweenPoints(start.Latitude, start.Longitude, end.Latitude, end.Longitude);
        }
    }
}
