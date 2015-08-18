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
        }

        private CancellationTokenSource MasterCancellationTokenSource = new CancellationTokenSource();

        private BusRoute Route;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
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
    }
}