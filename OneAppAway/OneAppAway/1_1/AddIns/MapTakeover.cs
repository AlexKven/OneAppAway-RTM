using OneAppAway._1_1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace OneAppAway._1_1.AddIns
{
    public class MapTakeover
    {
        public MapTakeover(UIElement mapOverlay, RectSubset centerRegionOverride, MapView viewOverride)
        {
            MapOverlay = mapOverlay;
            CenterRegionOverride = centerRegionOverride;
            ViewOverride = viewOverride;
        }

        public UIElement MapOverlay { get; }
        public RectSubset CenterRegionOverride { get; }
        public MapView ViewOverride { get; }
    }
}
