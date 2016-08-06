using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.Data
{
    public struct RectSubset
    {
        public double Left { get; set; }
        public double Top { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }

        public RectSubsetScale LeftScale { get; set; }
        public RectSubsetScale TopScale { get; set; }
        public RectSubsetScale RightScale { get; set; }
        public RectSubsetScale BottomScale { get; set; }

        public RectSubsetValueType LeftValueType { get; set; }
        public RectSubsetValueType TopValueType { get; set; }
        public RectSubsetValueType RightValueType { get; set; }
        public RectSubsetValueType BottomValueType { get; set; }

        public bool DoesNothing
        {
            get
            {
                return Left == 0 && Top == 0 && Right == 0 && Bottom == 0
                    && LeftValueType == RectSubsetValueType.Margin
                    && TopValueType == RectSubsetValueType.Margin
                    && RightValueType == RectSubsetValueType.Margin
                    && BottomValueType == RectSubsetValueType.Margin;
            }
        }

        private static void ApplyToSide(ref double length, double frontValue, double backValue, bool frontAbsolute, bool backAbsolute, bool frontMargin, bool backMargin, out double start)
        {
            start = 0; //Included because C#'s search for assignment is not very exhaustive
            double end;
            double lengthTrim = 0;
            double originalLength = length;
            if (backMargin && frontMargin)
            {
                if (frontAbsolute)
                {
                    start = frontValue;
                    length -= frontValue;
                }
                if (backAbsolute)
                {
                    end = backValue;
                    length -= backValue;
                }
                if (!frontAbsolute)
                {
                    start = frontValue * length;
                    lengthTrim += start;
                }
                if (!backAbsolute)
                {
                    end = backValue * length;
                    lengthTrim += end;
                }
                length -= lengthTrim;
                lengthTrim = 0;
            }
            else if (frontMargin && !backMargin)
            {
                if (frontAbsolute)
                {
                    start = frontValue;
                    length -= frontValue;
                }
                else
                {
                    start = frontValue * length;
                }
                length = backAbsolute ? backValue : backValue * length;
            }
            else if (!frontMargin && backMargin)
            {
                if (backAbsolute)
                {
                    end = backValue;
                    length -= backValue;
                }
                else
                {
                    end = backValue * length;
                }
                length = frontAbsolute ? frontValue : frontValue * length;
                start = originalLength - end - length;
            }
            else
            {
                length = (frontAbsolute ? frontValue : frontValue * length) + (backAbsolute ? backValue : backValue * length);
                start = (originalLength - length) / 2;
            }
        }

        public void Apply(ref double width, ref double height, out double leftOffset, out double topOffset)
        {
            ApplyToSide(ref width, Left, Right, LeftScale == RectSubsetScale.Absolute, RightScale == RectSubsetScale.Absolute, LeftValueType == RectSubsetValueType.Margin, RightValueType == RectSubsetValueType.Margin, out leftOffset);
            ApplyToSide(ref height, Top, Bottom, TopScale == RectSubsetScale.Absolute, BottomScale == RectSubsetScale.Absolute, TopValueType == RectSubsetValueType.Margin, BottomValueType == RectSubsetValueType.Margin, out topOffset);

        //    double width = rect.Width;
        //    double height = rect.Height;
        //    double? left = subset.LeftValueType == RectSubsetValueType.Margin ? null : new int?(0);
        //    double? top = subset.TopValueType == RectSubsetValueType.Margin ? null : new int?(0);
        //    double? right = subset.RightValueType == RectSubsetValueType.Margin ? null : new int?(0);
        //    double? bottom = subset.BottomValueType == RectSubsetValueType.Margin ? null : new int?(0);
            
        //    if (subset.LeftScale == RectSubsetScale.Absolute && left.HasValue)
        //    {
        //        left += subset.Left;
        //        width -= subset.Left;
        //    }
        //    if (subset.RightScale == RectSubsetScale.Absolute && right.HasValue)
        //    {
        //        right += subset.Right;
        //        width -= subset.Right;
        //    }
        //    if (subset.TopScale == RectSubsetScale.Absolute && top.HasValue)
        //    {
        //        top += subset.Top;
        //        height -= subset.Top;
        //    }
        //    if (subset.BottomScale == RectSubsetScale.Absolute && bottom.HasValue)
        //    {
        //        bottom += subset.Bottom;
        //        height -= subset.Bottom;
        //    }

        //    double relativeLengthChanges = 0;
        //    if (subset.LeftValueType == RectSubsetValueType.Margin)
        //    {
        //        if (subset.RightValueType == RectSubsetValueType.Margin)
        //        {
        //            //Margin, Margin
        //            if (subset.LeftScale == RectSubsetScale.Relative)
        //            {
        //                left = subset.Left * width;
        //                relativeLengthChanges += left.Value;
        //            }
        //            if (subset.RightScale == RectSubsetScale.Relative)
        //            {
        //                right = subset.Right * width;
        //                relativeLengthChanges += right.Value;
        //            }
        //            width -= relativeLengthChanges;
        //            relativeLengthChanges = 0;
        //        }
        //        else
        //        {
        //            //Margin, Length
        //            if (subset.LeftScale == RectSubsetScale.Relative)
        //            {
        //                left = subset.Left * width;
        //                relativeLengthChanges += left.Value;
        //            }
        //            if (subset.RightScale == RectSubsetScale.Relative)
        //            {
                        
        //            }
        //        }
        //    }
        }
    }

    public enum RectSubsetScale
    {
        Absolute, Relative
    }

    public enum RectSubsetValueType
    {
        Margin, Length
    }
}
