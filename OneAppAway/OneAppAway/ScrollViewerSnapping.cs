using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static System.Math;

namespace OneAppAway
{
    public static class ScrollViewerSnapping
    {
        private static Dictionary<ScrollViewer, Tuple<double, double>> LastOffsets = new Dictionary<ScrollViewer, Tuple<double, double>>();

        public static readonly DependencyProperty SnapIntervalProperty = DependencyProperty.RegisterAttached("SnapInterval", typeof(double), typeof(ScrollViewer), new PropertyMetadata(0.0, OnSnapIntervalChanged));

        public static double GetSnapInterval(ScrollViewer owner)
        {
            return (double)owner.GetValue(SnapIntervalProperty);
        }

        public static void SetSnapInterval(ScrollViewer owner, double value)
        {
            owner.SetValue(SnapIntervalProperty, value);
        }

        private static void OnSnapIntervalChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer typedSender = (ScrollViewer)sender;
            if (!LastOffsets.Keys.Contains(typedSender))
            {
                typedSender.PointerCaptureLost += TypedSender_PointerCaptureLost;
                typedSender.ViewChanged += TypedSender_ViewChanged;
                typedSender.IsScrollInertiaEnabled = false;
                LastOffsets.Add(typedSender, new Tuple<double, double>(typedSender.HorizontalOffset, typedSender.HorizontalOffset));
            }
        }

        private static void TypedSender_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            ScrollViewer typedSender = (ScrollViewer)sender;
            if (Abs(typedSender.HorizontalOffset - LastOffsets[typedSender].Item2) > 5)
                LastOffsets[typedSender] = new Tuple<double, double>(LastOffsets[typedSender].Item2, typedSender.HorizontalOffset);
            if (!e.IsIntermediate)
            {
                List<double> snapOffsets = new List<double>();
                double snap = GetSnapInterval(typedSender);
                double offset = typedSender.HorizontalOffset;
                for (double i = 0; i < typedSender.ScrollableWidth; i += snap)
                {
                    snapOffsets.Add(i);
                }
                snapOffsets.Add(typedSender.ScrollableWidth);
                double newOffset;
                if (offset > typedSender.ScrollableWidth)
                    newOffset = typedSender.ScrollableWidth;
                else if (offset < 0)
                    newOffset = 0;
                else if (LastOffsets[typedSender].Item2 - LastOffsets[typedSender].Item1 > 0)
                    newOffset = snapOffsets.First(off => off >= offset);
                else
                    newOffset = snapOffsets.Last(off => off <= offset);
                ((ScrollViewer)typedSender).ChangeView(newOffset, null, null, false);
            }
        }

        private static void TypedSender_PointerCaptureLost(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
        }
    }
}
