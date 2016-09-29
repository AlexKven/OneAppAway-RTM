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

        private ArrivalsControlInTransitPageViewModel ArrivalsViewModel;
        private StopPopupOuterControl ArrivalsPopup;
        private TranslateTransform ArrivalsPopupTransform = new TranslateTransform();
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

        public object SelectedStopsSource
        {
            get { return (object)GetValue(SelectedStopsSourceProperty); }
            set { SetValue(SelectedStopsSourceProperty, value); }
        }
        public static readonly DependencyProperty SelectedStopsSourceProperty =
            DependencyProperty.Register("SelectedStops", typeof(object), typeof(TransitMap), new PropertyMetadata(null, OnSelectedStopsSourceChangedStatic));
        static void OnSelectedStopsSourceChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((TransitMap)sender).OnSelectedStopsSourceChanged(e.OldValue, e.NewValue);
        }
        
        public ICommand StopTitleClickedCommand
        {
            get { return (ICommand)GetValue(StopTitleClickedCommandProperty); }
            set { SetValue(StopTitleClickedCommandProperty, value); }
        }
        public static readonly DependencyProperty StopTitleClickedCommandProperty =
            DependencyProperty.Register("StopTitleClickedCommand", typeof(ICommand), typeof(TransitMap), new PropertyMetadata(null));
        
        public bool HasSelectedStops
        {
            get { return (bool)GetValue(HasSelectedStopsProperty); }
            private set { SetValue(HasSelectedStopsProperty, value); }
        }
        public static readonly DependencyProperty HasSelectedStopsProperty =
            DependencyProperty.Register("HasSelectedStops", typeof(bool), typeof(TransitMap), new PropertyMetadata(false));
        
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

        void OnSelectedStopsSourceChanged(object oldValue, object newValue)
        {
            if (oldValue is ObservableCollection<TransitStop>)
                UnregisterSelectedStopsSourceHandlers((ObservableCollection<TransitStop>)oldValue);
            if (newValue is ObservableCollection<TransitStop>)
                RegisterSelectedStopsSourceHandlers((ObservableCollection<TransitStop>)newValue);
            else if (newValue is IEnumerable<TransitStop>)
            {
                SetStopArrivalsControl(CombineSeveralStops(null, ((IEnumerable<TransitStop>)newValue).ToArray()));
            }
            else if (newValue is TransitStop)
                SetStopArrivalsControl((TransitStop)newValue);
            else
                ClearStopArrivalsControl();
        }

        private void OnAreaChanged()
        {
            DelaySetArea();
        }
        #endregion

        #region Private Actions
        private async void SetStopArrivalsControl(TransitStop stop)
        {
            if (ArrivalsViewModel == null)
                SetArrivalsViewModel();
            ArrivalsViewModel.Stop = stop;
            TrySetView(new MapView(stop.Position)).ToString();
            await Task.Delay(150);

            ArrivalsViewModel.SetVisibility();
        }

        private void ClearStopArrivalsControl()
        {
            ArrivalsViewModel.Stop = null;
            ArrivalsViewModel.SetVisibility();
        }

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

        private void SetArrivalsViewModel()
        {
            ArrivalsPopup = new StopPopupOuterControl() { RenderTransform = ArrivalsPopupTransform };
            ArrivalsViewModel = new ArrivalsControlInTransitPageViewModel();
            ArrivalsViewModel.VisibilityChangedCallback += async visible =>
            {
                DoubleAnimation opacityAnimation = new DoubleAnimation() { From = visible ? 0 : 1, To = visible ? 1 : 0, Duration = TimeSpan.FromMilliseconds(150) };
                DoubleAnimation slideAnimation = new DoubleAnimation() { From = visible ? 10 : 0, To = visible ? 0 : 10, Duration = TimeSpan.FromMilliseconds(150) };
                Storyboard.SetTarget(opacityAnimation, ArrivalsPopup);
                Storyboard.SetTargetProperty(opacityAnimation, "Opacity");
                Storyboard.SetTarget(slideAnimation, ArrivalsPopupTransform);
                Storyboard.SetTargetProperty(slideAnimation, "Y");
                Storyboard sb = new Storyboard();
                sb.Children.Add(opacityAnimation);
                sb.Children.Add(slideAnimation);
                if (visible)
                    ArrivalsViewModel.IsVisible = true;
                await sb.BeginAsync();
                if (!visible)
                    ArrivalsViewModel.IsVisible = false;
            };
            ArrivalsViewModel.BindToControl(OnMapPopup, MapControl.LocationProperty, "MapLocation", false, LatLonToGeopointConverter.Instance, "UnsetNAL");
            ArrivalsViewModel.BindToControl(ArrivalsPopup, StopPopupOuterControl.WidthProperty, "Width");
            ArrivalsViewModel.BindToControl(ArrivalsPopup, StopPopupOuterControl.HeightProperty, "Height");
            ArrivalsPopup.Visibility = Visibility.Visible;
            ArrivalsViewModel.BindToControl(ArrivalsPopup, StopPopupOuterControl.VisibilityProperty, "IsVisible", false, BoolToVisibilityConverter.Instance);
            ArrivalsViewModel.BindToControl(ArrivalsPopup, StopPopupOuterControl.DataContextProperty, "DataContext");
            ArrivalsViewModel.BindToControl(ArrivalsPopup, StopPopupOuterControl.ExpandCommandProperty, "ExpandCommand");
            ArrivalsViewModel.BindToControl(ArrivalsPopup, StopPopupOuterControl.CompressCommandProperty, "CompressCommand");
            ArrivalsViewModel.BindToControl(ArrivalsPopup, StopPopupOuterControl.CloseCommandProperty, "CloseCommand");
            ArrivalsViewModel.BindToControl(ArrivalsPopup, StopPopupOuterControl.ShowBottomArrowProperty, "ShowBottomArrow");
            ArrivalsViewModel.BindToControl(ArrivalsPopup, StopPopupOuterControl.ShowRoutesListProperty, "ShowRoutesList");
            ArrivalsViewModel.BindToControl(this, TransitMap.CenterRegionProperty, "CenterRegion");
            ArrivalsViewModel.PropertyChanged += ArrivalsViewModel_PropertyChanged;
            OnMapPopup.Content = ArrivalsPopup;
            ArrivalsViewModel.SetSize(MainMap.ActualWidth, MainMap.ActualHeight);
            ArrivalsPopup.SetBinding(StopPopupOuterControl.TitleCommandProperty, new Binding { Source = this, Path = new PropertyPath("StopTitleClickedCommand") });
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

        #region Event Registration
        private bool SelectedStopsSourceChangeHandled = false;
        private bool ArrivalsViewModelCloseEventHandled = false;

        private void RegisterSelectedStopsSourceHandlers(ObservableCollection<TransitStop> collection)
        {
            if (!SelectedStopsSourceChangeHandled)
            {
                collection.CollectionChanged += SelectedStopsSource_CollectionChanged;
                SelectedStopsSourceChangeHandled = true;
            }
        }

        private void UnregisterSelectedStopsSourceHandlers(ObservableCollection<TransitStop> collection)
        {
            if (SelectedStopsSourceChangeHandled)
            {
                collection.CollectionChanged -= SelectedStopsSource_CollectionChanged;
                SelectedStopsSourceChangeHandled = false;
            }
        }

        private void RegisterArrivalsViewModelCloseEventHandler()
        {
            if (!ArrivalsViewModelCloseEventHandled && ArrivalsViewModel != null)
            {
                ArrivalsViewModel.Closed += ArrivalsViewModel_Closed;
                ArrivalsViewModelCloseEventHandled = true;
            }
        }

        private void UnregisterArrivalsViewModelCloseEventHandler()
        {
            if (ArrivalsViewModelCloseEventHandled)
            {
                ArrivalsViewModel.Closed -= ArrivalsViewModel_Closed;
                ArrivalsViewModelCloseEventHandled = true;
            }
        }
        #endregion

        #region Functions
        private static TransitStop CombineSeveralStops(LatLon? center, params TransitStop[] stops)
        {
            if (stops.Length == 0)
                throw new ArgumentException("stops needs to contain at least one stop.", "stops");
            if (stops.Length == 1)
                return stops[0];
            TransitStop result = new TransitStop();
            result.Name = $"Selected Stops";
            result.Direction = Data.StopDirection.Unspecified;
            result.ID = stops.Aggregate("", (acc, stop) => acc + "&" + stop.ID).Substring(1);
            result.Position = center ?? stops[0].Position;
            result.Children = stops.Select(stop => stop.ID).ToArray();
            return result;
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
            if (ArrivalsViewModel != null)
                ArrivalsViewModel.SetSize(e.NewSize.Width, e.NewSize.Height);
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
            if (SelectedStopsSource is ObservableCollection<TransitStop>)
                RegisterSelectedStopsSourceHandlers((ObservableCollection<TransitStop>)SelectedStopsSource);
            RegisterArrivalsViewModelCloseEventHandler();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (SelectedStopsSource is ObservableCollection<TransitStop>)
                UnregisterSelectedStopsSourceHandlers((ObservableCollection<TransitStop>)SelectedStopsSource);
            UnregisterArrivalsViewModelCloseEventHandler();
        }

        private void SelectedStopsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            
            if (!(HasSelectedStops = !(((IEnumerable<TransitStop>)SelectedStopsSource).Count() == 0)))
            {
                if (ArrivalsViewModel.Stop.HasValue)
                {
                    ClearStopArrivalsControl();
                }
                return;
            }

            LatLon? center = null;
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems.Count == 1)
            {
                center = ((TransitStop)e.NewItems[0]).Position;
            }
            SetStopArrivalsControl(CombineSeveralStops(center, ((IEnumerable<TransitStop>)SelectedStopsSource).ToArray()));
        }

        private void ArrivalsViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsOnMap")
            {
                if (ArrivalsViewModel.IsOnMap && OffMapPopup.Content == ArrivalsPopup)
                {
                    OffMapPopup.Content = null;
                    OnMapPopup.Content = ArrivalsPopup;
                }
                else if (!ArrivalsViewModel.IsOnMap && OnMapPopup.Content == ArrivalsPopup)
                {
                    OnMapPopup.Content = null;
                    OffMapPopup.Content = ArrivalsPopup;
                }
            }
        }

        private void ArrivalsViewModel_Closed(object sender, EventArgs e)
        {
            if (SelectedStopsSource is ObservableCollection<TransitStop>)
            {
                ((ObservableCollection<TransitStop>)SelectedStopsSource).Clear();
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

        private CompositeCollectionBinding<MapElement, TransitMapAddInBase> MapElementBindings;
        private CompositeCollectionBinding<DependencyObject, TransitMapAddInBase> MapChildrenBindings;
        private CompositeCollectionBinding<MapRouteView, TransitMapAddInBase> MapRouteBindings;

        private void AddIns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (TransitMapAddInBase addin in e.NewItems)
                {
                    MapElementBindings.AddCollection(addin, addin.MapElementsShown);
                    MapRouteBindings.AddCollection(addin, addin.MapRoutesShown);
                    MapChildrenBindings.AddCollection(addin, addin.MapChildrenShown);
                }
            }
            if (e.OldItems != null)
            {
                foreach (TransitMapAddInBase addin in e.NewItems)
                {
                    MapElementBindings.RemoveCollection(addin);
                    MapRouteBindings.RemoveCollection(addin);
                    MapChildrenBindings.RemoveCollection(addin);
                }
            }
        }
        #endregion

        private void MainMap_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            foreach (var addin in AddIns)
            {
                
            }
        }

        private void MainMap_MapElementPointerEntered(MapControl sender, MapElementPointerEnteredEventArgs args)
        {
            foreach (var addin in AddIns)
            {

            }
        }

        private void MainMap_MapElementPointerExited(MapControl sender, MapElementPointerExitedEventArgs args)
        {
            foreach (var addin in AddIns)
            {

            }
        }
    }
}
