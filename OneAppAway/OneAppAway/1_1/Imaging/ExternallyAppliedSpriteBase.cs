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
    public abstract class ExternallyAppliedSpriteBase : SpriteBase
    {
        public override void Lock()
        {
            AppliedSprite.Lock();
        }

        public override void Unlock()
        {
            AppliedSprite.Unlock();
        }

        public override async Task Load()
        {
            await AppliedSprite?.Load();
            _Bitmap = new WriteableBitmap(AppliedSprite.Bitmap.PixelWidth, AppliedSprite.Bitmap.PixelHeight);
        }

        public override Color Render(int x, int y)
        {
            return AppliedSprite.Render(x, y);
        }

        public override bool IsLoaded => AppliedSprite.IsLoaded;

        public override bool IsLocked => AppliedSprite.IsLocked;

        private WriteableBitmap _Bitmap;
        public override WriteableBitmap Bitmap => _Bitmap;

        private SpriteBase _AppliedSprite;
        public virtual SpriteBase AppliedSprite
        {
            get { return _AppliedSprite; }
            set
            {
                _AppliedSprite = value;
            }
        }
    }
}
