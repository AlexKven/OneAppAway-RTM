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
using System.Threading.Tasks;
using System.Collections.ObjectModel;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OneAppAway._1_1.Views.Controls
{
    public sealed partial class TransitMap : UserControl, INotifyPropertyChanged
    {
        #region Fields
        private LatLon CenterOffset = new LatLon();
        private ValueConverterGroup CenterConverter = new ValueConverterGroup();
        
        private StopSizeThresholdConverter StopSizeConverter = new StopSizeThresholdConverter() { LargeThreshold = 17, MediumThreshold = 16, SmallThreshold = 14 };
        //TransitStopIconWrapper[] icons = new TransitStopIconWrapper[15 * 15];

        private bool? User_CenterChanged = null;
        private bool? User_ZoomLevelChanged = null;
        #endregion

        public TransitMap()
        {
            this.InitializeComponent();
            MainMap.MapServiceToken = Keys.BingMapKey;
            CenterConverter.Add(new LatLonTransformConverter() { Transform = ll => CenterOffset.IsNotALocation ? ll : ll + CenterOffset, ReverseTransform = ll => CenterOffset.IsNotALocation ? ll : ll - CenterOffset });
            CenterConverter.Add(LatLonToGeopointConverter.Instance);

            //MainMap.SetBinding(MapControl.CenterProperty, new Binding() { Converter = CenterConverters, Source = this, Path = new PropertyPath("Center"), Mode = BindingMode.TwoWay });
            //MainMap.SetBinding(MapControl.ZoomLevelProperty, new Binding() { Source = this, Path = new PropertyPath("ZoomLevel"), Mode = BindingMode.TwoWay });

            MapIcon centerIndicator = new MapIcon() { NormalizedAnchorPoint = new Point(0.5, 1) };
            BindingOperations.SetBinding(centerIndicator, MapIcon.LocationProperty, new Binding() { Source = this, Path = new PropertyPath("Center"), Converter = LatLonToGeopointConverter.Instance });
            MainMap.MapElements.Add(centerIndicator);
        }

        #region Properties
        public LatLon Center
        {
            get { return (LatLon)GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }
        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register("Center", typeof(LatLon), typeof(TransitMap), new PropertyMetadata(LatLon.Seattle, OnCenterChangedStatic));
        static void OnCenterChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as TransitMap)?.OnCenterChanged();
        }

        public double ZoomLevel
        {
            get { return (double)GetValue(ZoomLevelProperty); }
            set { SetValue(ZoomLevelProperty, value); }
        }
        public static readonly DependencyProperty ZoomLevelProperty =
            DependencyProperty.Register("ZoomLevel", typeof(double), typeof(TransitMap), new PropertyMetadata(10.0, OnZoomLevelChangedStatic));
        static void OnZoomLevelChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as TransitMap)?.OnZoomLevelChanged();
        }

        public LatLon CenterDelay
        {
            get { return (LatLon)GetValue(CenterDelayProperty); }
            private set { SetValue(CenterDelayProperty, value); }
        }
        public static readonly DependencyProperty CenterDelayProperty =
            DependencyProperty.Register("CenterDelay", typeof(LatLon), typeof(TransitMap), new PropertyMetadata(LatLon.Seattle));
        
        public double ZoomLevelDelay
        {
            get { return (double)GetValue(ZoomLevelDelayProperty); }
            private set { SetValue(ZoomLevelDelayProperty, value); }
        }
        public static readonly DependencyProperty ZoomLevelDelayProperty =
            DependencyProperty.Register("ZoomLevelDelay", typeof(double), typeof(TransitMap), new PropertyMetadata(10.0));
        
        public RectSubset CenterRegion
        {
            get { return (RectSubset)GetValue(CenterRegionProperty); }
            set { SetValue(CenterRegionProperty, value); }
        }
        public static readonly DependencyProperty CenterRegionProperty =
            DependencyProperty.Register("CenterRegion", typeof(RectSubset), typeof(TransitMap), new PropertyMetadata(new RectSubset(), OnCenterRegionChangedStatic));
        static void OnCenterRegionChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as TransitMap)?.OnCenterRegionChanged();
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

        private TimeSpan DelayedPropertyWait { get; set; } = TimeSpan.FromMilliseconds(100);

        public object StopsSource
        {
            get { return (object)GetValue(StopsSourceProperty); }
            set { SetValue(StopsSourceProperty, value); }
        }
        public static readonly DependencyProperty StopsSourceProperty =
            DependencyProperty.Register("StopsSource", typeof(object), typeof(TransitMap), new PropertyMetadata(null, OnStopsSourceChangedStatic));
        static void OnStopsSourceChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((TransitMap)sender).OnStopsSourceChanged(e.OldValue, e.NewValue);
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

        private bool RecalculateCenterOffset()
        {
            SetLatLonDensities();
            LatLon oldOffset = CenterOffset;
            if (LatitudePerPixel != double.NaN && LongitudePerPixel != double.NaN)
            {
                if (CenterRegion.DoesNothing || LatitudePerPixel == double.NaN || LongitudePerPixel == double.NaN)
                {
                    CenterOffset = new LatLon();
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
                }
            }
            return (oldOffset != CenterOffset);
        }

        private void SetProperZoomLevel()
        {
            if (!User_ZoomLevelChanged.HasValue)
                return;
            if (User_ZoomLevelChanged.Value)
                ZoomLevel = MainMap.ZoomLevel;
            else
                MainMap.ZoomLevel = ZoomLevel;
            User_ZoomLevelChanged = null;
        }

        private void SetProperCenter()
        {
            if (!User_CenterChanged.HasValue)
                return;
            if (User_CenterChanged.Value)
                Center = (LatLon)CenterConverter.ConvertBack(MainMap.Center, typeof(LatLon), null, null);
            else
                MainMap.Center = (Geopoint)CenterConverter.Convert(Center, typeof(Geopoint), null, null);
            User_CenterChanged = null;
        }

        private void OnZoomLevelChanged()
        {
            if (!User_ZoomLevelChanged.HasValue)
            {
                User_ZoomLevelChanged = false;
                SetProperZoomLevel();
                if (RecalculateCenterOffset())
                {
                    User_CenterChanged = false;
                    SetProperCenter();
                }
            }
            DelaySetZoomLevel();

            ZoomLevelBlock.Text = $"ZoomLevel: {ZoomLevel}";
        }

        private void OnCenterChanged()
        {
            if (!User_CenterChanged.HasValue)
            {
                User_CenterChanged = false;
                SetProperCenter();
            }
            DelaySetCenter();

            CoordsBlock.Text = Center.ToString();
        }

        private void OnCenterRegionChanged()
        {
            if (RecalculateCenterOffset())
                OnCenterChanged();
        }

        void OnStopsSourceChanged(object oldValue, object newValue)
        {
            if (oldValue is ObservableCollection<TransitStop>)
                UnregisterStopsSourceHandlers((ObservableCollection<TransitStop>)oldValue);
            ClearStops();
            if (newValue is ObservableCollection<TransitStop>)
                RegisterStopsSourceHandlers((ObservableCollection<TransitStop>)newValue);
            else if (newValue is IEnumerable<TransitStop>)
            {
                foreach (var stop in (IEnumerable<TransitStop>)newValue)
                    AddStopsToMap(stop);
            }
            else if (newValue is TransitStop)
                AddStopsToMap((TransitStop)newValue);
        }

        private void RegisterStopsSourceHandlers(ObservableCollection<TransitStop> collection)
        {
            collection.CollectionChanged += StopsSource_CollectionChanged;
        }

        private void UnregisterStopsSourceHandlers(ObservableCollection<TransitStop> collection)
        {
            collection.CollectionChanged -= StopsSource_CollectionChanged;
        }

        private void AddStopsToMap(params TransitStop[] stops)
        {
            foreach (var stop in stops)
            {
                TransitStopIconWrapper wrapper = new TransitStopIconWrapper(stop);
                BindingOperations.SetBinding(wrapper, TransitStopIconWrapper.StopSizeProperty, new Binding() { Source = this, Path = new PropertyPath("ZoomLevelDelay"), Mode = BindingMode.OneWay, Converter = StopSizeConverter });
                MainMap.MapElements.Add(wrapper.Icon);
            }
        }

        private void ClearStops()
        {
            foreach (var item in MainMap.MapElements.ToArray().Where(me => (me is MapIcon) && AttachedProperties.GetElementType((MapIcon)me) == "TransitStop"))
                MainMap.MapElements.Remove(item);
        }

        private void RemoveStopsFromMap(params TransitStop[] stops)
        {
            foreach (var item in MainMap.MapElements.ToArray())
            {
                if ((item is MapIcon) && AttachedProperties.GetElementType((MapIcon)item) == "TransitStop")
                {
                    if (stops.Any(stop => stop.ID == AttachedProperties.GetElementID((MapIcon)item)))
                        MainMap.MapElements.Remove(item);
                }
            }
        }
        #endregion

        #region Event Handlers
        private void MainMap_MapElementClick(Windows.UI.Xaml.Controls.Maps.MapControl sender, Windows.UI.Xaml.Controls.Maps.MapElementClickEventArgs args)
        {
            
        }

        private void MainMap_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (RecalculateCenterOffset())
                OnCenterChanged();
        }

        private void MainMap_ZoomLevelChanged(Windows.UI.Xaml.Controls.Maps.MapControl sender, object args)
        {
            if (!User_ZoomLevelChanged.HasValue)
            {
                User_ZoomLevelChanged = true;
                SetProperZoomLevel();
                if (RecalculateCenterOffset())
                {
                    User_CenterChanged = true;
                    SetProperCenter();
                }
            }
        }

        private void MainMap_CenterChanged(Windows.UI.Xaml.Controls.Maps.MapControl sender, object args)
        {
            if (!User_CenterChanged.HasValue)
            {
                User_CenterChanged = true;
                SetProperCenter();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (StopsSource is ObservableCollection<TransitStop>)
                RegisterStopsSourceHandlers((ObservableCollection<TransitStop>)StopsSource);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (StopsSource is ObservableCollection<TransitStop>)
                UnregisterStopsSourceHandlers((ObservableCollection<TransitStop>)StopsSource);
        }

        private void StopsSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    ClearStops();
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    AddStopsToMap(e.NewItems.Cast<TransitStop>().ToArray());
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    RemoveStopsFromMap(e.OldItems.Cast<TransitStop>().ToArray());
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    RemoveStopsFromMap(e.OldItems.Cast<TransitStop>().ToArray());
                    AddStopsToMap(e.NewItems.Cast<TransitStop>().ToArray());
                    break;
            }

        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Delayed Property Methods
        private DateTime LastCenterLevelChange = DateTime.Now;
        private DateTime LastZoomLevelChange = DateTime.Now;

        private async void DelaySetCenter()
        {
            DateTime now = DateTime.Now;
            LastCenterLevelChange = now;
            await Task.Delay(DelayedPropertyWait);
            if (LastCenterLevelChange == now)
                CenterDelay = Center;
        }

        private async void DelaySetZoomLevel()
        {
            DateTime now = DateTime.Now;
            LastZoomLevelChange = now;
            await Task.Delay(DelayedPropertyWait);
            if (LastZoomLevelChange == now)
                ZoomLevelDelay = ZoomLevel;
        }
        #endregion
    }
}
