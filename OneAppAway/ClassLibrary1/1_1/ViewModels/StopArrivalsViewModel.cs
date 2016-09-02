using MvvmHelpers;
using OneAppAway._1_1.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.ViewModels
{
    public class StopArrivalsViewModel : BaseViewModel, IDisposable
    {
        private MemoryCache Cache = new MemoryCache();
        public TransitStop Stop { get; }
        public StopArrivalsViewModel(TransitStop stop)
        {
            Stop = stop;
            if (stop.Children != null)
            {
                foreach (var childID in stop.Children)
                {
                    //var child = TransitStop.SqlProvider.Select(DatabaseManager.MemoryDatabase, $"ID = '{childID}'").FirstOrDefault();
                    var child = MemoryCache.GetStop(childID);
                    if (child.HasValue)
                        ChildrenSource.Add(new StopArrivalsViewModel(child.Value) { IsTopLevel = false }); //Nested ViewModels!
                }
            }
            if (ChildrenSource != null && ChildrenSource.Count > 0)
                Width = ChildrenSource[0].Width + 50;
            else
                Width = 290;
            StopName = stop.Name;
            if (ChildrenSource.Count > 0)
                StopName += $" ({ChildrenSource.Count.ToString()} stops)";
            HasChildren = ChildrenSource.Count > 0;
            string postfix = ((stop.Direction == StopDirection.Unspecified) ? "BusBase" : ("BusDirection" + stop.Direction.ToString()));
            _IconUri = new Uri($"ms-appx:///Assets/Icons/{postfix}40.png");
            LoadRouteNames();
        }

        private ObservableCollection<StopArrivalsViewModel> _ChildrenSource = new ObservableCollection<StopArrivalsViewModel>();
        public ObservableCollection<StopArrivalsViewModel> ChildrenSource
        {
            get { return _ChildrenSource; }
            private set { _ChildrenSource = value; }
        }

        private async void LoadRouteNames()
        {
            if (Stop.Routes == null)
                return;
            IsBusy = true;
            foreach (var routeId in Stop.Routes)
            {
                var route = await ApiLayer.GetTransitRoute(routeId, new System.Threading.CancellationToken());
                if (route.HasValue)
                    RouteNames.Add(route.Value.Name);
            }
            IsBusy = false;
        }

        public void Dispose()
        {
            Cache.Dispose();
        }

        private ObservableCollection<string> _RouteNames = new ObservableCollection<string>();
        public ObservableCollection<string> RouteNames
        {
            get { return _RouteNames; }
        }

        private string _StopName;
        public string StopName
        {
            get { return _StopName; }
            set
            {
                SetProperty(ref _StopName, value);
            }
        }

        public bool HasChildren { get; }

        private Uri _IconUri;
        public Uri IconUri
        {
            get { return _IconUri; }
        }

        public double Width { get; }

        public bool IsTopLevel { get; private set; } = true;
    }
}
