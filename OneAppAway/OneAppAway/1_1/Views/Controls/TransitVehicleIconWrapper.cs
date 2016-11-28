using OneAppAway._1_1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace OneAppAway._1_1.Views.Controls
{
    public class TransitVehicleIconWrapper
    {
        private IRandomAccessStream ImageStream;
        private WriteableBitmap ImageBitmap;

        public TransitVehicleIconWrapper(RealTimeArrival arrival)
        {

        }
    }
}
