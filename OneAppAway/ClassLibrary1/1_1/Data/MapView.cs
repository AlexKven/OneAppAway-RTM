using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.Data
{
    public class MapView
    {
        public MapView(LatLon center)
        {
            Center = center;
        }
        
        public MapView(double zoomLevel)
        {
            ZoomLevel = zoomLevel;
        }

        public MapView(LatLon center, double zoomLevel)
        {
            Center = center;
            ZoomLevel = zoomLevel;
        }

        public MapView(LatLonRect area)
        {
            Area = area;
        }

        public LatLon? Center { get; }
        public double? ZoomLevel { get; }

        public LatLonRect? Area { get; }

        public bool Animate { get; set; } = true;
    }
}
