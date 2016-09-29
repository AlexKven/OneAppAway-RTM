using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace OneAppAway
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RouteViewPage : NavigationFriendlyPage
    {
        public RouteViewPage()
        {
            this.InitializeComponent();
            MainMap.MapControl.ZoomInteractionMode = MapInteractionMode.GestureAndControl;
            VisualStateManager.GoToState(this, "MapState", false);
        }

        private CancellationTokenSource MasterCancellationTokenSource = new CancellationTokenSource();

        private BusRoute Route;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Message.ShowMessage(new Message() { ShortSummary = "Not every trip serves every stop.", Caption = "Route Map Warning", FullText = "The path shown on the map is every path driven by this route in any circumstance. While sometimes every trip serves every stop, often times trips terminate earlier than others. Also, some routes have special extended trips that aren't normally served by the route, and this map shows all of them. A future update will allow you to more easily see which trips serve which stops.", Id = 4 });
            MainMap.Background = new SolidColorBrush(Colors.Black);
            if (e.Parameter is string)
            {
                Route = (await Data.GetRoute(e.Parameter.ToString(), MasterCancellationTokenSource.Token)).Value;
                RouteNameBlock.Text = (char.IsNumber(Route.Name.FirstOrDefault()) ? "Route " : "") + Route.Name;
                var stopInfo = await Data.GetStopsAndShapesForRoute(Route.ID, new DataRetrievalOptions(DataSourceDescriptor.Local), MasterCancellationTokenSource.Token);
                if (stopInfo.Item2.FinalSource != null)
                {
                    foreach (var stop in stopInfo.Item1.Item1)
                        MainMap.ShownStops.Add(stop);
                    List<BasicGeoposition> allPoints = new List<BasicGeoposition>();
                    foreach (var shape in stopInfo.Item1.Item2)
                    {
                        var points = HelperFunctions.DecodeShape(shape);
                        MainMap.MapControl.MapElements.Add(new MapPolyline() { Path = new Windows.Devices.Geolocation.Geopath(points), StrokeColor = (Color)App.Current.Resources["SystemColorControlAccentColor"], StrokeThickness = 4, ZIndex = 0 });
                        allPoints.AddRange(points);
                    }
                    if (allPoints.Count > 0)
                    {
                        GeoboundingBox box = GeoboundingBox.TryCompute(allPoints);
                        
                        await MainMap.MapControl.TrySetViewBoundsAsync(box, new Thickness(0), MapAnimationKind.Bow);
                    }
                }
                else
                {
                    await new MessageDialog("We could not download the data for the selected route, and you don't have that route downloaded.", "Error getting route").ShowAsync();
                }
            }
        }

        private void MainMap_StopsClicked(object sender, StopClickedEventArgs e)
        {
            ArrivalsBox.SetStops(e.Stops);
            RefreshState(true);
        }

        private void DisplayStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            CanGoBack = (e.NewState?.Name != "MapState");
        }

        protected override void OnGoBack(ref bool handled)
        {
            base.OnGoBack(ref handled);
            if (DisplayStates.CurrentState?.Name != "MapState")
            {
                VisualStateManager.GoToState(this, "MapState", false);
                handled = true;
            }
        }

        private void RefreshState(bool forceArrivalState = false)
        {
            if ((DisplayStates.CurrentState?.Name != "MapState" && DisplayStates.CurrentState?.Name != null) || forceArrivalState)
            {
                if (ActualWidth < 500 && DisplayStates.CurrentState?.Name != "ArrivalsStateThin")
                    VisualStateManager.GoToState(this, "ArrivalsStateThin", false);
                else if (ActualWidth >= 600 && DisplayStates.CurrentState?.Name != "ArrivalsStateNormal")
                    VisualStateManager.GoToState(this, "ArrivalsStateNormal", false);
            }
        }

        private void MainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshState();
        }
    }
}