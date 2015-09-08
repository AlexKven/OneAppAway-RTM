using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using OneAppAway.TemplateSelectors;
using System.Threading;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace OneAppAway
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StopViewPage : NavigationFriendlyPage
    {
        public StopViewPage()
        {
            this.NavigationCacheMode = NavigationCacheMode.Disabled;
            this.InitializeComponent();
        }

        private double? lonPP;
        private CancellationTokenSource MasterCancellationTokenSource = new CancellationTokenSource();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Message.ShowMessage(new Message() { ShortSummary = "Scheduled times are approximate.", Caption = "Schedules Warning", FullText = "Transit schedules are approximate, and not adjusted when the bus is running late. Please arrive at least 5 minutes early to avoid missing the bus. In addition, some express routes are known to arrive at freeway stops early. At these stops, you should come at least 10 minutes early. If you downloaded schedules, remember to update them after each service change.", Id = 3 });
            if (e.Parameter != null && e.Parameter is string)
            {
                SetPage((string)e.Parameter);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        private async void SetPage(string stopId)
        {
            Stop = (await Data.GetBusStop(stopId, MasterCancellationTokenSource.Token)).Value;
            TitleBlock.Text = Stop.Name;
            Uri imageUri = new Uri(Stop.Direction == StopDirection.Unspecified ? "ms-appx:///Assets/Icons/BusBase40.png" : "ms-appx:///Assets/Icons/BusDirection" + Stop.Direction.ToString() + "40.png");
            DirectionImage.Source = new BitmapImage(imageUri);
            MainScheduleBrowser.Stop = Stop;
            //MapIcon mico = new MapIcon();
            //mico.Location = new Geopoint(Stop.Position);
            //mico.Image = RandomAccessStreamReference.CreateFromUri(imageUri);
            //mico.NormalizedAnchorPoint = new Point(0.5, 0.5); 
            //MainMap.MapElements.Add(mico);
            //MainMap.MapElements.Add(mico);
            //MainMap.Center = new Geopoint(Stop.Position);
            MainMap.ShownStops.Add(Stop);
#pragma warning disable CS4014
            RefreshRoutes();
            ArrivalsBox.Stop = Stop;
            MainScheduleBrowser.LoadSchedule(false);
#pragma warning restore CS4014
            SetMapCenter();
        }

        private void SetMapCenter()
        {
            if (InnerGrid.ActualWidth == 0) return;
            if (lonPP == null)
            {
                Geopoint pointOutW;
                Geopoint pointOutE;
                MainMap.MapControl.GetLocationFromOffset(new Point(0, 0), out pointOutW);
                MainMap.MapControl.GetLocationFromOffset(new Point(InnerGrid.ActualWidth, 0), out pointOutE);
                lonPP = (pointOutE.Position.Longitude - pointOutW.Position.Longitude) / InnerGrid.ActualWidth;
            }
            MainMap.Center = new BasicGeoposition() { Latitude = Stop.Position.Latitude, Longitude = Stop.Position.Longitude - lonPP.Value * (InnerGrid.ActualWidth - 100 - InnerGrid.ActualWidth / 2) };
        }

        private BusStop Stop;

        private void MainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetInnerGridSize();
        }

        private void SetInnerGridSize()
        {
            InnerGrid.MinWidth = InnerGrid.ColumnDefinitions.Where(cd => cd.Width.IsStar).Count() * 285 + 200;
            if (MainGrid.ActualWidth > 0)
            {
                InnerGrid.Width = MainGrid.ActualWidth;
            }
        }

        private void SetColumns()
        {
            SetInnerGridSize();
        }

        private async Task RefreshRoutes()
        {
            RoutesProgressIndicator.IsActive = true;
            RoutesControl.Items.Clear();
            bool intercityReminder = false;
            foreach (string rte in Stop.Routes)
            {
                BusRoute? route = await Data.GetRoute(rte, MasterCancellationTokenSource.Token);
                if (route != null)
                    RoutesControl.Items.Add(new RouteListingTemplateSelector.RouteListing() { Name = route.Value.Name, Description = route.Value.Description, Agency = (await Data.GetTransitAgency(route.Value.Agency, MasterCancellationTokenSource.Token)).Value.Name, RouteId = route.Value.ID });
                if (route.Value.Agency == "19" && !intercityReminder)
                {
                    intercityReminder = true;
                    Message.ShowMessage(new Message() { Caption = "Intercity Transit Warning", ShortSummary = "Schedules and arrivals may appear incorrectly for Intercity Transit.", FullText = "Because of the data provided by Intercity Transit, all of Intercity Transit's routes (including Olympia Express) display either \"Inbound\" or \"Outbound\" as the destination, usually relative to Olympia but not always. IT also sometimes lumps two directions together as one, and for loop routes doesn't distinguish between an arrival and departure at Olympia TC. Thus, data for IT routes in this app is uncertain and risky, and it's recommended that you pick up a paper IT schedule avilable on IT buses.", Id = 5 });
                }
            }
            RoutesProgressIndicator.IsActive = false;
        }

        private void InnerGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetMapCenter();
        }

        private void RouteButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RouteViewPage), ((Button)sender).Tag.ToString());
        }

        private async void RefreshArrivalsButton_Click(object sender, RoutedEventArgs e)
        {
            await ArrivalsBox.RefreshArrivals(true);
        }
    }
}
