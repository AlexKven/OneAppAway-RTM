using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.ViewManagement;
using Windows.Devices.Geolocation;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls.Maps;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Windows.Foundation;
using Windows.Storage.Streams;
using System.Linq;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using System.IO;
using System.Xml.Linq;
using System.Text;
using System.Diagnostics;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Data;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace OneAppAway
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BusMapPage : NavigationFriendlyPage
    {
        private Grid StopArrivalBoxGrid = new Grid() { Width = 350, Height = 400 };

        public BusMapPage()
        {
            this.InitializeComponent();
            MainMap.MapControl.ActualCameraChanged += MainMap_ActualCameraChanged;
            StopArrivalBoxGrid.SetBinding(Grid.VisibilityProperty, new Binding() { Source = StopArrivalBox, Path = new PropertyPath("Visibility") });
            MapControl.SetNormalizedAnchorPoint(StopArrivalBoxGrid, new Point(0.5, 1));
            MainMap.MapControl.Children.Add(StopArrivalBoxGrid);
            WindowStateChanging();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            WindowStateChanging();
        }

        public async void CenterOnCurrentLocation()
        {
            await SetLoadingIndicator(true);
            await Data.ProgressivelyAcquireLocation(async delegate (BasicGeoposition pos)
            {
                await MainMap.MapControl.TrySetViewAsync(new Geopoint(pos), 17, null, null, MapAnimationKind.Linear);
            });
            await SetLoadingIndicator(false);
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Window.Current.Bounds.Height / Window.Current.Bounds.Width > 1.5)
            {
                if (ButtonsPanel.Orientation != Orientation.Horizontal)
                {
                    ButtonsPanel.Orientation = Orientation.Horizontal;
                    ButtonsPanel.VerticalAlignment = VerticalAlignment.Bottom;
                    ButtonsPanel.HorizontalAlignment = HorizontalAlignment.Center;
                }
            }
            else
            {
                if (ButtonsPanel.Orientation != Orientation.Vertical)
                {
                    ButtonsPanel.Orientation = Orientation.Vertical;
                    ButtonsPanel.VerticalAlignment = VerticalAlignment.Center;
                    ButtonsPanel.HorizontalAlignment = HorizontalAlignment.Right;
                }
            }
        }

        #region Fields
        private long LastMove;
        private Task<BusStop[]> GetStopsTask;
        private CancellationTokenSource GetStopsCancellationTokenSource;
        private CancellationTokenSource MasterCancellationTokenSource = new CancellationTokenSource();
        #endregion

        #region Methods
        private void AddStopsInBounds(GeoboundingBox bounds)
        {
            Action<BusStop[], GeoboundingBox> stopsLoadedCallback = delegate (BusStop[] stops, GeoboundingBox bnd)
            {
                foreach (var stop in stops)
                {
                    if (!MainMap.ShownStops.Contains(stop))
                        MainMap.ShownStops.Add(stop);
                }
            };
            if (GetStopsTask != null && !GetStopsTask.IsCompleted)
            {
                GetStopsCancellationTokenSource.Cancel();
                //await GetStopsTask;
            }
            GetStopsCancellationTokenSource = new CancellationTokenSource();
            GetStopsTask = Data.GetBusStopsForArea(bounds, stopsLoadedCallback, GetStopsCancellationTokenSource.Token);
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(params string[] propertyNames)
        {
            if (PropertyChanged != null)
            {
                foreach (string propertyName in propertyNames)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        private async void MainMap_ActualCameraChanged(MapControl sender, MapActualCameraChangedEventArgs args)
        {
            GeoboundingBox bounds;
            try
            {
                bounds = new GeoboundingBox(MainMap.TopLeft, MainMap.BottomRight);
            }
            catch (Exception) { return; }
            long lm = DateTime.Now.Ticks;
            LastMove = lm;
            await Task.Delay(50);
            if (LastMove == lm)
            {
                if (MainMap.ZoomLevel < MainMap.StopVisibilityThreshold) return;
                try
                {
                    AddStopsInBounds(bounds);
                }
                catch (TaskCanceledException) { }
            }
        }

        private void StopArrivalBox_CloseRequested(object sender, EventArgs e)
        {
            VisualStateManager.GoToState(this, "ArrivalBoxHidden", true);
        }

        private void WindowSizeVisualStates_CurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            WindowStateChanging();
        }

        private void WindowStateChanging()
        {
            if (WindowSizeVisualStates.CurrentState.Name == "NarrowState")
            {
                if (StopArrivalBoxGrid.Children.Contains(StopArrivalBox))
                    StopArrivalBoxGrid.Children.Remove(StopArrivalBox);
                if (!MainGrid.Children.Contains(StopArrivalBox))
                    MainGrid.Children.Add(StopArrivalBox);
            }
            else if (WindowSizeVisualStates.CurrentState.Name == "NormalState")
            {
                if (MainGrid.Children.Contains(StopArrivalBox))
                    MainGrid.Children.Remove(StopArrivalBox);
                if (!StopArrivalBoxGrid.Children.Contains(StopArrivalBox))
                    StopArrivalBoxGrid.Children.Add(StopArrivalBox);
            }
        }

        private void ArrivalBoxVisualStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            CanGoBack = (e.NewState.Name == "ArrivalBoxShown");
        }

        protected override void OnGoBack(ref bool handled)
        {
            handled = CanGoBack;
            VisualStateManager.GoToState(this, "ArrivalBoxHidden", true);
        }

        protected override void OnSaveState(Dictionary<string, object> state)
        {
            state.Add("Lat", MainMap.Center.Latitude);
            state.Add("Lon", MainMap.Center.Longitude);
            state.Add("Zoom", MainMap.ZoomLevel);
            if (ArrivalBoxVisualStates?.CurrentState?.Name == "ArrivalBoxShown")
            {
                state.Add("Stops", StopArrivalBox.GetStops().Select(bs => bs.ID).ToArray());
                var boxPosition = MapControl.GetLocation(StopArrivalBoxGrid);
                state.Add("StopsLat", boxPosition.Position.Latitude);
                state.Add("StopsLon", boxPosition.Position.Longitude);
            }
        }

        protected override async void OnLoadState(Dictionary<string, object> state, object navigationParameter)
        {
            if (state != null && state.ContainsKey("Lat") && state.ContainsKey("Lon") && state.ContainsKey("Zoom"))
            {
                MainMap.Center = new BasicGeoposition() { Latitude = (double)state["Lat"], Longitude = (double)state["Lon"] };
                MainMap.ZoomLevel = (double)state["Zoom"];
                if (state.ContainsKey("Stops") && state.ContainsKey("StopsLat") && state.ContainsKey("StopsLon"))
                {
                    var location = new BasicGeoposition() { Latitude = (double)state["StopsLat"], Longitude = (double)state["StopsLon"] };
                    var stopIds = (string[])state["Stops"];
                    BusStop[] stops = new BusStop[stopIds.Length];
                    for (int i = 0; i < stopIds.Length; i++)
                        stops[i] = await Data.GetBusStop(stopIds[i], MasterCancellationTokenSource.Token);
                    OnStopsClicked(stops, location);
                    CanGoBack = true;
                }
            }
            else if (navigationParameter?.ToString() == "CurrentLocation")
            {
                CenterOnCurrentLocation();

            }
        }

        private async Task SetLoadingIndicator(bool value)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var bar = StatusBar.GetForCurrentView();
                if (value)
                    await bar.ProgressIndicator.ShowAsync();
                else
                    await bar.ProgressIndicator.HideAsync();
            }
            else
            {
                LoadingIndicator.IsIndeterminate = value;
                LoadingIndicator.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void BusMap_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ZoomLevel" && MainMap.ZoomLevel < MainMap.StopVisibilityThreshold && MainMap.ShownStops.Count > 0)
                MainMap.ShownStops.Clear(); 
        }

        private void MainMap_StopsClicked(object sender, StopClickedEventArgs e)
        {
            OnStopsClicked(e.Stops, e.Center);
        }

        private async void OnStopsClicked(BusStop[] stops, BasicGeoposition location)
        {
            BasicGeoposition newCenter = location;
            newCenter.Latitude = location.Latitude + (MainMap.TopLeft.Latitude - MainMap.BottomRight.Latitude) / 2 - 50 * MainMap.LatitudePerPixel;
            double halfLatSpan = (MainMap.TopLeft.Latitude - MainMap.BottomRight.Latitude) / 2.5;
            double halfLonSpan = (MainMap.BottomRight.Longitude - MainMap.TopLeft.Longitude) / 2.5;
            await MainMap.MapControl.TrySetViewBoundsAsync(new GeoboundingBox(new BasicGeoposition() { Latitude = newCenter.Latitude + halfLatSpan, Longitude = newCenter.Longitude - halfLonSpan },
                new BasicGeoposition() { Latitude = newCenter.Latitude - halfLatSpan, Longitude = newCenter.Longitude + halfLonSpan }), null, MapAnimationKind.Linear);

            MapControl.SetLocation(StopArrivalBoxGrid, new Geopoint(location));
            StopArrivalBox.SetStops(stops);
            VisualStateManager.GoToState(this, "ArrivalBoxShown", true);
        }

        private void ArrivalBoxVisualStates_CurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            DoubleAnimation ani = new DoubleAnimation() { To = e.NewState.Name == "ArrivalBoxShown" ? 1 : 0, Duration = TimeSpan.FromSeconds(0.15) };
            Storyboard sb = new Storyboard();
            Storyboard.SetTarget(ani, StopArrivalBox);
            Storyboard.SetTargetProperty(ani, "Opacity");
            sb.Children.Add(ani);
            if (e.NewState.Name == "ArrivalBoxShown")
                StopArrivalBox.Visibility = Visibility.Visible;
            else
                ani.Completed += (s, ev) => StopArrivalBox.Visibility = Visibility.Collapsed;
            sb.Begin();
        }

        private async void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            await MainMap.MapControl.TryZoomToAsync(MainMap.ZoomLevel + .5);
        }

        private async void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            await MainMap.MapControl.TryZoomToAsync(MainMap.ZoomLevel - .5);
        }
    }
}
