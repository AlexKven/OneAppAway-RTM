using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.Views.Structures
{
    public struct TitleBarElementSize
    {
        public TitleBarElementSize(double width, bool prioritize = false)
        {
            RelativeWidth = null;
            MaxWidth = null;
            GuaranteedWidth = width;
            Prioritize = prioritize;
        }

        public TitleBarElementSize(double relativeWidth, double minWidth, bool prioritize = false)
        {
            RelativeWidth = relativeWidth;
            MaxWidth = null;
            GuaranteedWidth = minWidth;
            Prioritize = prioritize;
        }

        public TitleBarElementSize(double relativeWidth, double minWidth, double maxWidth, bool prioritize = false)
        {
            RelativeWidth = relativeWidth;
            MaxWidth = maxWidth;
            GuaranteedWidth = minWidth;
            Prioritize = prioritize;
        }

        public double? RelativeWidth { get; }
        public double GuaranteedWidth { get; }
        public double? MaxWidth { get; }
        public bool Prioritize { get; }
    }
}
