using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace OneAppAway
{
    public static class FavoritesManager
    {
        static FavoritesManager()
        {
            var favs = SettingsManager.GetSetting("FavoriteArrivals", true, new FavoriteArrival[0]);
            foreach (var fav in favs)
                _FavoriteArrivals.Add(fav);
            _FavoriteArrivals.CollectionChanged += _FavoriteArrivals_CollectionChanged;
        }

        private static void _FavoriteArrivals_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SettingsManager.SetSetting("FavoriteArrivals", true, _FavoriteArrivals.ToArray());
            if (FavoritesChanged != null)
                FavoritesChanged(null, new EventArgs());
        }

        private static ObservableCollection<FavoriteArrival> _FavoriteArrivals = new ObservableCollection<FavoriteArrival>();

        public static IList<FavoriteArrival> FavoriteArrivals
        {
            get { return _FavoriteArrivals; }
        }

        public static event EventHandler FavoritesChanged;

        public static async Task ShowAddOrRemoveFavorite(UIElement element, string route, string stop, string destination)
        {
            if (FavoriteArrivals.Any(fav => fav.Route == route && fav.Stop == stop && fav.Destination == destination))
            {

            }
            else
            {
                await ((App)App.Current).MainHamburgerBar.ShowPopup(element, 300, 350, typeof(AddToFavoritesPage), new string[] { route, stop, destination });
            }
        }
    }
}
