using OneAppAway._1_1.Data;
using OneAppAway._1_1.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace OneAppAway._1_1.ViewModels
{
    class TransitMapPageUwpViewModel : TransitMapPageViewModel
    {
        public TransitMapPageUwpViewModel(MemoryCache cache)
            : base(cache)
        {
        }

        private TimeSpan LocationTimeout = TimeSpan.FromSeconds(5);

        protected override void OnNavigatedTo(object parameter)
        {
            if (parameter == null)
            {
                var location = LocationHelper.GetLastKnownLocation();
                if (location != null)
                    OnViewChangeRequested(new MapView(location.Value.ToLatLon(), 16.75) { Animate = false });
            }
            base.OnNavigatedTo(parameter);
        }

        protected override bool IsMultiSelectOn
        {
            get
            {
                return (Window.Current.CoreWindow.GetKeyState(Windows.System.VirtualKey.Control) & Windows.UI.Core.CoreVirtualKeyStates.Down) == Windows.UI.Core.CoreVirtualKeyStates.Down;
            }
        }

        protected override async Task GetLocation(Action<LatLon> locationCallback)
        {
            try
            {
                await LocationHelper.ProgressivelyAcquireLocation(pos => locationCallback(pos.ToLatLon()), LocationTimeout);
            }
            catch (TimeoutException)
            {
                MessageDialog dialog = new MessageDialog("Finding your location took too long and timed out. Try again and we'll try it with more time.");
                await dialog.ShowAsync();
                LocationTimeout += LocationTimeout;
            }
            catch (Exception ex)
            {
                MessageDialog dialog = new MessageDialog(ex.Message);
                await dialog.ShowAsync();
            }
        }

        protected override async void Search_Execute(object parameter)
        {
            SearchResults.Clear();
            var results = await Windows.Services.Maps.MapLocationFinder.FindLocationsAsync(parameter.ToString(), Area.Center.ToGeopoint());
            SearchResults.AddRange(results.Locations.Select(ml => new LocationSearchResult() { Location = ml.Point.ToLatLon(), Name = ml.Address.FormattedAddress, Command = GoToLocationCommand }));
            IsSearchBoxOpen = true;
        }
    }
}
