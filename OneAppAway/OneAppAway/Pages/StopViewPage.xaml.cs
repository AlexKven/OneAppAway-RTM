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
        private bool ScheduleLoaded = false;
        private CancellationTokenSource MasterCancellationTokenSource = new CancellationTokenSource();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
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
            if (!ScheduleLoaded && BandwidthManager.EffectiveBandwidthOptions == BandwidthOptions.Normal)
                GetSchedule();
            else
                LoadSchedulesButton.Visibility = Visibility.Visible;
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

        private async Task GetSchedule()
        {
            bool technicalMode = SettingsManager.GetSetting("TechnicalMode", false, true);
            ScheduleProgressIndicator.IsActive = true;
            ScheduleNotAvailableBlock.Visibility = Visibility.Collapsed;
            LoadSchedulesButton.Visibility = Visibility.Collapsed;
            Schedule = await Data.GetScheduleForStop(Stop.ID, MasterCancellationTokenSource.Token);
            DayScheduleSelector.Items.Clear();
            DayScheduleSelector.SelectedIndex = -1;
            foreach (var day in technicalMode ? Schedule.TechnicalDayGroups : Schedule.DayGroups)
            {
                DayScheduleSelector.Items.Add(new ComboBoxItem() { Content = day.GetFriendlyName(), Tag = day });
            }
            if (DayScheduleSelector.Items.Count == 0)
            {
                DayScheduleSelector.IsEnabled = false;
                DayScheduleSelector.Items.Add("No Schedules Available");
                ScheduleNotAvailableBlock.Visibility = Visibility.Visible;
                DayScheduleSelector.SelectedIndex = 0;
            }
            else
            {
                DayScheduleSelector.IsEnabled = true;
                DayScheduleSelector.SelectionChanged += DayScheduleSelector_SelectionChanged;
                DayScheduleSelector.SelectedIndex = 0;
            }
            ScheduleLoaded = true;
            ScheduleProgressIndicator.IsActive = false;
            //var saved = formatter.ToString();
        }

        private void DayScheduleSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainScheduleBrowser.Schedule = Schedule[(ServiceDay)((ComboBoxItem)DayScheduleSelector.SelectedItem).Tag];
        }

        private WeekSchedule Schedule;

        private async Task RefreshRoutes()
        {
            RoutesProgressIndicator.IsActive = true;
            RoutesControl.Items.Clear();
            foreach (string rte in Stop.Routes)
            {
                BusRoute? route = await Data.GetRoute(rte, MasterCancellationTokenSource.Token);
                if (route != null)
                    RoutesControl.Items.Add(new RouteListingTemplateSelector.RouteListing() { Name = route.Value.Name, Description = route.Value.Description, Agency = (await Data.GetTransitAgency(route.Value.Agency, MasterCancellationTokenSource.Token)).Value.Name, RouteId = route.Value.ID });
            }
            RoutesProgressIndicator.IsActive = false;
        }

        private void InnerGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetMapCenter();
        }

        private async void LoadSchedulesButton_Click(object sender, RoutedEventArgs e)
        {
            await GetSchedule();
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
