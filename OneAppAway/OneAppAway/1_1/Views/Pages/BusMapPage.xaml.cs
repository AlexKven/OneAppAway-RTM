using OneAppAway._1_1.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SQLitePCL;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace OneAppAway._1_1.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BusMapPage : Page
    {
        public BusMapPage()
        {
            this.InitializeComponent();
            //MainMap.CenterRegion = new RectSubset() { Left = 300, LeftValueType = RectSubsetValueType.Length, Top = 100, Bottom = 150, Right = 200 };
            MainMap.CenterRegion = new RectSubset() { Left = 200, LeftValueType = RectSubsetValueType.Length };
            Run();
        }
        
        async void Run()
        {
            DatabaseManager.Initialize(null);
            TransitStopSqlProvider provider = new TransitStopSqlProvider();
            DatabaseManager.ExecuteSQL(DatabaseManager.MemoryDatabase, provider.CreateTableQuery());
            DatabaseManager.ExecuteSQL(DatabaseManager.MemoryDatabase, provider.InsertQuery(new TransitStop() { ID = "000", Position = LatLon.Seattle, Name = "Stop 1", Direction = Data.StopDirection.NE }));
            DatabaseManager.ExecuteSQL(DatabaseManager.MemoryDatabase, provider.InsertQuery(new TransitStop() { ID = "001", Position = LatLon.Seattle, Name = "Stop 2", Direction = Data.StopDirection.SE }));
            DatabaseManager.ExecuteSQL(DatabaseManager.MemoryDatabase, provider.InsertQuery(new TransitStop() { ID = "002", Position = LatLon.Seattle, Name = "Stop 3", Direction = Data.StopDirection.Unspecified }));
            var count = DatabaseManager.ExecuteSQL(DatabaseManager.MemoryDatabase, "select count(*) from TransitStop;").ToNumber<int>();
            string[] cols;
            var results = provider.GetObjects(DatabaseManager.ExecuteSQL(DatabaseManager.MemoryDatabase, @"select Name, ID from TransitStop;", out cols), cols);

            await Task.Delay(6000);
            MainMap.StopsSource = new TransitStop() { Position = LatLon.Seattle, Direction = Data.StopDirection.NW, ID = "a" };
            await Task.Delay(2000);

            MainMap.UnderlyingMapControl.MapElements.Add(new Windows.UI.Xaml.Controls.Maps.MapPolygon()
            {
                FillColor = Colors.LightGray,
                StrokeColor = Colors.Black,
                StrokeThickness = 2,
                ZIndex = 4,
                Path = new Windows.Devices.Geolocation.Geopath(new Windows.Devices.Geolocation.BasicGeoposition[]
            {
                (LatLon.Seattle + new LatLon(-0.0004, -0.0004)).ToBasicGeoposition(),
                (LatLon.Seattle + new LatLon(0.0004, -0.0004)).ToBasicGeoposition(),
                (LatLon.Seattle + new LatLon(0.0004, 0.0004)).ToBasicGeoposition(),
                (LatLon.Seattle + new LatLon(-0.0004, 0.0004)).ToBasicGeoposition()
            })
            });

            await Task.Delay(2000);

            ObservableCollection<TransitStop> stops = new ObservableCollection<TransitStop>();
            MainMap.StopsSource = stops;

            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(1000);
                stops.Add(new TransitStop() { Position = LatLon.Seattle + new LatLon(0, 0.001 * i), Direction = Data.StopDirection.SW, ID = i.ToString() });
            }
            await Task.Delay(1000);
            stops.Add(new TransitStop() { Position = LatLon.Seattle, Direction = Data.StopDirection.SW, ID = "b", Children = new string[] { "0", "1", "2" } });
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(1000);
                stops.RemoveAt(0);
            }

            //MainMap.ZoomLevel = 18;
            //for (int i = 3; i <= 40; i++)
            //{
            //    MainMap.Center += new LatLon(0, 0.001);
            //    await Task.Delay(200);
            //    MainMap.ZoomLevel -= 0.1;
            //    await Task.Delay(200);
            //}
        }
    }
}
