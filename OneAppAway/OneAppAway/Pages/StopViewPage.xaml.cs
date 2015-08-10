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
                RoutesToggle.IsChecked = SettingsManager.GetSetting<bool>("StopViewPage.RoutesToggleChecked", false, true);
                ArrivalsToggle.IsChecked = SettingsManager.GetSetting<bool>("StopViewPage.ArrivalsToggleChecked", false, true);
                ScheduleToggle.IsChecked = SettingsManager.GetSetting<bool>("StopViewPage.ScheduleToggleChecked", false, false);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            SettingsManager.SetSetting<bool>("StopViewPage.RoutesToggleChecked", false, RoutesToggle.IsChecked.Value);
            SettingsManager.SetSetting<bool>("StopViewPage.ArrivalsToggleChecked", false, ArrivalsToggle.IsChecked.Value);
            SettingsManager.SetSetting<bool>("StopViewPage.ScheduleToggleChecked", false, ScheduleToggle.IsChecked.Value);
        }

        private async void SetPage(string stopId)
        {
            Stop = await Data.GetBusStop(stopId, MasterCancellationTokenSource.Token);
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
            RefreshArrivals();
            if (ScheduleToggle.IsChecked.Value && !ScheduleLoaded && BandwidthManager.EffectiveBandwidthOptions == BandwidthOptions.Normal)
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
            ArrivalsColumn.Width = ArrivalsToggle.IsChecked.Value ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
            ScheduleColumn.Width = ScheduleToggle.IsChecked.Value ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
            RoutesColumn.Width = RoutesToggle.IsChecked.Value ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
            SetInnerGridSize();
        }

        private async Task RefreshArrivals()
        {
            ArrivalsProgressIndicator.IsActive = true;
            var arrivals = await ApiLayer.GetBusArrivals(Stop.ID, MasterCancellationTokenSource.Token);
            var removals = ArrivalsStackPanel.Children.Where(child => !arrivals.Contains(((BusArrivalBox)child).Arrival));
            foreach (var item in removals)
                ArrivalsStackPanel.Children.Remove(item);
            foreach (var item in arrivals)
            {
                if (ArrivalsStackPanel.Children.Any(child => ((BusArrivalBox)child).Arrival == item))
                    ((BusArrivalBox)ArrivalsStackPanel.Children.First(child => ((BusArrivalBox)child).Arrival == item)).Arrival = item;
                else
                    ArrivalsStackPanel.Children.Add(new BusArrivalBox() { Arrival = item });
            }
            LastRefreshBox.Text = "Last update: " + DateTime.Now.ToString("h:mm:ss");
            ArrivalsProgressIndicator.IsActive = false;
        }

        private async Task GetSchedule()
        {
            ScheduleProgressIndicator.IsActive = true;
            ScheduleNotAvailableBlock.Visibility = Visibility.Collapsed;
            LoadSchedulesButton.Visibility = Visibility.Collapsed;
            Schedule = await Data.GetScheduleForStop(Stop.ID, MasterCancellationTokenSource.Token);
            DayScheduleSelector.Items.Clear();
            DayScheduleSelector.SelectedIndex = -1;
            foreach (var day in Schedule.DayGroups)
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
                BusRoute route = await Data.GetRoute(rte, MasterCancellationTokenSource.Token);
                RoutesControl.Items.Add(new RouteListingTemplateSelector.RouteListing() { Name = route.Name, Description = route.Description, Agency = (await Data.GetTransitAgency(route.Agency, MasterCancellationTokenSource.Token)).Name, RouteId = route.ID });
            }
            RoutesProgressIndicator.IsActive = false;
        }

        private void InnerGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetMapCenter();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
#pragma warning disable CS4014
            RefreshArrivals();
            RefreshRoutes();
#pragma warning restore CS4014
        }

        private void AppBarToggleButton_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (!(ArrivalsToggle.IsChecked.Value || ScheduleToggle.IsChecked.Value || RoutesToggle.IsChecked.Value))
                ArrivalsToggle.IsChecked = true;
            else
                SetColumns();
        }

        private async void LoadSchedulesButton_Click(object sender, RoutedEventArgs e)
        {
            await GetSchedule();
        }

        private void RouteButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RouteViewPage), ((Button)sender).Tag.ToString());
        }
    }
}
