using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using OneAppAway._1_1.ViewModels;
using Windows.UI.Xaml.Controls.Maps;
using OneAppAway._1_1.Data;
using OneAppAway._1_1.Converters;
using Windows.Devices.Geolocation;
using System.ComponentModel;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OneAppAway._1_1.Views.Controls
{
    public sealed partial class BusMap : UserControl, INotifyPropertyChanged
    {
        #region Fields
        private LatLon CenterOffset = new LatLon();
        private LatLonTransformConverter OffsetConverter;
        private StopViewModel_BusMap vm;
        private BusStopIconWrapper icon;
        #endregion

        public BusMap()
        {
            this.InitializeComponent();
            MainMap.MapServiceToken = Keys.BingMapKey;
            OffsetConverter = new LatLonTransformConverter() { Transform = ll => ll + CenterOffset, ReverseTransform = ll => ll - CenterOffset };
            ValueConverterGroup centerConverters = new ValueConverterGroup() { OffsetConverter, LatLonToGeopointConverter.Instance };
            MainMap.SetBinding(MapControl.CenterProperty, new Binding() { Converter = centerConverters, Source = this, Path = new PropertyPath("Center"), Mode = BindingMode.TwoWay });
            MainMap.SetBinding(MapControl.ZoomLevelProperty, new Binding() { Source = this, Path = new PropertyPath("ZoomLevel"), Mode = BindingMode.TwoWay });

            MapIcon centerIndicator = new MapIcon() { NormalizedAnchorPoint = new Point(0.5, 1) };
            BindingOperations.SetBinding(centerIndicator, MapIcon.LocationProperty, new Binding() { Source = this, Path = new PropertyPath("Center"), Converter = LatLonToGeopointConverter.Instance });
            MainMap.MapElements.Add(centerIndicator);

            BusStop seattle = new BusStop() { Direction = StopDirection.NE, Position = LatLon.Seattle.ToBasicGeoposition() };
            vm = new StopViewModel_BusMap(seattle);
            BindingOperations.SetBinding(vm, StopViewModel_BusMap.StopSizeProperty, new Binding() { Source = MainMap, Path = new PropertyPath("ZoomLevel"), Converter = new StopSizeThresholdConverter() { Threshold = 8 } });

            icon = new Controls.BusStopIconWrapper(seattle);
            BindingOperations.SetBinding(icon, BusStopIconWrapper.MapZoomLevelProperty, new Binding() { Source = MainMap, Path = new PropertyPath("ZoomLevel"), Mode = BindingMode.OneWay });
            

            MainMap.MapElements.Add(icon.Icon);
        }

        #region Properties
        public LatLon Center
        {
            get { return (LatLon)GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }
        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register("Center", typeof(LatLon), typeof(BusMap), new PropertyMetadata(LatLon.Seattle));

        public double ZoomLevel
        {
            get { return (double)GetValue(ZoomLevelProperty); }
            set { SetValue(ZoomLevelProperty, value); }
        }
        public static readonly DependencyProperty ZoomLevelProperty =
            DependencyProperty.Register("ZoomLevel", typeof(double), typeof(BusMap), new PropertyMetadata(10.0, OnZoomLevelChangedStatic));
        static void OnZoomLevelChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as BusMap)?.RecalculateCenterOffset();
        }

        public RectSubset CenterRegion
        {
            get { return (RectSubset)GetValue(CenterRegionProperty); }
            set { SetValue(CenterRegionProperty, value); }
        }
        public static readonly DependencyProperty CenterRegionProperty =
            DependencyProperty.Register("CenterRegion", typeof(RectSubset), typeof(BusMap), new PropertyMetadata(new RectSubset(), OnCenterRegionChangedStatic));
        static void OnCenterRegionChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as BusMap)?.RecalculateCenterOffset();
        }

        private double _LatitudePerPixel = double.NaN;
        private double _LongitudePerPixel = double.NaN;

        public double LatitudePerPixel
        {
            get { return _LatitudePerPixel; }
            set
            {
                _LatitudePerPixel = value;
                OnPropertyChanged("LatitudePerPixel");
            }
        }
        public double LongitudePerPixel
        {
            get { return _LongitudePerPixel; }
            set
            {
                _LongitudePerPixel = value;
                OnPropertyChanged("LongitudePerPixel");
            }
        }
        #endregion

        #region Methods
        private void SetLatLonDensities()
        {
            if (MainMap.ActualWidth > 0 && MainMap.ActualHeight > 0)
            {
                try
                {
                    double width = MainMap.ActualWidth;
                    double height = MainMap.ActualHeight;
                    Geopoint nw;
                    Geopoint se;
                    MainMap.GetLocationFromOffset(new Point(0, 0), out nw);
                    MainMap.GetLocationFromOffset(new Point(width, height), out se);
                    LatitudePerPixel = (nw.Position.Latitude - se.Position.Latitude) / height;
                    LongitudePerPixel = (nw.Position.Longitude - se.Position.Longitude) / width;
                }
                catch (ArgumentException)
                {
                    LatitudePerPixel = double.NaN;
                    LongitudePerPixel = double.NaN;
                }
            }
            else
            {
                LatitudePerPixel = double.NaN;
                LongitudePerPixel = double.NaN;
            }
        }

        private void RecalculateCenterOffset()
        {
            SetLatLonDensities();
            bool changed = false;
            if (LatitudePerPixel != double.NaN && LongitudePerPixel != double.NaN)
            {
                if (CenterRegion.DoesNothing)
                {
                    if (!CenterOffset.IsZero)
                    {
                        CenterOffset = new LatLon();
                        changed = true;
                    }
                }
                else
                {
                    double width = MainMap.ActualWidth;
                    double height = MainMap.ActualHeight;
                    double widthCenter = width / 2;
                    double heightCenter = height / 2;
                    double leftOffset;
                    double topOffset;
                    CenterRegion.Apply(ref width, ref height, out leftOffset, out topOffset);
                    double newWidthCenter = leftOffset + width / 2;
                    double newHeightCenter = topOffset + height / 2;
                    double pixelOffsetX = newWidthCenter - widthCenter;
                    double pixelOffsetY = newHeightCenter - heightCenter;
                    CenterOffset = new LatLon(pixelOffsetY * LatitudePerPixel, pixelOffsetX * LongitudePerPixel);
                    changed = true;
                }
            }
            //if (changed)
            //    Center = LatLon.Seattle;
        }
        #endregion

        #region Event Handlers
        private void MainMap_MapElementClick(Windows.UI.Xaml.Controls.Maps.MapControl sender, Windows.UI.Xaml.Controls.Maps.MapElementClickEventArgs args)
        {
            
        }

        private void MainMap_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RecalculateCenterOffset();
        }

        private void MainMap_ZoomLevelChanged(Windows.UI.Xaml.Controls.Maps.MapControl sender, object args)
        {

        }

        private void MainMap_CenterChanged(Windows.UI.Xaml.Controls.Maps.MapControl sender, object args)
        {

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
