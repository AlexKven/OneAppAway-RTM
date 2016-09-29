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

namespace OneAppAway._1_1.AddIns
{
    public class ShownStopsAddIn : TransitMapAddInBase
    {
        #region Fields
        private StopSizeThresholdConverter StopSizeConverter = new StopSizeThresholdConverter() { LargeThreshold = 18, MediumThreshold = 16.5, SmallThreshold = 14 };

        private bool StopsSourceChangeHandled = false;
        private List<string> HiddenStops = new List<string>();
        private List<TransitStopIconWrapper> StopIconWrappers = new List<TransitStopIconWrapper>();
        #endregion

        #region Properties
        public double SmallThreshold
        {
            get { return StopSizeConverter.SmallThreshold; }
            set { StopSizeConverter.SmallThreshold = value; }
        }

        public double MediumThreshold
        {
            get { return StopSizeConverter.MediumThreshold; }
            set { StopSizeConverter.MediumThreshold = value; }
        }

        public double LargeThreshold
        {
            get { return StopSizeConverter.LargeThreshold; }
            set { StopSizeConverter.LargeThreshold = value; }
        }

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
        #endregion

        #region Methods
        private void MainMap_MapElementPointerEntered(MapElementPointerEnteredEventArgs args)
        {
            var wrapper = StopIconWrappers.FirstOrDefault(w => w.Element == args.MapElement);
            if (wrapper != null)
                wrapper.Hovered = true;
        }

        private void MainMap_MapElementPointerExited(MapElementPointerExitedEventArgs args)
        {
            var wrapper = StopIconWrappers.FirstOrDefault(w => w.Element == args.MapElement);
            if (wrapper != null)
                wrapper.Hovered = false;
        }

        private void MainMap_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            //foreach (var el in args.MapElements)
            //{
            //}
            if (args.MapElements != null && args.MapElements.Count > 0)
            {
                //if (ArrivalsViewModel == null)
                //    SetArrivalsViewModel();
                var stops = StopIconWrappers.Where(w => w.Element == args.MapElements[0]).Select(w => w.Stop);
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
            if (!StopsSourceChangeHandled)
            {
                collection.CollectionChanged += StopsSource_CollectionChanged;
                StopsSourceChangeHandled = true;
            }
        }

        private void UnregisterStopsSourceHandlers(ObservableCollection<TransitStop> collection)
        {
            if (StopsSourceChangeHandled)
            {
                collection.CollectionChanged -= StopsSource_CollectionChanged;
                StopsSourceChangeHandled = false;
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
                TransitStopIconWrapper wrapper = new TransitStopIconWrapper(stop);
                BindingOperations.SetBinding(wrapper, TransitStopIconWrapper.StopSizeProperty, new Binding() { Source = this, Path = new PropertyPath("ZoomLevelDelay"), Mode = BindingMode.OneWay, Converter = StopSizeConverter });
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
