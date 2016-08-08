//*dbusing SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseArchiver
{
    public class Route
    {
        public Route() { }
        public Route(string routeID, string name, string description, string agencyID, string shapeID)
        {
            RouteID = routeID;
            Name = name;
            Description = description;
            AgencyID = agencyID;
            ShapeID = shapeID;
        }

        //*db[PrimaryKey, NotNull]
        public string RouteID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string AgencyID { get; set; }

        public string ShapeID { get; set; }

        public override string ToString()
        {
            return "Route(" + RouteID + ", " + Name + ", " + Description + ", " + AgencyID + ", " + ShapeID + ")";
        }
    }
}
