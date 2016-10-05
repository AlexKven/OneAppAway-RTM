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
using Windows.UI.Xaml.Media.Animation;
using System.Collections.Specialized;
using MvvmHelpers;
using OneAppAway._1_1.Helpers;
using System.Windows.Input;
using OneAppAway._1_1.AddIns;
using OneAppAway._1_1.Addins;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OneAppAway._1_1.Views.Controls
{
    public sealed partial class TransitMap : UserControl, INotifyPropertyChanged
    {
        #region Fields
        private LatLon CenterOffset = new LatLon();
        private ValueConverterGroup CenterConverter = new ValueConverterGroup();

        private bool? User_CenterChanged = null;
        private bool? User_ZoomLevelChanged = null;
        #endregion

        public TransitMap()
        {
            this.InitializeComponent();
            MainMap.MapServiceToken = Keys.BingMapKey;
            CenterConverter.Add(new LatLonTransformConverter() { Transform = ll => CenterOffset.IsNotALocation ? ll : ll + CenterOffset, ReverseTransform = ll => CenterOffset.IsNotALocation ? ll : ll - CenterOffset });
            CenterConverter.Add(LatLonToGeopointConverter.Instance);
            WeakEventListener<TransitMap, object, NotifyCollectionChangedEventArgs> addInsListener = new WeakEventListener<TransitMap, object, NotifyCollectionChangedEventArgs>(this);
            addInsListener.OnEventAction = (map, obj, e) => map.AddIns_CollectionChanged(obj, e);
            AddIns.CollectionChanged += addInsListener.OnEvent;

            MapRouteBindings = new CompositeCollectionBinding<MapRouteView, TransitMapAddInBase>(MainMap.Routes);
            MapElementBindings = new CompositeCollectionBinding<MapElement, TransitMapAddInBase>(MainMap.MapElements);
            MapChildrenBindings = new CompositeCollectionBinding<DependencyObject, TransitMapAddInBase>(MainMap.Children);

            //MainMap.SetBinding(MapControl.CenterProperty, new Binding() { Converter = CenterConverters, Source = this, Path = new PropertyPath("Center"), Mode = BindingMode.TwoWay });
            //MainMap.SetBinding(MapControl.ZoomLevelProperty, new Binding() { Source = this, Path = new PropertyPath("ZoomLevel"), Mode = BindingMode.TwoWay });

            //MapIcon centerIndicator = new MapIcon() { NormalizedAnchorPoint = new Point(0.5, 1) };
            //BindingOperations.SetBinding(centerIndicator, MapIcon.LocationProperty, new Binding() { Source = this, Path = new PropertyPath("Center"), Converter = LatLonToGeopointConverter.Instance });
            //MainMap.MapElements.Add(centerIndicator);
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

        public LatLon ActualCenter
        {
            get { return MainMap.Center.ToLatLon(); }
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

        public LatLonRect Area
        {
            get { return (LatLonRect)GetValue(AreaProperty); }
            set { SetValue(AreaProperty, value); }
        }
        public static readonly DependencyProperty AreaProperty =
            DependencyProperty.Register("Area", typeof(LatLonRect), typeof(TransitMap), new PropertyMetadata(new LatLonRect(), OnAreaChangedStatic));
        static void OnAreaChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as TransitMap)?.OnAreaChanged();
        }

        public LatLonRect AreaDelay
        {
            get { return (LatLonRect)GetValue(AreaDelayProperty); }
            set { SetValue(AreaDelayProperty, value); }
        }
        public static readonly DependencyProperty AreaDelayProperty =
            DependencyProperty.Register("AreaDelay", typeof(LatLonRect), typeof(TransitMap), new PropertyMetadata(new LatLonRect()));

        private TimeSpan DelayedPropertyWait { get; set; } = TimeSpan.FromMilliseconds(100);
        internal MapControl UnderlyingMapControl => MainMap;
        
        public double CurrentZoomRate
        {
            get { return (double)GetValue(CurrentZoomRateProperty); }
            set { SetValue(CurrentZoomRateProperty, value); }
        }
        public static readonly DependencyProperty CurrentZoomRateProperty =
            DependencyProperty.Register("CurrentZoomRate", typeof(double), typeof(TransitMap), new PropertyMetadata(0.0, OnCurretZoomRateChangedStatic));
        static void OnCurretZoomRateChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TransitMap map = sender as TransitMap;
            if (map != null)
            {
                if ((double)e.OldValue != 0)
                    map.MainMap.StopContinuousZoom();
                if ((double)e.NewValue != 0)
                    map.MainMap.StartContinuousZoom((double)e.NewValue);
            }
        }

        //public bool IsMapUpdatingSuspended
        //{
        //    get { return (bool)GetValue(IsMapUpdatingSuspendedProperty); }
        //    set { SetValue(IsMapUpdatingSuspendedProperty, value); }
        //}
        //public static readonly DependencyProperty IsMapUpdatingSuspendedProperty =
        //    DependencyProperty.Register("IsMapUpdatingSuspended", typeof(bool), typeof(TransitMap), new PropertyMetadata(false));
        //private static void OnIsMapUpdatingSuspendedChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        //{
        //    if (!(bool)e.NewValue)
        //        (sender as TransitMap)?.OnCenterChanged();
        //}
        #endregion

        #region Methods
        #region On___Changed
        private void OnZoomLevelChanged()
        {
            SetArea();
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

            ZoomLevelBlock.Text = $"ZoomLevel: {ZoomLevel.ToString("#.00000")}";

            ZoomLevelChanged?.Invoke(this, new EventArgs());
        }

        private void OnCenterChanged()
        {
            //if (IsMapUpdatingSuspended)
            //    return;
            SetArea();
            if (!User_CenterChanged.HasValue)
            {
                User_CenterChanged = false;
                SetProperCenter();
            }
            DelaySetCenter();

            CoordsBlock.Text = $"Center: {Center.ToString("#.00000")} Span: {Area.Span.ToString("#.00000")}";
            CoordsBlock2.Text = $"Area: {Area.ToString("#.00000")}";
            CenterChanged?.Invoke(this, new EventArgs());
        }

        private void OnCenterRegionChanged()
        {
            if (RecalculateCenterOffset())
                MainMap_CenterChanged(MainMap, null);
        }

        private void OnAreaChanged()
        {
            DelaySetArea();
        }
        #endregion

        #region Private Actions
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
                if ((CenterRegion.DoesNothing && (CurrentTakeover?.CenterRegionOverride.DoesNothing ?? true)) || LatitudePerPixel == double.NaN || LongitudePerPixel == double.NaN)
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
                    if (CurrentTakeover != null)
                    {
                        double leftOffset2;
                        double topOffset2;
                        CurrentTakeover.CenterRegionOverride.Apply(ref width, ref height, out leftOffset2, out topOffset2);
                        leftOffset += leftOffset2;
                        topOffset += topOffset2;
                    }
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

        private void SetArea()
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
                    Area = LatLonRect.FromNWSE(nw.ToLatLon(), se.ToLatLon());
                }
                catch (ArgumentException)
                {
                    Area = LatLonRect.NotAnArea;
                }
            }
            else
                Area = LatLonRect.NotAnArea;
        }
        #endregion

        #region Public Methods
        public async Task TrySetView(MapView view)
        {
            if (view.Area == null)
            {
                Geopoint adjustedCenter = null;
                if (view.Center != null)
                    adjustedCenter = (Geopoint)CenterConverter.Convert(view.Center.Value, typeof(Geopoint), null, null);
                var result = await MainMap.TrySetViewAsync(adjustedCenter, view.ZoomLevel, null, null, view.Animate ? MapAnimationKind.Default : MapAnimationKind.None);
            }
            else
            {
                await MainMap.TrySetViewBoundsAsync(new GeoboundingBox(view.Area.Value.NW.ToBasicGeoposition(), view.Area.Value.SE.ToBasicGeoposition()), null, view.Animate ? MapAnimationKind.Default : MapAnimationKind.None);
            }
        }
        #endregion

        //private void SetPropertyInternal(DependencyProperty property, object value)
        //{
        //    var binding = GetBindingExpression(property);
        //    SetValue(property, value);
        //    if (binding != null)
        //        SetBinding(property, binding.ParentBinding);
        //}
        #endregion

        #region Event Handlers
        private void MainMap_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (RecalculateCenterOffset())
                OnCenterChanged();
            foreach (var addIn in AddIns)
            {
                addIn.OnSizeChanged(e.PreviousSize, e.NewSize);
            }
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
            //if (SelectedStopsSource is ObservableCollection<TransitStop>)
            //    RegisterSelectedStopsSourceHandlers((ObservableCollection<TransitStop>)SelectedStopsSource);
            //RegisterArrivalsViewModelCloseEventHandler();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //if (SelectedStopsSource is ObservableCollection<TransitStop>)
            //    UnregisterSelectedStopsSourceHandlers((ObservableCollection<TransitStop>)SelectedStopsSource);
            //UnregisterArrivalsViewModelCloseEventHandler();
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
        private DateTime LastAreaChange = DateTime.Now;

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

        private async void DelaySetArea()
        {
            DateTime now = DateTime.Now;
            LastAreaChange = now;
            await Task.Delay(DelayedPropertyWait);
            if (LastAreaChange == now)
                AreaDelay = Area;
        }
        #endregion

        #region Events
        public event EventHandler CenterChanged;
        public event EventHandler ZoomLevelChanged;
        #endregion

        #region AddIns
        private ObservableCollection<TransitMapAddInBase> _AddIns = new ObservableCollection<TransitMapAddInBase>();
        public ObservableCollection<TransitMapAddInBase> AddIns => _AddIns;

        private MapTakeover _CurrentTakeover;
        internal MapTakeover CurrentTakeover
        {
            get { return _CurrentTakeover; }
            set
            {
                _CurrentTakeover = value;
                TakeoverOverlayControl.Visibility = CurrentTakeover?.MapOverlay == null ? Visibility.Collapsed : Visibility.Visible;
                TakeoverOverlayControl.Content = CurrentTakeover?.MapOverlay;
                OnCenterRegionChanged();
                SetTakeoverView();
            }
        }

        internal TransitMapAddInBase CurrentTakeoverOwner { get; set; }

        private CompositeCollectionBinding<MapElement, TransitMapAddInBase> MapElementBindings;
        private CompositeCollectionBinding<DependencyObject, TransitMapAddInBase> MapChildrenBindings;
        private CompositeCollectionBinding<MapRouteView, TransitMapAddInBase> MapRouteBindings;

        private Dictionary<TransitMapAddInBase, WeakEventListener<TransitMap, object, MapTakeoverRequestedEventArgs>> MapTakeoverRequestedListeners = new Dictionary<TransitMapAddInBase, WeakEventListener<TransitMap, object, MapTakeoverRequestedEventArgs>>();

        private void AddIns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (TransitMapAddInBase addIn in e.NewItems)
                {
                    MapElementBindings.AddCollection(addIn, addIn.MapElementsShown);
                    MapRouteBindings.AddCollection(addIn, addIn.MapRoutesShown);
                    MapChildrenBindings.AddCollection(addIn, addIn.MapChildrenShown);
                    WeakEventListener<TransitMap, object, MapTakeoverRequestedEventArgs> takeoverRequestedListener = new WeakEventListener<TransitMap, object, MapTakeoverRequestedEventArgs>(this);
                    takeoverRequestedListener.OnEventAction = (map, _sender, _e) => map.Addin_TakeoverRequested(_sender, _e);
                    addIn.TakeoverRequested += takeoverRequestedListener.OnEvent;
                    addIn.OnSizeChanged(null, new Size(ActualWidth, ActualHeight));
                }
            }
            if (e.OldItems != null)
            {
                foreach (TransitMapAddInBase addIn in e.OldItems)
                {
                    MapElementBindings.RemoveCollection(addIn);
                    MapRouteBindings.RemoveCollection(addIn);
                    MapChildrenBindings.RemoveCollection(addIn);
                    MapTakeoverRequestedListeners[addIn].Detach();
                    MapTakeoverRequestedListeners.Remove(addIn);
                }
            }
        }

        private void Addin_TakeoverRequested(object sender, MapTakeoverRequestedEventArgs e)
        {
            if (CurrentTakeover != null)
                CurrentTakeoverOwner?.OnTakeoverEvicted(CurrentTakeover);
            if (e.Takeover != null)
            {
                CurrentTakeoverOwner = sender as TransitMapAddInBase;
                CurrentTakeoverOwner?.OnTakeoverGranted(e.Takeover);
            }
            CurrentTakeover = e.Takeover;
        }

        private async void SetTakeoverView()
        {
            if (CurrentTakeover?.ViewOverride != null)
                await TrySetView(CurrentTakeover.ViewOverride);
        }
        #endregion

        private void MainMap_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            foreach (var addin in AddIns)
            {
                addin.OnMapElementsClicked(args.MapElements.Intersect(addin.MapElementsShown), args.Location.ToLatLon(), args.Position);
            }
        }

        private void MainMap_MapElementPointerEntered(MapControl sender, MapElementPointerEnteredEventArgs args)
        {
            AddIns.FirstOrDefault(addin => addin.MapElementsShown.Contains(args.MapElement))?.OnMapElementPointerEntered(args.MapElement, args.Location.ToLatLon(), args.Position);
        }

        private void MainMap_MapElementPointerExited(MapControl sender, MapElementPointerExitedEventArgs args)
        {
            AddIns.FirstOrDefault(addin => addin.MapElementsShown.Contains(args.MapElement))?.OnMapElementPointerExited(args.MapElement, args.Location.ToLatLon(), args.Position);
        }
    }
}
