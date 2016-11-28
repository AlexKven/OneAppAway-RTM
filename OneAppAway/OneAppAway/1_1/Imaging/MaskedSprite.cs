using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace OneAppAway._1_1.Imaging
{
    public class MaskedSprite : ExternallyAppliedSpriteBase
    {
        public Func<Color, Color> Mask { get; set; }

        public override Color Render(int x, int y)
        {
            return Mask?.Invoke(base.Render(x, y)) ?? base.Render(x, y);
        }
    }
}
