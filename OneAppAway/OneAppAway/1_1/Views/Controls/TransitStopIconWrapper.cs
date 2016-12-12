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
using OneAppAway._1_1.Helpers;
using OneAppAway._1_1.Imaging;
using System.IO;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using static System.Math;

namespace OneAppAway._1_1.Views.Controls
{
    public class TransitStopIconWrapper : DependencyObject
    {
        public MapElement Element { get; }
        public TransitStop Stop { get; }
        const int NUM_ICON_TYPES = 11;
        const int NUM_ICON_SIZES = 3;

        private static IRandomAccessStream[] BusIconStreams = new IRandomAccessStream[NUM_ICON_TYPES * NUM_ICON_SIZES];

        public static async Task LoadImages()
        {
            for (int i = 0; i < NUM_ICON_SIZES; i++)
            {
                for (int j = 0; j < NUM_ICON_TYPES; j++)
                {
                    string postfix;
                    if (j >= 1 && j <= 8)
                        postfix = "BusDirection" + ((StopDirection)j).ToString();
                    else if (j == 0)
                        postfix = "BusBase";
                    else
                        postfix = j == 9 ? "BusAlert" : "BusClosed";
                    postfix += (i == 0 ? "20" : "40");
                    var sprite = new Sprite() { ImageUri = new Uri($"ms-appx:///Assets/Icons/{postfix}.png") };
                    await sprite.Load();
                    sprite.Unlock();
                    //var buffer1 = new SpriteBitmapStream(sprite).GetFullBuffer();
                    //using (MemoryStream stream1 = new MemoryStream(buffer1, true))
                    //{
                    //    BusIconStreams[i * NUM_ICON_TYPES + j] = stream1.AsRandomAccessStream();
                    //}
                    //var bitmap = await WriteableBitmapExtensions.FromContent(null, new Uri($"ms-appx:///Assets/Icons/{postfix}.png"));
                    InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream();
                    
                    await sprite.Bitmap.ToStream(stream, BitmapEncoder.BmpEncoderId);
                    BusIconStreams[i * NUM_ICON_TYPES + j] = stream;
                    byte[] buffer = new byte[stream.Size];
                    stream.AsStreamForRead().Read(buffer, 0, buffer.Length);
                    for (int k = 0; k < buffer.Length; k++)
                    {
                        //if (buffer[k] != buffer1[k])
                        //{
                        //    //System.Diagnostics.Debug.WriteLine($"{buffer[k]} vs {buffer1[k]}");
                        //}
                    }
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
                typedSender?.SetCorrectStopSize();
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
                    int iconNumber;
                    if (Stop.Status == AlertStatus.Alert)
                        iconNumber = 9;
                    else if (Stop.Status == AlertStatus.Cancelled)
                        iconNumber = 10;
                    else
                        iconNumber = (int)Stop.Direction;
                    ((MapIcon)Element).Image = RandomAccessStreamReference.CreateFromStream(BusIconStreams[((int)size - 1) * NUM_ICON_TYPES + iconNumber]);
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
                    if (Stop.Status == AlertStatus.Alert)
                        ((MapPolygon)Element).FillColor = Colors.LightGoldenrodYellow;
                    else if (Stop.Status == AlertStatus.Cancelled)
                        ((MapPolygon)Element).FillColor = Colors.Red;
                    else
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
