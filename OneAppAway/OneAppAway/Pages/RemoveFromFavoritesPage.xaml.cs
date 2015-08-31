using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class RemoveFromFavoritesPage : Page
    {
        private string Route;
        private string Stop;
        private string Destination;

        public RemoveFromFavoritesPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is string[])
            {
                Route = ((string[])e.Parameter)[0];
                Stop = ((string[])e.Parameter)[1];
                Destination = ((string[])e.Parameter)[2];
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            FavoritesManager.FavoriteArrivals.Remove(new FavoriteArrival() { Route = this.Route, Stop = this.Stop, Destination = this.Destination });
            ((App)App.Current).MainHamburgerBar.DismissPopup();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ((App)App.Current).MainHamburgerBar.DismissPopup();
        }
    }
}
