using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OneAppAway._1_1.Data
{
    public class LocationSearchResult
    {
        public string Name { get; set; }
        public LatLon Location { get; set; }
        public ICommand Command { get; set; }
    }
}
