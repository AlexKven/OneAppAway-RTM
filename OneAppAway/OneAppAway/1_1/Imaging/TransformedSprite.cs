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
    public class TransformedSprite : ExternallyAppliedSpriteBase
    {
        public override Color Render(int x, int y)
        {
            if (InverseTransform == null)
                return base.Render(x, y);
            Point transformed;
            if (!InverseTransform.TryTransform(new Point((int)(x - OffsetX), (int)(y - OffsetY)), out transformed))
                return Colors.Transparent;
            double fX = transformed.X + OffsetX;
            double fY = transformed.Y + OffsetY;
            //if (fX < 0 || fX >= Width || fY < 0 || fY >= Height)
            //    return Colors.Transparent;
            //else
            //{
                int x1 = (int)Floor(fX);
                int x2 = (int)Ceiling(fX);
                int y1 = (int)Floor(fY);
                int y2 = (int)Ceiling(fY);
                double xF = fX - x1;
                double yF = fY - y1;
                if (xF == 0)
                {
                    if (yF == 0)
                        return base.Render(x1, y1);
                    else
                        return CombineColors(base.Render(x1, y1), base.Render(x1, y2), yF);
                }
                else
                {
                    if (yF == 0)
                        return CombineColors(base.Render(x1, y1), base.Render(x2, y1), xF);
                    else
                        return CombineColors(CombineColors(base.Render(x1, y1), base.Render(x2, y1), xF), CombineColors(base.Render(x1, y2), base.Render(x2, y2), xF), yF);
                }
                //return base.Render(fX, fY);
            //}
        }

        private GeneralTransform InverseTransform;
        private int OffsetX;
        private int OffsetY;

        private void RefreshOffsets()
        {
            OffsetX = (int)(Width * RelativeTransformOrigin.X);
            OffsetY = (int)(Height * RelativeTransformOrigin.Y);
        }

        public static Color CombineColors(Color clr1, Color clr2, double portion)
        {
            return Color.FromArgb((byte)(clr1.A + (clr2.A - clr1.A) * portion), (byte)(clr1.R + (clr2.R - clr1.R) * portion), (byte)(clr1.G + (clr2.G - clr1.G) * portion), (byte)(clr1.B + (clr2.B - clr1.B) * portion));
        }

        private GeneralTransform _Transform;
        public GeneralTransform Transform
        {
            get { return _Transform; }
            set
            {
                _Transform = value;
                InverseTransform = Transform?.Inverse;
            }
        }

        private Point _RelativeTransformOrigin;
        public Point RelativeTransformOrigin
        {
            get { return _RelativeTransformOrigin; }
            set
            {
                _RelativeTransformOrigin = value;
                RefreshOffsets();
            }
        }

        public override async Task Load()
        {
            await base.Load();
            RefreshOffsets();
        }

        public override SpriteBase AppliedSprite
        {
            get
            {
                return base.AppliedSprite;
            }
            set
            {
                base.AppliedSprite = value;
                RefreshOffsets();
            }
        }
    }
}
