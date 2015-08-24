using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
    public sealed partial class AddToFavoritesPage : Page
    {
        public AddToFavoritesPage()
        {
            this.InitializeComponent();
        }

        private CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is string[])
            {
                var stop = await Data.GetBusStop(((string[])e.Parameter)[0], CancellationTokenSource.Token);
                var route = await Data.GetRoute(((string[])e.Parameter)[1], CancellationTokenSource.Token);
                string destination = ((string[])e.Parameter)[2];
                DescriptionBlock.Text = route.Value.Name + " to " + destination + " at " + stop.Value.Name;
                Windows.Devices.Geolocation.Geolocator locator = new Windows.Devices.Geolocation.Geolocator();
                TitleBox.Text = destination;
                string city = null;
                try
                {
                    var finder = await MapLocationFinder.FindLocationsAtAsync(new Windows.Devices.Geolocation.Geopoint(stop.Value.Position));
                    if (finder.Locations.Count > 0)
                    {
                        city = finder.Locations[0].Address.Town;
                        CityContextBox.IsEnabled = true;
                        CityContextBox.Content = "In " + city;
                    }
                    else
                    {
                        CityContextBox.Content = "(Could not get city)";
                    }
                }
                catch (Exception)
                {
                    CityContextBox.Content = "(Could not get city)";
                }
            }
        }

        private void MileSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            MileContextDescription.Text = "Within " + MileSlider.Value.ToString("F2") + " miles of this stop";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ((App)App.Current).MainHamburgerBar.DismissPopup();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ((App)App.Current).MainHamburgerBar.DismissPopup();
        }
    }
}
