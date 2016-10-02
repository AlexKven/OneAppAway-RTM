using OneAppAway._1_1.Converters;
using OneAppAway._1_1.Data;
using OneAppAway._1_1.Views.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Data;
using Windows.Foundation;
using OneAppAway._1_1.Addins;
using OneAppAway._1_1.Helpers;
using System.Collections.Specialized;

namespace OneAppAway._1_1.AddIns
{
    public class ShownStopsAddIn : TransitMapAddInBase
    {
        #region Fields
        //private StopSizeThresholdConverter StopSizeConverter = new StopSizeThresholdConverter() { LargeThreshold = 18, MediumThreshold = 16.5, SmallThreshold = 14 };
        
        private List<string> HiddenStops = new List<string>();
        private List<TransitStopIconWrapper> StopIconWrappers = new List<TransitStopIconWrapper>();
        private WeakEventListener<ShownStopsAddIn, object, NotifyCollectionChangedEventArgs> StopsSource_CollectionChanged_Listener;
        #endregion

        #region Properties
        public ICommand StopsClickedCommand
        {
            get { return (ICommand)GetValue(StopsClickedCommandProperty); }
            set { SetValue(StopsClickedCommandProperty, value); }
        }
        public static readonly DependencyProperty StopsClickedCommandProperty =
            DependencyProperty.Register("StopsClickedCommand", typeof(ICommand), typeof(TransitMap), new PropertyMetadata(null));

        public object StopsSource
        {
            get { return (object)GetValue(StopsSourceProperty); }
            set { SetValue(StopsSourceProperty, value); }
        }
        public static readonly DependencyProperty StopsSourceProperty =
            DependencyProperty.Register("StopsSource", typeof(object), typeof(TransitMap), new PropertyMetadata(null, OnStopsSourceChangedStatic));
        static void OnStopsSourceChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((ShownStopsAddIn)sender).OnStopsSourceChanged(e.OldValue, e.NewValue);
        }

        public MapStopSize StopSize
        {
            get { return (MapStopSize)GetValue(StopSizeProperty); }
            set { SetValue(StopSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StopSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StopSizeProperty =
            DependencyProperty.Register("StopSize", typeof(MapStopSize), typeof(ShownStopsAddIn), new PropertyMetadata(MapStopSize.Medium, OnStopSizeChangedStatic));
        static void OnStopSizeChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var wrappers = (sender as ShownStopsAddIn)?.StopIconWrappers;
            if (wrappers == null)
                return;
            foreach (var wrapper in wrappers)
                wrapper.StopSize = (MapStopSize)e.NewValue;
        }
        #endregion

        #region Methods
        public override void OnMapElementPointerEntered(MapElement element, LatLon pointOnMap, Point pointOnControl)
        {
            var wrapper = StopIconWrappers.FirstOrDefault(w => w.Element == element);
            if (wrapper != null)
                wrapper.Hovered = true;
        }

        public override void OnMapElementPointerExited(MapElement element, LatLon pointOnMap, Point pointOnControl)
        {
            var wrapper = StopIconWrappers.FirstOrDefault(w => w.Element == element);
            if (wrapper != null)
                wrapper.Hovered = false;
        }

        public override void OnMapElementsClicked(IEnumerable<MapElement> elements, LatLon pointOnMap, Point pointOnControl)
        {
            if (elements != null && elements.Count() > 0)
            {
                //if (ArrivalsViewModel == null)
                //    SetArrivalsViewModel();
                var stops = StopIconWrappers.Where(w => elements.Contains(w.Element)).Select(w => w.Stop);
                //ArrivalsViewModel.Stop = stop;
                //if (stop.HasValue)
                //{
                //    TrySetView(stop.Value.Position).ToString();
                //    await Task.Delay(150);
                //}
                //ArrivalsViewModel.SetVisibility();
                if (StopsClickedCommand.CanExecute(stops))
                    StopsClickedCommand.Execute(stops);
            }
        }

        private void RegisterStopsSourceHandlers(ObservableCollection<TransitStop> collection)
        {
            if (StopsSource_CollectionChanged_Listener == null)
            {
                StopsSource_CollectionChanged_Listener = new WeakEventListener<ShownStopsAddIn, object, NotifyCollectionChangedEventArgs>(this);
                StopsSource_CollectionChanged_Listener.OnEventAction = (addin, sender, e) => addin.StopsSource_CollectionChanged(sender, e);
                StopsSource_CollectionChanged_Listener.OnDetachAction = (listener) => listener.OnEventAction = null;
                collection.CollectionChanged += StopsSource_CollectionChanged_Listener.OnEvent;
            }
        }

        private void UnregisterStopsSourceHandlers(ObservableCollection<TransitStop> collection)
        {
            if (StopsSource_CollectionChanged_Listener != null)
            {
                collection.CollectionChanged -= StopsSource_CollectionChanged_Listener.OnEvent;
                StopsSource_CollectionChanged_Listener?.Detach();
                StopsSource_CollectionChanged_Listener = null;
            }
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
        
        private void AddStopsToMap(params TransitStop[] stops)
        {
            foreach (var stop in stops)
            {
                TransitStopIconWrapper wrapper = new TransitStopIconWrapper(stop) { StopSize = this.StopSize };
                if (stop.Children != null)
                {
                    HiddenStops.AddRange(stop.Children);
                    RemoveStopsFromMap(stop.Children);
                }
                StopIconWrappers.Add(wrapper);
                if (!HiddenStops.Contains(stop.ID))
                    MapElementsShown.Add(wrapper.Element);
            }
        }

        private void ClearStops()
        {
            foreach (var item in StopIconWrappers)
                MapElementsShown.Remove(item.Element);
            StopIconWrappers.Clear();
        }

        private void RemoveStopsFromMap(params string[] stops)
        {
            foreach (var item in StopIconWrappers.ToArray())
            {
                if (stops.Contains(AttachedProperties.GetElementID(item.Element)))
                {
                    MapElementsShown.Remove(item.Element);
                    StopIconWrappers.Remove(item);
                }
            }
        }
        #endregion

        #region Event Handlers
        private void StopsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
                    RemoveStopsFromMap(e.OldItems.Cast<TransitStop>().Select(stop => stop.ID).ToArray());
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    RemoveStopsFromMap(e.OldItems.Cast<TransitStop>().Select(stop => stop.ID).ToArray());
                    AddStopsToMap(e.NewItems.Cast<TransitStop>().ToArray());
                    break;
            }
        }
        #endregion
    }
}
