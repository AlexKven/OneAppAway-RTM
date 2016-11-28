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
            int fX = (int)(transformed.X + OffsetX);
            int fY = (int)(transformed.Y + OffsetY);
            if (fX < 0 || fX >= Width || fY < 0 || fY >= Height)
                return Colors.Transparent;
            else
                return base.Render(fX, fY);
        }

        private GeneralTransform InverseTransform;
        private double OffsetX;
        private double OffsetY;

        private void RefreshOffsets()
        {
            OffsetX = Width * RelativeTransformOrigin.X;
            OffsetY = Height * RelativeTransformOrigin.Y;
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
