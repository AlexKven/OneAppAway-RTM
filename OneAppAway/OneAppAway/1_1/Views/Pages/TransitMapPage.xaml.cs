using OneAppAway._1_1.Data;
using OneAppAway._1_1.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace OneAppAway._1_1.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TransitMapPage : Page
    {
        private ObservableCollection<TransitStop> Stops = new ObservableCollection<TransitStop>();
        private TransitStopSqlProvider provider;

        public TransitMapPage()
        {
            this.InitializeComponent();
            provider = new TransitStopSqlProvider();
            DatabaseManager.Initialize(null);
            DatabaseManager.ExecuteSQL(DatabaseManager.MemoryDatabase, provider.CreateTableQuery());
            MainMap.StopsSource = Stops;
        }

        private void LoadFromDatabase()
        {
            var stops = DatabaseRetriever.RetrieveStops();
            foreach (var stop in stops)
                if (!Stops.Any(stp => stp.ID == stop.ID))
                    Stops.Add(stop);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var fwtc = new string[] { "1_80439", "1_80431", "1_80438", "1_80432", "1_80437", "1_80433", "3_27814", "3_29410" };
            var stops = await _1_1.Data.ApiLayer.GetTransitStopsForArea(MainMap.Area, new System.Threading.CancellationToken());
            var fwtcStop = new TransitStop() { ID = "FWTC", Position = new LatLon(47.31753, -122.30486), Path = "_vx_HlnniVF??LJ??MF??nIG??MK??LG?", Name = "Federal Way Transit Center" };
            DatabaseManager.ExecuteSQL(DatabaseManager.MemoryDatabase, provider.InsertQuery(fwtcStop));
            foreach (var stop in stops)
            {
                var finalStop = stop;
                if (fwtc.Contains(finalStop.ID))
                    finalStop.Parent = "FWTC";
                DatabaseManager.ExecuteSQL(DatabaseManager.MemoryDatabase, provider.InsertQuery(finalStop));
            }
            LoadFromDatabase();
            //MainGrid.Children.Add(new Controls.StopArrivalsControl() { DataContext = new StopArrivalsViewModel(new TransitStop() { Children = new string[] { "FWTC" }, Name = "Selected Stops" }), Margin = new Thickness(50) });
        }
    }
}
