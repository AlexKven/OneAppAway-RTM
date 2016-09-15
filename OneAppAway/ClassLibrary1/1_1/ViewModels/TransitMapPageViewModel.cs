using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmHelpers;
using OneAppAway._1_1.Data;
using System.Threading;
using System.Net.Http;
using System.Windows.Input;
using System.Collections.Specialized;

namespace OneAppAway._1_1.ViewModels
{
    public abstract class TransitMapPageViewModel : BaseViewModel
    {
        public const double MAX_LAT_RANGE = 0.01;
        public const double MAX_LON_RANGE = 0.015;

        public TransitMapPageViewModel(MemoryCache cache)
        {
            StopsClickedCommand = new Command(StopsClickedCommand_Execute);
            NavigateToStopPageCommand = new Command(NavigateToStopPageCommand_Execute);
            CenterOnCurrentLocationCommand = new Command(CenterOnCurrentLocation_Execute);
            RefreshCommand = new Command(Refresh_Execute);
            SearchCommand = new Command(Search_Execute, obj => (obj?.ToString()?.Length ?? 0) > 4);
            GoToLocationCommand = new Command(GoToLocation_Execute);
            NavigatedToCommand = new Command(OnNavigatedTo);
            CancelRefreshCommand = new Command((obj) =>
            {
                TokenSource.Cancel();
                TokenSource = new CancellationTokenSource();
            });
            Cache = cache;
            NetworkManagerBase.Instance.NetworkTypeChanged += (s, e) => LoadFromSettings(); //*MemoryLeak*!!! Temporary
            LoadFromSettings();
        }

        #region Fields
        private RectSubset OuterStopCacheMargin = new RectSubset() { Left = -.25, LeftScale = RectSubsetScale.Relative, Right = -.25, RightScale = RectSubsetScale.Relative, Top = -.25, TopScale = RectSubsetScale.Relative, Bottom = -.25, BottomScale = RectSubsetScale.Relative };
        private Queue<LatLonRect> PendingRegions = new Queue<LatLonRect>();
        private CancellationTokenSource TokenSource = new CancellationTokenSource();
        private MemoryCache Cache;
        double lastZoomLevel = -1;
        private bool NonUISetProperties = false;
        #endregion

        #region Protected Properties
        protected abstract bool IsMultiSelectOn { get; }
        #endregion

        #region Properties
        #region Constant Properties
        public double SmallThreshold => 14;
        public double MediumThreshold => 16.5;
        public double LargeThreshold => 18;
        #endregion

        #region Collections
        public ObservableRangeCollection<TransitStop> ShownStops { get; } = new ObservableRangeCollection<TransitStop>();
        public ObservableRangeCollection<TransitStop> SelectedStops { get; } = new ObservableRangeCollection<TransitStop>();
        public ObservableRangeCollection<LocationSearchResult> SearchResults { get; } = new ObservableRangeCollection<LocationSearchResult>();
        #endregion

        #region Commands
        public ICommand StopsClickedCommand { get; }
        public ICommand NavigateToStopPageCommand { get; }
        public ICommand CenterOnCurrentLocationCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand CancelRefreshCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand GoToLocationCommand { get; }
        public ICommand NavigatedToCommand { get; }
        #endregion

        #region Main Map Control
        private bool _HasSelectedStops;
        public bool HasSelectedStops
        {
            get { return _HasSelectedStops; }
            set
            {
                SetProperty(ref _HasSelectedStops, value);
                CanGoBack = HasSelectedStops || IsSearchBoxOpen;
            }
        }

        private LatLonRect _Area = LatLonRect.NotAnArea;
        public LatLonRect Area
        {
            get { return _Area; }
            set
            {
                var old = Area;
                SetProperty(ref _Area, value);
                OnAreaChanged(old, value);
            }
        }

        private double _ZoomLevel = 1;
        public double ZoomLevel
        {
            get { return _ZoomLevel; }
            set
            {
                SetProperty(ref _ZoomLevel, value);
            }
        }

        private double _CurrentZoomRate = 0;
        public double CurrentZoomRate
        {
            get { return _CurrentZoomRate; }
            set { SetProperty(ref _CurrentZoomRate, value); }
        }
        #endregion

        #region Page
        private bool _CanGoBack;
        public bool CanGoBack
        {
            get { return _CanGoBack; }
            private set { SetProperty(ref _CanGoBack, value); }
        }
        #endregion

        #region Appbar Controls
        private bool _IsSearchBoxOpen = false;
        public bool IsSearchBoxOpen
        {
            get { return _IsSearchBoxOpen; }
            set
            {
                SetProperty(ref _IsSearchBoxOpen, value);
                CanGoBack = HasSelectedStops || IsSearchBoxOpen;
                if (!IsSearchBoxOpen)
                    SearchResults.Clear();
            }
        }

        private bool _ZoomInButtonPressed = false;
        public bool ZoomInButtonPressed
        {
            get { return _ZoomInButtonPressed; }
            set
            {
                SetProperty(ref _ZoomInButtonPressed, value);
                if (value)
                    OnZoom(true);
                else
                    CurrentZoomRate = 0;
            }
        }
        private async void OnZoom(bool zoomIn)
        {
            await Task.Delay(100);
            if (zoomIn ? ZoomInButtonPressed : ZoomOutButtonPressed)
                CurrentZoomRate = zoomIn ? 1 : -1;
            else
                ViewChangeRequested?.Invoke(this, new EventArgs<MapView>(new MapView(ZoomLevel + (zoomIn ? 1 : -1))));
        }

        private bool _ZoomOutButtonPressed = false;
        public bool ZoomOutButtonPressed
        {
            get { return _ZoomOutButtonPressed; }
            set
            {
                SetProperty(ref _ZoomOutButtonPressed, value);
                if (value)
                    OnZoom(false);
                else
                    CurrentZoomRate = 0;
            }
        }

        private bool _AutoDownloadArrivals;
        public bool AutoDownloadArrivals
        {
            get { return _AutoDownloadArrivals; }
            set
            {
                if (!NonUISetProperties)
                {
                    SetMutallyExclusiveProperties(ManuallyDownloadArrivalsMode.Never, value);
                    SetToSettings();
                }
                else
                    SetProperty(ref _AutoDownloadArrivals, value);
            }
        }

        private bool _ManuallyDownloadArrivalsGroups;
        public bool ManuallyDownloadArrivalsGroups
        {
            get { return _ManuallyDownloadArrivalsGroups; }
            set
            {
                if (!NonUISetProperties)
                {
                    SetMutallyExclusiveProperties(ManuallyDownloadArrivalsMode.GroupsOnly, value);
                    SetToSettings();
                }
                else
                    SetProperty(ref _ManuallyDownloadArrivalsGroups, value);
            }
        }

        private bool _ManuallyDownloadArrivalsAll;
        public bool ManuallyDownloadArrivalsAll
        {
            get { return _ManuallyDownloadArrivalsAll; }
            set
            {
                if (!NonUISetProperties)
                {
                    SetMutallyExclusiveProperties(ManuallyDownloadArrivalsMode.Always, value);
                    SetToSettings();
                }
                else
                    SetProperty(ref _ManuallyDownloadArrivalsAll, value);
            }
        }

        private bool _ManuallyDownloadStops;
        public bool ManuallyDownloadStops
        {
            get { return _ManuallyDownloadStops; }
            set
            {
                SetProperty(ref _ManuallyDownloadStops, value);
                if (!NonUISetProperties)
                    SetToSettings();
            }
        }

        private bool _IsFindingLocation = false;
        public bool IsFindingLocation
        {
            get { return _IsFindingLocation; }
            set { SetProperty(ref _IsFindingLocation, value); }
        }
        #endregion
        #endregion

        #region Methods
        //private void LoadSpecialStops()
        //{
        //    var fwtc = new TransitStop() { ID = "FWTC", Position = new LatLon(47.31753, -122.30486), Path = "_vx_HlnniVF??LJ??MF??nIG??MK??LG?", Name = "Federal Way Transit Center", Children = new[] { "1_80439", "1_80431", "1_80438", "1_80432", "1_80437", "1_80433", "3_27814", "3_29410" } };
        //    Cache.Add(fwtc);
        //}

        private void SetMutallyExclusiveProperties(ManuallyDownloadArrivalsMode mode, bool value)
        {
            NonUISetProperties = true;
            try
            {
                if (value)
                {
                    switch (mode)
                    {
                        case ManuallyDownloadArrivalsMode.Always:
                            AutoDownloadArrivals = false;
                            ManuallyDownloadArrivalsGroups = false;
                            ManuallyDownloadArrivalsAll = true;
                            break;
                        case ManuallyDownloadArrivalsMode.Never:
                            AutoDownloadArrivals = true;
                            ManuallyDownloadArrivalsGroups = false;
                            ManuallyDownloadArrivalsAll = false;
                            break;
                        case ManuallyDownloadArrivalsMode.GroupsOnly:
                            AutoDownloadArrivals = false;
                            ManuallyDownloadArrivalsGroups = true;
                            ManuallyDownloadArrivalsAll = false;
                            break;
                    }
                }
                else
                {
                    switch (mode)
                    {
                        case ManuallyDownloadArrivalsMode.Always:
                            AutoDownloadArrivals = false;
                            ManuallyDownloadArrivalsGroups = true;
                            ManuallyDownloadArrivalsAll = false;
                            break;
                        case ManuallyDownloadArrivalsMode.Never:
                            AutoDownloadArrivals = false;
                            ManuallyDownloadArrivalsGroups = true;
                            ManuallyDownloadArrivalsAll = false;
                            break;
                        case ManuallyDownloadArrivalsMode.GroupsOnly:
                            AutoDownloadArrivals = true;
                            ManuallyDownloadArrivalsGroups = false;
                            ManuallyDownloadArrivalsAll = false;
                            break;
                    }
                }
            }
            finally
            {
                NonUISetProperties = false;
            }
        }
        private void OnAreaChanged(LatLonRect oldArea, LatLonRect newArea)
        {
            if ((ZoomLevel < SmallThreshold && lastZoomLevel != -1) || (newArea.Span.Latitude > MAX_LAT_RANGE * 8 || newArea.Span.Longitude > MAX_LON_RANGE * 8))
            {
                ShownStops.Clear();
                PendingRegions.Clear();
            }
            else
            {
                IEnumerable<LatLonRect> newRegion;
                if (lastZoomLevel < SmallThreshold)
                    newRegion = new LatLonRect[] { newArea };
                else
                    newRegion = oldArea.GetNewRegion(newArea);
                AppendRegions(newRegion);
                if (!IsBusy)
                    RefreshShownStops(TokenSource.Token, !ManuallyDownloadStops);
            }
            lastZoomLevel = ZoomLevel;
        }

        private void AppendRegions(IEnumerable<LatLonRect> newRegion)
        {
            foreach (var piece in newRegion)
            {
                foreach (var subPiece in piece.Miniaturize(new LatLon(MAX_LAT_RANGE, MAX_LON_RANGE)))
                {
                    PendingRegions.Enqueue(subPiece);
                }
            }
        }

        protected abstract Task GetLocation(Action<LatLon> locationCallback);

        protected async void RefreshShownStops(CancellationToken token, bool download = false)
        {
            IsBusy = true;
            try
            {
                if (ZoomLevel < SmallThreshold)
                {
                    ShownStops.Clear();
                }
                else
                {
                    //var toRemove = ShownStops.Where(stop => !Area.ContainsLatLon(stop.Position)).ToArray();
                    //ShownStops.RemoveRange(toRemove);
                    var area = Area;
                    for (int i = 0; i < ShownStops.Count; i++)
                    {
                        if (!area.ApplySubset(OuterStopCacheMargin).ContainsLatLon(ShownStops[i].Position))
                        {
                            ShownStops.RemoveAt(i);
                            i--;
                        }
                    }
                    var fwtc = MemoryCache.GetStop("FWTC");
                    if (fwtc.HasValue && area.ContainsLatLon(fwtc.Value.Position))
                        ShownStops.Add(fwtc.Value);
                    while (PendingRegions.Count > 0)
                    {
                        var region = PendingRegions.Dequeue();
                        //var fwtc = new string[] { "1_80439", "1_80431", "1_80438", "1_80432", "1_80437", "1_80433", "3_27814", "3_29410" };
                        if (region.Intersects(area))
                        {
                            var sResult = await DataSource.GetTransitStopsForAreaAsync(region, download ? DataSourcePreference.All : DataSourcePreference.OfflineSources, token);
                            var stops = sResult.Data.Where(stop => !ShownStops.Contains(stop));
                            //var fwtcStop = new TransitStop() { ID = "FWTC", Position = new LatLon(47.31753, -122.30486), Path = "_vx_HlnniVF??LJ??MF??nIG??MK??LG?", Name = "Federal Way Transit Center" };
                            //TransitStop.SqlProvider.Insert(fwtcStop, DatabaseManager.MemoryDatabase);
                            if (stops == null)
                                return;
                            Cache.Add(stops.ToArray());
                            ShownStops.AddRange(stops);
                        }
                        //foreach (var stop in stops)
                        //{
                        //    if (!ShownStops.Any(stp => stp.ID == stop.ID))
                        //        ShownStops.Add(stop);
                        //}
                    }
                }
            }
            catch (HttpRequestException)
            {
                PendingRegions.Clear();
            }
            catch (OperationCanceledException)
            {

            }
            finally
            {
                IsBusy = false;
            }
        }

        private void LoadFromSettings()
        {
            NonUISetProperties = true;
            try
            {
                ManuallyDownloadStops = SettingsManagerBase.Instance.GetSetting($"{(NetworkManagerBase.Instance.UnlimitedNetwork ? "UnlimitedData" : "LimitedData")}.ManuallyDownloadStops", false, false);
                var downloadArrivalsMode = SettingsManagerBase.Instance.CurrentDownloadArrivalsMode;
                AutoDownloadArrivals = downloadArrivalsMode == ManuallyDownloadArrivalsMode.Never;
                ManuallyDownloadArrivalsAll = downloadArrivalsMode == ManuallyDownloadArrivalsMode.Always;
                ManuallyDownloadArrivalsGroups = downloadArrivalsMode == ManuallyDownloadArrivalsMode.GroupsOnly;
            }
            finally
            {
                NonUISetProperties = false;
            }
        }

        private void SetToSettings()
        {
            SettingsManagerBase.Instance.SetSetting($"{(NetworkManagerBase.Instance.UnlimitedNetwork ? "UnlimitedData" : "LimitedData")}.ManuallyDownloadStops", false, ManuallyDownloadStops);
            SettingsManagerBase.Instance.CurrentDownloadArrivalsMode = AutoDownloadArrivals ? ManuallyDownloadArrivalsMode.Never : ManuallyDownloadArrivalsGroups ? ManuallyDownloadArrivalsMode.GroupsOnly : ManuallyDownloadArrivalsMode.Always;
        }

        public void GoBack()
        {
            if (IsSearchBoxOpen)
                IsSearchBoxOpen = false;
            else
                SelectedStops.Clear();
        }

        protected virtual void OnNavigatedTo(object parameter)
        {
        }
        #endregion

        #region Event Handlers
        private void StopsClickedCommand_Execute(object parameter)
        {
            if (IsMultiSelectOn)
                SelectedStops.AddRange((IEnumerable<TransitStop>)parameter);
            else
                SelectedStops.ReplaceRange((IEnumerable<TransitStop>)parameter);
        }

        private void NavigateToStopPageCommand_Execute(object parameter)
        {

        }

        private async void CenterOnCurrentLocation_Execute(object parameter)
        {
            IsFindingLocation = true;
            try
            {
                await GetLocation((pos) =>
                {
                    if (ZoomLevel < 14)
                        OnViewChangeRequested(new MapView(pos, 16.75));
                    else
                        OnViewChangeRequested(new MapView(pos));
                });
            }
            finally
            {
                IsFindingLocation = false;
            }
        }

        private async void Refresh_Execute(object parameter)
        {
            if (ZoomLevel < SmallThreshold)
                return;
            AppendRegions(new LatLonRect[] { Area });
            while (IsBusy)
                await Task.Delay(100);
            RefreshShownStops(TokenSource.Token, true);
        }

        protected abstract void Search_Execute(object parameter);

        private void GoToLocation_Execute(object parameter)
        {
            if (parameter == null)
                return;
            if (parameter is LocationSearchResult)
            {
                parameter = ((LocationSearchResult)parameter).Location;
                IsSearchBoxOpen = false;
                SearchResults.Clear();
            }
            if (parameter is LatLon)
            {
                var pos = (LatLon)parameter;
                if (ZoomLevel < 14)
                    ViewChangeRequested?.Invoke(this, new EventArgs<MapView>(new MapView(pos, 16.75)));
                else
                    ViewChangeRequested?.Invoke(this, new EventArgs<MapView>(new MapView(pos)));
            }
        }
        #endregion

        #region Events
        public event EventHandler<EventArgs<MapView>> ViewChangeRequested;
        protected void OnViewChangeRequested(MapView view)
        {
            ViewChangeRequested?.Invoke(this, new EventArgs<MapView>(view));
        }
        #endregion

        //bool startRefreshShownStopsRunning = false;
        //private async void StartRefreshShownStops()
        //{
        //    if (startRefreshShownStopsRunning)
        //        StopsNeedRefreshing = true;
        //    else
        //    {
        //        StopsNeedRefreshing = true;
        //        StopsNeedRefreshing = true;
        //        while (StopsNeedRefreshing)
        //        {
        //            StopsNeedRefreshing = false;
        //            await RefreshShownStops(RefreshShownStopsTokenSource.Token);
        //        }
        //    }
        //}
    }
}
