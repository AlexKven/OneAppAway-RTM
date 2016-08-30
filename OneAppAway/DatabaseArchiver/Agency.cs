//*dbusing SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseArchiver
{
    public class Agency
    {
        public Agency() { }
        public Agency(string agencyID, string name, string url)
        {
            AgencyID = agencyID;
            Name = name;
            URL = url;
        }

        //*db[PrimaryKey][NotNull]
        public string AgencyID { get; set; }

        public string Name { get; set; }
        
        public string URL { get; set; }

        public override string ToString()
        {
            return "Agency(" + AgencyID + ", " + Name + ", " + URL + ")";
        }
    }
}
