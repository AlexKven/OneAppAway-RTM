using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace OneAppAway
{
    public static class FavoritesManager
    {
        private static DataContractSerializer FavoriteArrivalsSerializer;
        static FavoritesManager()
        {
            FavoriteArrivalsSerializer = new DataContractSerializer(typeof(FavoriteArrival[]), new Type[] { typeof(FavoriteArrival), typeof(ContextLocation), typeof(LocationContext), typeof(CityContext), typeof(DistanceContext), typeof(CardinalDirectionContext), typeof(CardinalDirection), typeof(double), typeof(string) });
            string favsDecoded = SettingsManager.GetSetting<string>("FavoriteArrivals", true);
            if (!string.IsNullOrWhiteSpace(favsDecoded))
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(favsDecoded);
                    writer.Flush();
                    stream.Position = 0;
                    var favs = (FavoriteArrival[])FavoriteArrivalsSerializer.ReadObject(stream);
                    foreach (var fav in favs)
                        _FavoriteArrivals.Add(fav);
                }
            }
            _FavoriteArrivals.CollectionChanged += _FavoriteArrivals_CollectionChanged;
        }

        private static void _FavoriteArrivals_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            string favsDecoded;
            using (MemoryStream stream = new MemoryStream())
            {
                FavoriteArrivalsSerializer.WriteObject(stream, _FavoriteArrivals.ToArray());
                stream.Position = 0;
                StreamReader reader = new StreamReader(stream);
                favsDecoded = reader.ReadToEnd();
            }
            SettingsManager.SetSetting("FavoriteArrivals", true, favsDecoded);
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
                await ((App)App.Current).MainHamburgerBar.ShowPopup(element, 300, 100, typeof(RemoveFromFavoritesPage), new string[] { route, stop, destination });
            }
            else
            {
                await ((App)App.Current).MainHamburgerBar.ShowPopup(element, 300, 350, typeof(AddToFavoritesPage), new string[] { route, stop, destination });
            }
        }
    }
}
