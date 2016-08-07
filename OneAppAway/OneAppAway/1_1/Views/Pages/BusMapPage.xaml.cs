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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
            await Task.Delay(6000);
            MainMap.StopsSource = new TransitStop() { Position = LatLon.Seattle, Direction = Data.StopDirection.NW, ID = "a" };
            await Task.Delay(2000);

            ObservableCollection<TransitStop> stops = new ObservableCollection<TransitStop>();
            MainMap.StopsSource = stops;

            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(1000);
                stops.Add(new TransitStop() { Position = LatLon.Seattle + new LatLon(0, 0.001 * i), Direction = Data.StopDirection.SW, ID = i.ToString() });
            }
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
