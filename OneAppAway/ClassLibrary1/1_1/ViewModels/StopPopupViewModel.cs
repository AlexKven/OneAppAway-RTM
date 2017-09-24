using MvvmHelpers;
using OneAppAway._1_1.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OneAppAway._1_1.ViewModels
{
    public class StopPopupViewModel : BaseViewModel
    {
        private CancellationTokenSource LoadRoutesTokenSource;
        private bool RoutesLoaded = false;

        private TransitStop _Stop;
        public TransitStop Stop
        {
            get { return _Stop; }
            set
            {
                SetProperty(ref _Stop, value);
                if (Stop.Children == null)
                    HasChildren = false;
                else
                    HasChildren = Stop.Children.Length > 0;
                LoadStopProperties();
            }
        }

        public StopPopupViewModel()
        {
            //if (Children != null && Children.Count > 0)
            //    Width = Children.Aggregate(0.0, (acc, child) => acc + child.Width);
            //else
            //    Width = 290;
            LoadRoutesTokenSource = new CancellationTokenSource();
            Children.CollectionChanged += Children_CollectionChanged;
        }

        private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
        }

        private ObservableCollection<TransitStop> _Children = new ObservableCollection<TransitStop>();
        public ObservableCollection<TransitStop> Children
        {
            get { return _Children; }
            private set { _Children = value; }
        }

        private async void LoadStopProperties()
        {
            RoutesLoaded = false;
            StopName = Stop.Name;
            TitleToolTip = $"Stop ID = {Stop.ID}";
            Children.Clear();
            if (Stop.Children != null)
            {
                foreach (var childID in Stop.Children)
                {
                    var child = await DataSource.GetTransitStopAsync(childID, DataSourcePreference.All, CancellationToken.None);
                    if (child.HasData)
                        Children.Add(child.Data);
                }
            }
            if (Children.Count > 0)
                StopName += $" ({Children.Count.ToString()} stops)";
            HasChildren = Children.Count > 0;
            string postfix = ((Stop.Direction == StopDirection.Unspecified) ? "BusBase" : ("BusDirection" + Stop.Direction.ToString()));
            IconUri = new Uri($"ms-appx:///Assets/Icons/{postfix}40.png");
            LoadRouteNames();
        }

        private async void LoadRouteNames()
        {
            try
            {
                RouteNames.Clear();
                if (Stop.Routes == null)
                    return;
                IsBusy = true;
                foreach (var routeId in Stop.Routes)
                {
                    //var route = await ApiLayer.GetTransitRoute(routeId, new System.Threading.CancellationToken());
                    var route = await DataSource.GetTransitRouteAsync(routeId, DataSourcePreference.All, LoadRoutesTokenSource.Token);
                    if (route.HasData)
                        RouteNames.Add(route.Data.Name);
                }
                RoutesLoaded = true;
            }
            catch (OperationCanceledException) { }
            finally
            {
                IsBusy = false;
            }
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

        private string _TitleToolTip;
        public string TitleToolTip
        {
            get { return _TitleToolTip; }
            set { SetProperty(ref _TitleToolTip, value); }
        }

        private bool _ShowSizeControls = false;
        public bool ShowSizeControls
        {
            get { return _ShowSizeControls; }
            set { SetProperty(ref _ShowSizeControls, value); }
        }

        private bool _HasChildren = false;
        public bool HasChildren
        {
            get { return _HasChildren; }
            set { SetProperty(ref _HasChildren, value); }
        }

        private Uri _IconUri;
        public Uri IconUri
        {
            get { return _IconUri; }
            set
            {
                SetProperty(ref _IconUri, value);
            }
        }

        private bool IsSettingTab = false;

        private bool _ShowArrivals = true;
        public bool ShowArrivals
        {
            get { return _ShowArrivals; }
            set
            {
                var old = ShowArrivals;
                SetProperty(ref _ShowArrivals, value);
                if (!ShowArrivals && old && !RoutesLoaded)
                {
                    LoadRoutesTokenSource.Cancel();
                    LoadRoutesTokenSource = new CancellationTokenSource();
                }
                else if (ShowArrivals && !old && !RoutesLoaded)
                {
                    LoadRouteNames();
                }
                if (IsSettingTab)
                    return;
                IsSettingTab = true;
                if (ShowArrivals)
                {
                    ShowSchedule = false;
                    ShowAlerts = false;
                }
                else
                    ShowSchedule = true;
                IsSettingTab = false;
            }
        }

        private bool _ShowSchedule = false;
        public bool ShowSchedule
        {
            get { return _ShowSchedule; }
            set
            {
                SetProperty(ref _ShowSchedule, value);
                if (IsSettingTab)
                    return;
                IsSettingTab = true;
                if (ShowSchedule)
                {
                    ShowAlerts = false;
                    ShowArrivals = false;
                }
                else
                    ShowArrivals = true;
                IsSettingTab = false;
            }
        }

        private bool _ShowAlerts = false;
        public bool ShowAlerts
        {
            get { return _ShowAlerts; }
            set
            {
                SetProperty(ref _ShowAlerts, value);
                if (IsSettingTab)
                    return;
                IsSettingTab = true;
                if (ShowAlerts)
                {
                    ShowSchedule = false;
                    ShowArrivals = false;
                }
                else
                    ShowArrivals = true;
                IsSettingTab = false;
            }
        }

        //public double Width { get; }

        //public bool IsTopLevel { get; private set; } = true;
    }
}
