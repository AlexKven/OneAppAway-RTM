using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;

namespace OneAppAway._1_1.Views.Controls
{
    public abstract class TransitMapAddInBase : FrameworkElement
    {
        private ObservableRangeCollection<MapElement> _MapElementsShown = new ObservableRangeCollection<MapElement>();
        private ObservableRangeCollection<DependencyObject> _MapChildrenShown = new ObservableRangeCollection<DependencyObject>();
        private ObservableRangeCollection<MapRouteView> _MapRoutesShown = new ObservableRangeCollection<MapRouteView>();
        public ObservableRangeCollection<MapElement> MapElementsShown => _MapElementsShown;
        public ObservableRangeCollection<DependencyObject> MapChildrenShown => _MapChildrenShown;
        public ObservableRangeCollection<MapRouteView> MapRoutesShown => _MapRoutesShown;


    }
}
