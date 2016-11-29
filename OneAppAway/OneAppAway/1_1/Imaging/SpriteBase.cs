using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using static System.Math;

namespace OneAppAway._1_1.Imaging
{
    public abstract class SpriteBase
    {
        public abstract WriteableBitmap Bitmap { get; set; }
        public abstract double Width { get; }
        public abstract double Height { get; }
        public int PixelWidth => (int)Ceiling(Width);
        public int PixelHeight => (int)Ceiling(Height);

        public abstract bool IsLocked { get; }
        public abstract bool IsLoaded { get; }

        public abstract Task Load();
        public abstract void Lock();
        public abstract void Unlock();

        public abstract Color Render(int x, int y);

        public void Render(bool recreateBitmap = false)
        {
            if (!IsLoaded)
                throw new InvalidOperationException("Can't render a sprite that isn't loaded.");
            if (IsLocked)
                throw new InvalidOperationException("Can't render a sprite that is locked.");
            var start = DateTime.Now;
            if (recreateBitmap || Bitmap == null)
                Bitmap = new WriteableBitmap(PixelWidth, PixelHeight);
            Bitmap.ForEach((x, y) => Render(x, y));
            var end = DateTime.Now;
            double diff = (end - start).TotalMilliseconds;
            System.Diagnostics.Debug.WriteLine($"Total render time: {diff} ms.");
            System.Diagnostics.Debug.WriteLine($"Average render time per pixel: {diff / (Width * Height)} ms.");
        }

        public async Task<IRandomAccessStream> GetRandomAccessStream()
        {
            if (IsLocked)
            {
                InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream();
                await Bitmap.ToStream(stream, BitmapEncoder.PngEncoderId);
                return stream;
            }
            return null;
        }
    }
}
