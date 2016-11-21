using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.Data
{
    public struct VehicleDetail
    {
        public string VehicleNumber { get; set; }
        public string VehicleName { get; set; }
        public string AgencyFunded { get; set; }
        public string AgencyOperated { get; set; }
        public string SeatedCapacity { get; set; }
        public Uri ImageUri { get; set; }
    }
}
