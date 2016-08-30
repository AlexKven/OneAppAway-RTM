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

        private ContextLocation location;
        private FavoriteArrival favorite;

        private CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is string[])
            {
                var stop = await Data.GetBusStop(((string[])e.Parameter)[1], CancellationTokenSource.Token);
                var route = await Data.GetRoute(((string[])e.Parameter)[0], CancellationTokenSource.Token);
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
                location = new ContextLocation() { Latitude = stop.Value.Position.Latitude, Longitude = stop.Value.Position.Longitude, City = city };
                favorite = new FavoriteArrival() { Contexts = new LocationContext[0], Route = route.Value.ID, Stop = stop.Value.ID, Destination = destination };
            }
        }

        private void MileSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            MileContextDescription.Text = "Within " + MileSlider.Value.ToString("F2") + " miles of this stop";
            if (favorite == null) return;
            var list = favorite.Contexts.ToList();
            list.Where(ctxt => ctxt is DistanceContext).Select(ctxt => ((DistanceContext)ctxt).Distance = MileSlider.Value);
            favorite.Contexts = list.ToArray();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            favorite.CustomName = TitleBox.Text;
            FavoritesManager.FavoriteArrivals.Add(favorite);
            //((App)App.Current).MainHamburgerBar.DismissPopup();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            //((App)App.Current).MainHamburgerBar.DismissPopup();
        }

        private void CityContextBox_Checked(object sender, RoutedEventArgs e)
        {
            if (favorite == null) return;
            if (favorite.Contexts.All(ctxt => !(ctxt is CityContext)))
            {
                var list = favorite.Contexts.ToList();
                list.Add(new CityContext() { RelativeLocation = location });
                favorite.Contexts = list.ToArray();
            }
        }

        private void CityContextBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (favorite == null) return;
            var list = favorite.Contexts.ToList();
            list.RemoveAll(ctxt => ctxt is CityContext);
            favorite.Contexts = list.ToArray();
        }

        private void MileContextBox_Checked(object sender, RoutedEventArgs e)
        {
            if (favorite == null) return;
            if (favorite.Contexts.All(ctxt => !(ctxt is DistanceContext)))
            {
                var list = favorite.Contexts.ToList();
                list.Add(new DistanceContext() { RelativeLocation = location, Distance = MileSlider.Value });
                favorite.Contexts = list.ToArray();
            }
        }

        private void MileContextBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (favorite == null) return;
            var list = favorite.Contexts.ToList();
            list.RemoveAll(ctxt => ctxt is DistanceContext);
            favorite.Contexts = list.ToArray();
        }

        private void DirectionContextBox_Checked(object sender, RoutedEventArgs e)
        {
            if (favorite == null) return;
            if (favorite.Contexts.All(ctxt => !(ctxt is CardinalDirectionContext)))
            {
                var list = favorite.Contexts.ToList();
                list.Add(new CardinalDirectionContext() { RelativeLocation = location, Direction = (CardinalDirection)Enum.Parse(typeof(CardinalDirection), ((ComboBoxItem)CardinalDirectionSelector.SelectedValue).Content.ToString()) });
                favorite.Contexts = list.ToArray();
            }
        }

        private void DirectionContextBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (favorite == null) return;
            var list = favorite.Contexts.ToList();
            list.RemoveAll(ctxt => ctxt is CardinalDirectionContext);
            favorite.Contexts = list.ToArray();
        }

        private void CardinalDirectionSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (favorite == null) return;
            var list = favorite.Contexts.ToList();
            list.Where(ctxt => ctxt is CardinalDirectionContext).Select(ctxt => ((CardinalDirectionContext)ctxt).Direction = (CardinalDirection)Enum.Parse(typeof(CardinalDirection), ((ComboBoxItem)CardinalDirectionSelector.SelectedValue).Content.ToString()));
            favorite.Contexts = list.ToArray();
        }
    }
}
