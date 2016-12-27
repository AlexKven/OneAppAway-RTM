using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace OneAppAway._1_1.Imaging
{
    public static class MapIconImageGenerator
    {
        public static async Task<WriteableBitmap> GenerateBusMapIcon(StopDirection direction)
        {
            var bus = await GenerateBusMapIcon();
            if (direction == StopDirection.Unspecified)
                return bus;
            var arrow = await WriteableBitmapExtensions.FromContent(null, new Uri("ms-appx:///Assets/Icons/ArrowBase.png"));
            int angle = (int)direction * 45 - 45;
            arrow = arrow.RotateFree(angle);
            bus.Blit(new Windows.Foundation.Rect(43, 36, 170, 170), arrow, new Windows.Foundation.Rect(0, 0, 170, 170), WriteableBitmapExtensions.BlendMode.Alpha);
            return bus;
        }

        public static async Task<WriteableBitmap> GenerateBusMapIcon()
        {
            return await WriteableBitmapExtensions.FromContent(null, new Uri("ms-appx:///Assets/Icons/BusBase.png"));
        }


    }
}
