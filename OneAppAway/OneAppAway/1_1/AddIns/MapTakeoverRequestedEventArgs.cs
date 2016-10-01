using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.AddIns
{
    public class MapTakeoverRequestedEventArgs : EventArgs
    {
        public MapTakeoverRequestedEventArgs() : this(null) { }

        public MapTakeoverRequestedEventArgs(MapTakeover takeover)
        {
            Takeover = takeover;
        }

        public MapTakeover Takeover { get; }
    }
}
