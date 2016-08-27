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
using Windows.Graphics.Imaging;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace OneAppAway._1_1.Views.Controls
{
    public class TransitStopIconWrapper : DependencyObject
    {
        public MapElement Element { get; }
        public TransitStop Stop { get; }

        private static IRandomAccessStream[] BusIconStreams = new IRandomAccessStream[27];

        public static async Task LoadImages()
        {
            DateTime now = DateTime.Now;
            for (int i = 0; i <= 2; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    string postfix = ((j == 0) ? "BusBase" : ("BusDirection" + ((StopDirection)j).ToString())) + (i == 0 ? "20" : "40");
                    var bitmap = await WriteableBitmapExtensions.FromContent(null, new Uri($"ms-appx:///Assets/Icons/{postfix}.png"));
                    InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream();
                    await bitmap.ToStream(stream, BitmapEncoder.PngEncoderId);
                    BusIconStreams[i * 9 + j] = stream;
                }
            }
        }

        public TransitStopIconWrapper(TransitStop stop)
        {
            if (stop.Path == null)
            {
                Element = new MapIcon() { Location = stop.Position.ToGeopoint(), NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 0.5), CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible, ZIndex = 5 };
            }
            else
            {
                Element = new MapPolygon() { Path = new Windows.Devices.Geolocation.Geopath(GooglePolylineConverter.Decode(stop.Path).Select(ll => ll.ToBasicGeoposition())), StrokeColor = Colors.Black, FillColor = Colors.DarkGray, StrokeThickness = 2, ZIndex = 4 };
            }
            AttachedProperties.SetElementType(Element, "TransitStop");
            AttachedProperties.SetElementID(Element, stop.ID);
            Stop = stop;
            SetStopSize();
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
            var typedSender = sender as TransitStopIconWrapper;
            if (newVal != oldVal)
            {
                typedSender.SetCorrectStopSize();
            }
        }

        private bool _Hovered = false;
        public bool Hovered
        {
            get { return _Hovered; }
            set
            {
                _Hovered = value;
                SetCorrectStopSize();
            }
        }

        private void SetStopSize() => SetStopSize(StopSize);

        private void SetStopSize(MapStopSize size)
        {
            if (Element.Visible = (size != MapStopSize.Invisible))
            {
                if (Element is MapIcon)
                {
                    ((MapIcon)Element).Image = RandomAccessStreamReference.CreateFromStream(BusIconStreams[((int)size - 1) * 9 + (int)Stop.Direction]);
                    ((MapIcon)Element).Title = size == MapStopSize.Large ? Stop.Name ?? "" : "";
                }
            }
        }

        private void SetCorrectStopSize()
        {
            if (Hovered)
            {
                if (Element is MapIcon)
                    SetStopSize((MapStopSize)Math.Min((int)StopSize + 1, (int)MapStopSize.Large));
                else if (Element is MapPolygon)
                {
                    ((MapPolygon)Element).FillColor = Colors.LightGray;
                    //ToolTipService.SetToolTip(((MapPolygon)Element), new ToolTip() { Content = Stop.Name });
                }
            }
            else
            {
                if (Element is MapIcon)
                    SetStopSize();
                else if (Element is MapPolygon)
                {
                    ((MapPolygon)Element).FillColor = Colors.DarkGray;
                    
                    //ToolTipService.SetToolTip(((MapPolygon)Element), DependencyProperty.UnsetValue);
                }
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
