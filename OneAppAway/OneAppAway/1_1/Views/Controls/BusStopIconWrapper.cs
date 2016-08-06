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

namespace OneAppAway._1_1.Views.Controls
{
    public class BusStopIconWrapper : DependencyObject
    {
        const int SIZE_THRESHOLD = 10;

        string lastSize = "";

        public MapIcon Icon { get; }
        public BusStop Stop { get; }

        public BusStopIconWrapper(BusStop stop)
        {
            Icon = new MapIcon() { Location = LatLon.Seattle.ToGeopoint() };
            Icon.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
            Stop = stop;
        }

        public double MapZoomLevel
        {
            get { return (double)GetValue(MapZoomLevelProperty); }
            set { SetValue(MapZoomLevelProperty, value); }
        }
        public static readonly DependencyProperty MapZoomLevelProperty =
            DependencyProperty.Register("MapZoomLevel", typeof(double), typeof(BusStopIconWrapper), new PropertyMetadata(15.0, OnMapZoomLevelChangedStatic));
        static void OnMapZoomLevelChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            double val = (double)e.NewValue;
            BusStopIconWrapper instance = (BusStopIconWrapper)sender;

            string size = val < SIZE_THRESHOLD ? "20" : "40";
            //System.Diagnostics.Debug.WriteLine(size);
            if (instance.lastSize != size)
            {
                instance.Icon.Image = RandomAccessStreamReference.CreateFromUri(new Uri(instance.Stop.Direction == StopDirection.Unspecified ? "ms-appx:///Assets/Icons/BusBase" + size + ".png" : "ms-appx:///Assets/Icons/BusDirection" + instance.Stop.Direction.ToString() + size + ".png"));
                instance.lastSize = size;
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

        public LatLon Location
        {
            get { return (LatLon)GetValue(LocationProperty); }
            set { SetValue(LocationProperty, value); }
        }
        public static readonly DependencyProperty LocationProperty =
            DependencyProperty.Register("Location", typeof(LatLon), typeof(BusStopIconWrapper), new PropertyMetadata(LatLon.Seattle));
        static void OnLocationChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as BusStopIconWrapper).Icon.Location = ((LatLon)e.NewValue).ToGeopoint();
        }
    }
}
