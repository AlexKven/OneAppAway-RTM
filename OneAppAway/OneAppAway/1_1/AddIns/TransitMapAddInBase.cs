using MvvmHelpers;
using OneAppAway._1_1.AddIns;
using OneAppAway._1_1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;

namespace OneAppAway._1_1.AddIns
{
    public abstract class TransitMapAddInBase : DependencyObject
    {
        private ObservableRangeCollection<MapElement> _MapElementsShown = new ObservableRangeCollection<MapElement>();
        private ObservableRangeCollection<DependencyObject> _MapChildrenShown = new ObservableRangeCollection<DependencyObject>();
        private ObservableRangeCollection<MapRouteView> _MapRoutesShown = new ObservableRangeCollection<MapRouteView>();
        public ObservableRangeCollection<MapElement> MapElementsShown => _MapElementsShown;
        public ObservableRangeCollection<DependencyObject> MapChildrenShown => _MapChildrenShown;
        public ObservableRangeCollection<MapRouteView> MapRoutesShown => _MapRoutesShown;

        public virtual void OnMapElementPointerExited(MapElement element, LatLon pointOnMap, Point pointOnControl) { }
        public virtual void OnMapElementPointerEntered(MapElement element, LatLon pointOnMap, Point pointOnControl) { }
        public virtual void OnMapElementsClicked(IEnumerable<MapElement> elements, LatLon pointOnMap, Point pointOnControl) { }

        public virtual void OnSizeChanged(Size? previousSize, Size newSize) { }

        public virtual void OnTakeoverGranted(MapTakeover takeover) { }
        public virtual void OnTakeoverEvicted(MapTakeover takeover) { }

        public event EventHandler<MapTakeoverRequestedEventArgs> TakeoverRequested;
        protected void InvokeTakeoverRequested(MapTakeover takeover)
        {
            TakeoverRequested?.Invoke(this, new MapTakeoverRequestedEventArgs(takeover));
        }
    }
}
