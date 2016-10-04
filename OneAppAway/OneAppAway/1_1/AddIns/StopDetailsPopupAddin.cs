using OneAppAway._1_1.Addins;
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

namespace OneAppAway._1_1.AddIns
{
    public class StopDetailsPopupAddIn : TransitMapAddInBase
    {
        #region Fields
        private ContentControl OnMapPopup = new ContentControl();
        private ContentControl OffMapPopup = new ContentControl();
        private StopPopupOuterControl ArrivalsPopup = new StopPopupOuterControl() { Width = 500 };
        #endregion

        public StopDetailsPopupAddIn()
        {
            MapChildrenShown.Add(OnMapPopup);
            SetArrivalsViewModel();
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
                SetStopArrivalsControl(CombineSeveralStops(null, ((IEnumerable<TransitStop>)newValue).ToArray()));
            }
            else if (newValue is TransitStop)
                SetStopArrivalsControl((TransitStop)newValue);
            else
                ClearStopArrivalsControl();
        }

        private async void SetStopArrivalsControl(TransitStop stop)
        {
            //if (ArrivalsViewModel == null)
            //    SetArrivalsViewModel();
            ArrivalsPopup.Stop = stop;
            //ArrivalsViewModel.Stop = stop;
            //TrySetView(new MapView(stop.Position)).ToString();
            await Task.Delay(150);
            //ArrivalsViewModel.SetVisibility();
        }

        public override void OnSizeChanged(Size? previousSize, Size newSize)
        {
            //ArrivalsViewModel.SetSize(newSize.Width, newSize.Height);
        }

        private void ClearStopArrivalsControl()
        {
            ArrivalsPopup.Stop = new TransitStop();
            //ArrivalsViewModel.Stop = null;
            //ArrivalsViewModel.SetVisibility();
        }

        private void SetArrivalsViewModel()
        {
            //ArrivalsViewModel = new ArrivalsControlInTransitPageViewModel();
            //ArrivalsViewModel.VisibilityChangedCallback += async visible =>
            //{
            //    if (visible)
            //    {
            //        ArrivalsViewModel.IsVisible = true;
            //        ArrivalsPopup.Offset(offsetY: 0).Fade(value: 1).SetDurationForAll(200).Start();
            //    }
            //    else
            //    {
            //        await ArrivalsPopup.Offset(offsetY: 20).Fade(value: 0).SetDurationForAll(200).StartAsync();
            //        ArrivalsViewModel.IsVisible = false;
            //    }
            //    //DoubleAnimation opacityAnimation = new DoubleAnimation() { From = visible ? 0 : 1, To = visible ? 1 : 0, Duration = TimeSpan.FromMilliseconds(150) };
            //    //DoubleAnimation slideAnimation = new DoubleAnimation() { From = visible ? 10 : 0, To = visible ? 0 : 10, Duration = TimeSpan.FromMilliseconds(150) };
            //    //Storyboard.SetTarget(opacityAnimation, ArrivalsPopup);
            //    //Storyboard.SetTargetProperty(opacityAnimation, "Opacity");
            //    //Storyboard.SetTarget(slideAnimation, ArrivalsPopupTransform);
            //    //Storyboard.SetTargetProperty(slideAnimation, "Y");
            //    //Storyboard sb = new Storyboard();
            //    //sb.Children.Add(opacityAnimation);
            //    //sb.Children.Add(slideAnimation);
            //    //if (visible)
            //    //    ArrivalsViewModel.IsVisible = true;
            //    //await sb.BeginAsync();
            //    //if (!visible)
            //    //    ArrivalsViewModel.IsVisible = false;
            //};
            ////ArrivalsViewModel.BindToControl(OnMapPopup, MapControl.LocationProperty, "MapLocation", false, LatLonToGeopointConverter.Instance, "UnsetNAL");
            //ArrivalsViewModel.BindToControl(ArrivalsPopup, StopPopupOuterControl.WidthProperty, "Width");
            //ArrivalsViewModel.BindToControl(ArrivalsPopup, StopPopupOuterControl.HeightProperty, "Height");
            //ArrivalsPopup.Visibility = Visibility.Visible;
            //ArrivalsViewModel.BindToControl(ArrivalsPopup, StopPopupOuterControl.VisibilityProperty, "IsVisible", false, BoolToVisibilityConverter.Instance);
            //ArrivalsViewModel.BindToControl(ArrivalsPopup, StopPopupOuterControl.DataContextProperty, "DataContext");
            //ArrivalsViewModel.BindToControl(ArrivalsPopup, StopPopupOuterControl.ExpandCommandProperty, "ExpandCommand");
            //ArrivalsViewModel.BindToControl(ArrivalsPopup, StopPopupOuterControl.CompressCommandProperty, "CompressCommand");
            //ArrivalsViewModel.BindToControl(ArrivalsPopup, StopPopupOuterControl.CloseCommandProperty, "CloseCommand");
            //ArrivalsViewModel.BindToControl(ArrivalsPopup, StopPopupOuterControl.ShowBottomArrowProperty, "ShowBottomArrow");
            //ArrivalsViewModel.BindToControl(ArrivalsPopup, StopPopupOuterControl.ShowRoutesListProperty, "ShowRoutesList");
            ////ArrivalsViewModel.BindToControl(this, TransitMap.CenterRegionProperty, "CenterRegion");
            //ArrivalsViewModel.PropertyChanged += ArrivalsViewModel_PropertyChanged;
            OnMapPopup.Content = ArrivalsPopup;
            ArrivalsPopup.SetBinding(StopPopupOuterControl.TitleCommandProperty, new Binding() { Source = this, Path = new PropertyPath("StopTitleClickedCommand") });
        }

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
            SetStopArrivalsControl(CombineSeveralStops(center, ((IEnumerable<TransitStop>)SelectedStopsSource).ToArray()));
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
        #endregion
    }
}
