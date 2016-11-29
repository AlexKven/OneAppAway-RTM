using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI;

namespace OneAppAway._1_1.Imaging
{
    public class Sprite : SpriteBase
    {
        public override WriteableBitmap Bitmap { get; set; }
        public override bool IsLoaded => Bitmap != null;
        public override double Width => Bitmap?.PixelWidth ?? double.NaN;
        public override double Height => Bitmap?.PixelHeight ?? double.NaN;

        public byte[] BitmapBytes;
        public override bool IsLocked => BitmapBytes == null;
        
        public Uri ImageUri { get; set; }

        public override void Lock()
        {
            BitmapBytes = null;
        }

        public override void Unlock()
        {
            BitmapBytes = new byte[(int)(Width * Height * 4)];
            using (var stream = Bitmap.PixelBuffer.AsStream())
            {
                stream.Read(BitmapBytes, 0, BitmapBytes.Length);
            }
        }

        public override async Task Load()
        {
            Bitmap = await WriteableBitmapExtensions.FromContent(null, ImageUri);
        }

        public override Color Render(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return Colors.Transparent;
            int ind = 4 * (y * Bitmap.PixelWidth + x);
            return Color.FromArgb(BitmapBytes[ind + 3], BitmapBytes[ind], BitmapBytes[ind + 1], BitmapBytes[ind + 2]);
        }
    }
}
