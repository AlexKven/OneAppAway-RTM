using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;
using OneAppAway._1_1.Data;
using System.IO;
using Windows.UI.Xaml.Media.Imaging;

namespace OneAppAway._1_1.Views.Controls
{
    public class TransitStopIconWrapper : DependencyObject
    {
        public MapIcon Icon { get; }
        public TransitStop Stop { get; }

        private static IRandomAccessStream ImageS = null;
        private static IRandomAccessStream ImageM = null;

        public TransitStopIconWrapper(TransitStop stop)
        {
            //if (ImageS == null)
            //{
            //    var imageS = new MemoryStream();
            //    var imageM = new MemoryStream();
            //    var bitmap = new BitmapImage(new Uri("ms-appx:///Assets/Icons/BusBase40.png"));
            //    WriteableBitmap wb = new WriteableBitmap(bitmap.PixelWidth, bitmap.PixelHeight);
            //    bitmap.
            //    var sf = StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Icons/BusBase40.png"));
            //    sf.
            //    str.CopyTo(imageS);
            //    str.Dispose();
            //    str = new FileStream(, FileMode.Open);
            //    str.CopyTo(imageM);
            //    str.Dispose();
            //    ImageM = imageM.AsRandomAccessStream();
            //    ImageS = imageS.AsRandomAccessStream();
                
            //}
            Icon = new MapIcon() { Location = stop.Position.ToGeopoint() };
            Icon.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
            Stop = stop;
        }

        public MapStopSize StopSize
        {
            get { return (MapStopSize)GetValue(StopSizeProperty); }
            set { SetValue(StopSizeProperty, value); }
        }
        public static readonly DependencyProperty StopSizeProperty =
            DependencyProperty.Register("StopSize", typeof(MapStopSize), typeof(TransitStopIconWrapper), new PropertyMetadata(MapStopSize.Medium, OnStopSizeChangedStatic));
        static void OnStopSizeChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            MapStopSize newVal = (MapStopSize)e.NewValue;
            MapStopSize oldVal = (MapStopSize)e.OldValue;
            if (newVal != oldVal)
            {
                TransitStopIconWrapper instance = (TransitStopIconWrapper)sender;
                StringBuilder uriBuilder = new StringBuilder("ms-appx:///Assets/Icons/");
                if (instance.Stop.Direction == Data.StopDirection.Unspecified)
                    uriBuilder.Append("BusBase");
                else
                {
                    uriBuilder.Append("BusDirection");
                    uriBuilder.Append(instance.Stop.Direction.ToString());
                }
                uriBuilder.Append(newVal == MapStopSize.Medium ? "40" : "20");
                uriBuilder.Append(".png");
                //instance.Icon.Image = newVal == MapStopSize.Medium ? RandomAccessStreamReference.CreateFromStream(ImageM) : RandomAccessStreamReference.CreateFromStream(ImageS);
                instance.Icon.Image = RandomAccessStreamReference.CreateFromUri(new Uri(uriBuilder.ToString()));
            }
        }

        //public object ImageStreamSource
        //{
        //    get { return GetValue(ImageStreamSourceProperty); }
        //    set { SetValue(ImageStreamSourceProperty, value); }
        //}
        //public static readonly DependencyProperty ImageStreamSourceProperty =
        //    DependencyProperty.Register("ImageStreamSource", typeof(object), typeof(BusStopIconWrapper), new PropertyMetadata(null, OnImageStreamSourceChangedStatic));
        //static void OnImageStreamSourceChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        //{
        //    if (e.NewValue == null)
        //        (sender as BusStopIconWrapper).Icon.Image = null;
        //    else if (e.NewValue is string)
        //        (sender as BusStopIconWrapper).Icon.Image = RandomAccessStreamReference.CreateFromUri(new Uri((string)e.NewValue));
        //    else if (e.NewValue is IStorageFile)
        //        (sender as BusStopIconWrapper).Icon.Image = RandomAccessStreamReference.CreateFromFile((IStorageFile)e.NewValue);
        //    else if (e.NewValue is IRandomAccessStream)
        //        (sender as BusStopIconWrapper).Icon.Image = RandomAccessStreamReference.CreateFromStream((IRandomAccessStream)e.NewValue);
        //    else
        //        (sender as BusStopIconWrapper).Icon.Image = null;
        //}
    }
}
