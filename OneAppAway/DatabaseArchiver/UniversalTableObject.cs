using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseArchiver
{
    public class UniversalTableObject
    {
        public string RouteID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string AgencyID { get; set; }

        public string ShapeID { get; set; }

        public string URL { get; set; }

        public object ConvertToSpecificObject()
        {
            if (RouteID != null)
                return new Route(RouteID, Name, Description, AgencyID, ShapeID);
            else if (AgencyID != null)
                return new Agency(AgencyID, Name, URL);
            else
                return new object();
        }
    }
}
