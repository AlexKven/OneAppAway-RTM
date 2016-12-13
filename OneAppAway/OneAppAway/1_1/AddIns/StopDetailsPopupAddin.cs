using OneAppAway._1_1.Data;
using OneAppAway._1_1.Helpers;
using OneAppAway._1_1.ViewModels;
using OneAppAway._1_1.Views.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Windows.Foundation;
using System.ComponentModel;
using Windows.UI.Xaml.Data;
using OneAppAway._1_1.Converters;
using OneAppAway.Common;
using static System.Math;
using Windows.UI.Xaml.Controls.Maps;

namespace OneAppAway._1_1.AddIns
{
    public class StopDetailsPopupAddIn : TransitMapAddInBase
    {
        #region Constants
        private const double COLUMN_SIZE = 310;
        private const double MAXIMIZED_MAP_MARGIN = 50;
        private const double ARRIVALS_CONTROL_TRIANGLE_HEIGHT = 30;
        private const double NORMAL_HEIGHT = 400;

        private const float SLIDE_OFFSET = 25;
        private const double ANIMATION_DURATION = 200;
        #endregion

        #region Fields
        private ContentControl OnMapPopup = new ContentControl() { Visibility = Visibility.Collapsed, HorizontalContentAlignment = HorizontalAlignment.Stretch, VerticalContentAlignment = VerticalAlignment.Stretch };
        private ContentControl OffMapPopup = new ContentControl() { Visibility = Visibility.Collapsed, HorizontalContentAlignment = HorizontalAlignment.Stretch, VerticalContentAlignment = VerticalAlignment.Stretch, Margin = new Thickness(0, 0, 0, 20) };
        private StopPopupOuterControl ArrivalsPopup = new StopPopupOuterControl() { Visibility = Visibility.Collapsed };

        private ObservableCollection<RealTimeArrival> ShownVehicleArrivals = new ObservableCollection<RealTimeArrival>();
        private CompositeCollectionBinding<RealTimeArrival, int> ShownVehicleArrivalsBinding;
        private Dictionary<Tuple<string, string>, TransitVehicleIconWrapper> VehicleWrappers = new Dictionary<Tuple<string, string>, TransitVehicleIconWrapper>();

        private double MapWidth;
        private double MapHeight;
        private double NumColsRequested = 1;
        private double MaxColsVisible
        {
            get
            {
                return Math.Floor(Math.Max(0, 2 * (MapWidth - 100 - COLUMN_SIZE / 2) / COLUMN_SIZE)) / 2 + 1;
            }
        }
        bool Maximized = false;

        private DisableableRelayCommand ExpandCommand;
        private DisableableRelayCommand CompressCommand;
        private DisableableRelayCommand CloseCommand;
        #endregion

        public StopDetailsPopupAddIn()
        {
            ExpandCommand = new DisableableRelayCommand((obj) =>
            {
                NumColsRequested += .5;
                RefreshPopupSize();
            });
            CompressCommand = new DisableableRelayCommand((obj) =>
            {
                do
                {
                    NumColsRequested -= .5;
                } while (NumColsRequested >= MaxColsVisible && NumColsRequested > 1);
                RefreshPopupSize();
            });
            CloseCommand = new DisableableRelayCommand((obj) => ClosePopup());
            MapControl.SetNormalizedAnchorPoint(OnMapPopup, new Point(0.5, 1));

            ArrivalsPopup.SetBinding(StopPopupOuterControl.TitleCommandProperty, new Binding() { Source = this, Path = new PropertyPath("StopTitleClickedCommand") });
            ArrivalsPopup.ExpandCommand = ExpandCommand;
            ArrivalsPopup.CompressCommand = CompressCommand;
            ArrivalsPopup.CloseCommand = CloseCommand;
            ArrivalsPopup.Offset(offsetY: SLIDE_OFFSET).Fade(value: 0).SetDurationForAll(0).Start();

            ShownVehicleArrivals.CollectionChanged += ShownVehicleArrivals_CollectionChanged;
            ShownVehicleArrivalsBinding = new CompositeCollectionBinding<RealTimeArrival, int>(ShownVehicleArrivals);
            ShownVehicleArrivalsBinding.AddCollection(0, ArrivalsPopup.ShownArrivals);

            MapChildrenShown.Add(OnMapPopup);

            RefreshPopupSize();

            ArrivalsPopup.ShownArrivals.CollectionChanged += ShownArrivals_CollectionChanged;
        }

        private void ShownArrivals_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Number of arrivals: {ArrivalsPopup.ShownArrivals.Count}.");
        }

        #region Properties
        public object SelectedStopsSource
        {
            get { return (object)GetValue(SelectedStopsSourceProperty); }
            set { SetValue(SelectedStopsSourceProperty, value); }
        }
        public static readonly DependencyProperty SelectedStopsSourceProperty =
            DependencyProperty.Register("SelectedStops", typeof(object), typeof(StopDetailsPopupAddIn), new PropertyMetadata(null, OnSelectedStopsSourceChangedStatic));
        static void OnSelectedStopsSourceChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((StopDetailsPopupAddIn)sender).OnSelectedStopsSourceChanged(e.OldValue, e.NewValue);
        }

        public ICommand StopTitleClickedCommand
        {
            get { return (ICommand)GetValue(StopTitleClickedCommandProperty); }
            set { SetValue(StopTitleClickedCommandProperty, value); }
        }
        public static readonly DependencyProperty StopTitleClickedCommandProperty =
            DependencyProperty.Register("StopTitleClickedCommand", typeof(ICommand), typeof(StopDetailsPopupAddIn), new PropertyMetadata(null));

        public bool HasSelectedStops
        {
            get { return (bool)GetValue(HasSelectedStopsProperty); }
            private set { SetValue(HasSelectedStopsProperty, value); }
        }
        public static readonly DependencyProperty HasSelectedStopsProperty =
            DependencyProperty.Register("HasSelectedStops", typeof(bool), typeof(StopDetailsPopupAddIn), new PropertyMetadata(false));
        
        private double _Width;
        public double Width
        {
            get { return _Width; }
            set
            {
                _Width = value;
                OnMapPopup.Width = Width;
            }
        }

        private double _Height;
        public double Height
        {
            get { return _Height; }
            set
            {
                _Height = value;
                OnMapPopup.Height = Height;
            }
        }
        #endregion

        #region Methods
        void OnSelectedStopsSourceChanged(object oldValue, object newValue)
        {
            if (oldValue is ObservableCollection<TransitStop>)
                UnregisterSelectedStopsSourceHandlers((ObservableCollection<TransitStop>)oldValue);
            if (newValue is ObservableCollection<TransitStop>)
                RegisterSelectedStopsSourceHandlers((ObservableCollection<TransitStop>)newValue);
            else if (newValue is IEnumerable<TransitStop>)
            {
                SetPopup(CombineSeveralStops(null, ((IEnumerable<TransitStop>)newValue).ToArray()));
            }
            else if (newValue is TransitStop)
                SetPopup((TransitStop)newValue);
            else
                ClosePopup();
        }

        private void SetPopup(TransitStop stop)
        {
            ArrivalsPopup.Stop = stop;
            if (ArrivalsPopup.Visibility != Visibility.Visible)
            {
                ArrivalsPopup.Visibility = Visibility.Visible;
                OnMapPopup.Visibility = Visibility.Visible;
                OffMapPopup.Visibility = Visibility.Visible;
                ArrivalsPopup.Offset(offsetY: 0).Fade(value: 1).SetDurationForAll(ANIMATION_DURATION).Start();
            }
            MapControl.SetLocation(OnMapPopup, stop.Position.ToGeopoint());
            SetTakeover(new MapView(stop.Position));
        }

        public override void OnSizeChanged(Size? previousSize, Size newSize)
        {
            MapWidth = newSize.Width;
            MapHeight = newSize.Height;
            RefreshPopupSize();
        }

        private void RefreshPopupSize()
        {
            if (NumColsRequested >= MaxColsVisible && !Maximized)
                Maximize();
            if (NumColsRequested < MaxColsVisible && Maximized)
                Restore();
            CompressCommand.IsEnabled = NumColsRequested > 1;
            ExpandCommand.IsEnabled = NumColsRequested < MaxColsVisible;
            Width = Max(0, Maximized ? MapWidth : NumColsRequested * COLUMN_SIZE);
            Height = Max(0, Maximized ? MapHeight - MAXIMIZED_MAP_MARGIN + ARRIVALS_CONTROL_TRIANGLE_HEIGHT : Min(NORMAL_HEIGHT, MapHeight - MAXIMIZED_MAP_MARGIN));
            ArrivalsPopup.ShowBottomArrow = Height > 275;
            ArrivalsPopup.ShowRoutesList = Height > 225;
            ArrivalsPopup.ShowCompactMenu = Height < 350;
        }

        private async void ClosePopup()
        {
            if (ArrivalsPopup.Visibility != Visibility.Collapsed)
                await ArrivalsPopup.Offset(offsetY: SLIDE_OFFSET).Fade(value: 0).SetDurationForAll(ANIMATION_DURATION).StartAsync();
            ArrivalsPopup.Visibility = Visibility.Collapsed;
            OnMapPopup.Visibility = Visibility.Collapsed;
            OffMapPopup.Visibility = Visibility.Collapsed;
            ArrivalsPopup.Stop = new TransitStop();
        }

        private void Maximize()
        {
            OnMapPopup.Content = null;
            OffMapPopup.Content = ArrivalsPopup;
            Maximized = true;
            SetTakeover();
        }

        private void Restore()
        {
            OffMapPopup.Content = null;
            OnMapPopup.Content = ArrivalsPopup;
            Maximized = false;
            SetTakeover();
        }

        private void SetTakeover(MapView requestedView = null)
        {
            if (ArrivalsPopup.Visibility == Visibility.Visible)
            {
                MapTakeover takeover;
                if (Maximized)
                    takeover = new MapTakeover(OffMapPopup, new RectSubset() { Top = MAXIMIZED_MAP_MARGIN, TopValueType = RectSubsetValueType.Length }, requestedView);
                else
                    takeover = new MapTakeover(null, new RectSubset() { Top = 0.2, TopValueType = RectSubsetValueType.Length, TopScale = RectSubsetScale.Relative }, requestedView);
                InvokeTakeoverRequested(takeover);
            }
            else
                InvokeTakeoverRequested(null);
        }

        private bool AddVehicle(RealTimeArrival arrival)
        {
            var key = new Tuple<string, string>(arrival.Stop, arrival.Trip);
            if (!VehicleWrappers.ContainsKey(key))
            {
                TransitVehicleIconWrapper tviw = new TransitVehicleIconWrapper();
                tviw.Arrival = arrival;
                MapElementsShown.Add(tviw.Element);
                VehicleWrappers.Add(key, tviw);
                return true;
            }
            return false;
        }

        private bool RemoveVehicle(RealTimeArrival arrival)
        {
            var key = new Tuple<string, string>(arrival.Stop, arrival.Trip);
            if (VehicleWrappers.ContainsKey(key))
            {
                var tviw = VehicleWrappers[key];
                MapElementsShown.Remove(tviw.Element);
                VehicleWrappers.Remove(key);
                return true;
            }
            return false;
        }

        private void ClearVehicles()
        {
            VehicleWrappers.Clear();
            MapElementsShown.Clear();
        }

        //private bool UpdateVehicle(RealTimeArrival arrival, bool skipCheck = false)
        //{
        //    var key = new Tuple<string, string>(arrival.Stop, arrival.Trip);
        //    if (VehicleWrappers.ContainsKey(key))
        //    {
        //        var tviw = VehicleWrappers[key];
        //        tviw.Arrival = arrival;
        //    }
        //    return false;
        //}

        #region Event Registration
        private WeakEventListener<StopDetailsPopupAddIn, object, NotifyCollectionChangedEventArgs> SelectedStopsSource_CollectionChanged_Listener;

        private void RegisterSelectedStopsSourceHandlers(ObservableCollection<TransitStop> collection)
        {
            if (SelectedStopsSource_CollectionChanged_Listener == null)
            {
                SelectedStopsSource_CollectionChanged_Listener = new WeakEventListener<StopDetailsPopupAddIn, object, NotifyCollectionChangedEventArgs>(this);
                SelectedStopsSource_CollectionChanged_Listener.OnEventAction = (listener, sender, e) => listener.SelectedStopsSource_CollectionChanged(sender, e);
                SelectedStopsSource_CollectionChanged_Listener.OnDetachAction = (listener) => listener.OnEventAction = null;
                collection.CollectionChanged += SelectedStopsSource_CollectionChanged_Listener.OnEvent;
            }
        }

        private void UnregisterSelectedStopsSourceHandlers(ObservableCollection<TransitStop> collection)
        {
            if (SelectedStopsSource_CollectionChanged_Listener != null)
            {
                SelectedStopsSource_CollectionChanged_Listener.Detach();
                SelectedStopsSource_CollectionChanged_Listener = null;
            }
        }

        //private void RegisterArrivalsViewModelCloseEventHandler()
        //{
        //    if (!ArrivalsViewModelCloseEventHandled && ArrivalsViewModel != null)
        //    {
        //        ArrivalsViewModel.Closed += ArrivalsViewModel_Closed;
        //        ArrivalsViewModelCloseEventHandled = true;
        //    }
        //}

        //private void UnregisterArrivalsViewModelCloseEventHandler()
        //{
        //    if (ArrivalsViewModelCloseEventHandled)
        //    {
        //        ArrivalsViewModel.Closed -= ArrivalsViewModel_Closed;
        //        ArrivalsViewModelCloseEventHandled = true;
        //    }
        //}
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
        #endregion

        #region Event Handlers
        private void SelectedStopsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            //if (!(HasSelectedStops = !(((IEnumerable<TransitStop>)SelectedStopsSource).Count() == 0)))
            //{
            //    if (ArrivalsViewModel.Stop.HasValue)
            //    {
            //        ClearStopArrivalsControl();
            //    }
            //    return;
            //}

            LatLon? center = null;
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems.Count == 1)
            {
                center = ((TransitStop)e.NewItems[0]).Position;
            }
            SetPopup(CombineSeveralStops(center, ((IEnumerable<TransitStop>)SelectedStopsSource).ToArray()));
        }

        //private void ArrivalsViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "IsOnMap")
        //    {
        //        if (ArrivalsViewModel.IsOnMap && OffMapPopup.Content == ArrivalsPopup)
        //        {
        //            OffMapPopup.Content = null;
        //            OnMapPopup.Content = ArrivalsPopup;
        //        }
        //        else if (!ArrivalsViewModel.IsOnMap && OnMapPopup.Content == ArrivalsPopup)
        //        {
        //            OnMapPopup.Content = null;
        //            OffMapPopup.Content = ArrivalsPopup;
        //        }
        //    }
        //}

        private void ArrivalsViewModel_Closed(object sender, EventArgs e)
        {
            if (SelectedStopsSource is ObservableCollection<TransitStop>)
            {
                ((ObservableCollection<TransitStop>)SelectedStopsSource).Clear();
            }
        }

        private void ShownVehicleArrivals_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Move)
                return;
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                ClearVehicles();
                foreach (var arrival in ShownVehicleArrivals)
                {
                    AddVehicle(arrival);
                }
            }
            else
            {
                if (e.NewItems != null)
                {
                    foreach (RealTimeArrival item in e.NewItems)
                    {
                        AddVehicle(item);
                    }
                }
                if (e.OldItems != null)
                {
                    foreach (RealTimeArrival item in e.OldItems)
                    {
                        RemoveVehicle(item);
                    }
                }
            }
        }
        #endregion
    }
}
