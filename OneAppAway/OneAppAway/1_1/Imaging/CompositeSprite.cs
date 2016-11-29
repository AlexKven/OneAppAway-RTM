using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using static System.Math;

namespace OneAppAway._1_1.Imaging
{
    public class CompositeSprite : SpriteBase
    {
        public override double Width => Sprites.Count > 0 ? Sprites[0].Width : double.NaN;
        public override double Height => Sprites.Count > 0 ? Sprites[0].Height : double.NaN;

        public CompositeSprite(params SpriteBase[] sprites)
        {
            Sprites.AddRange(sprites);
        }

        public override void Lock()
        {
            _IsLocked = true;
            foreach (SpriteBase sprite in Sprites)
                sprite.Lock();
        }

        public override void Unlock()
        {
            _IsLocked = false;
            foreach (SpriteBase sprite in Sprites)
                sprite.Unlock();
        }

        public override async Task Load()
        {
            if (Sprites.Count == 0)
                return;
            foreach (SpriteBase sprite in Sprites)
                await sprite.Load();
            Bitmap = new WriteableBitmap((int)Ceiling(Width), (int)Ceiling(Height));
        }

        public override Color Render(int x, int y)
        {
            Color result = Colors.Transparent;
            foreach (SpriteBase sprite in Sprites)
            {
                if (x >= sprite.Width || y >= sprite.Height)
                    continue;
                var color = sprite.Render(x, y);
                if (color.A == 0)
                    continue;
                if (color.A == 255 || result.A == 0)
                    result = color;
                else
                {
                    double srcA = color.A / 255.0;
                    double destA = result.A / 255.0;
                    double outA = srcA + destA * (1 - srcA);
                    if (outA == 0)
                        return Colors.Transparent;
                    byte r = AlphaBlend(color.R, result.R, srcA, destA, outA);
                    byte g = AlphaBlend(color.G, result.G, srcA, destA, outA);
                    byte b = AlphaBlend(color.B, result.B, srcA, destA, outA);
                    result.A = (byte)(outA * 255);
                    result.R = r;
                    result.G = g;
                    result.B = b;
                }
            }
            return result;
        }

        private static byte AlphaBlend(byte srcRGB, byte destRGB, double srcA, double destA, double outA)
        {
            return (byte)((srcRGB * srcA + destRGB * destA * (1 - srcA)) / outA);
        }
        
        private bool _IsLocked = true;

        public override bool IsLoaded => Bitmap != null;

        public override bool IsLocked => _IsLocked;
        
        public override WriteableBitmap Bitmap { get; set; }

        public List<SpriteBase> Sprites { get; } = new List<SpriteBase>();
    }
}
