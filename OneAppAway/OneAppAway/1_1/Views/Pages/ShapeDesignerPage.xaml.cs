using MvvmHelpers;
using OneAppAway._1_1.Data;
using OneAppAway._1_1.Helpers;
using OneAppAway._1_1.Views.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
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
    public sealed partial class ShapeDesignerPage : ApplicationPage
    {
        public ShapeDesignerPage()
        {
            this.InitializeComponent();
            DataContext = new BaseViewModel() { Title = "Shape Designer" };
            MainMap.MapServiceToken = Keys.BingMapKey;
            Points.CollectionChanged += (e, s) => ReAddPoints();
        }

        private void ReAddPoints()
        {
            MainPolygon.Points.Clear();
            foreach (var pt in Points.Select(p => p.Point))
            {
                MainPolygon.Points.Add(pt);
            }
            RefreshResults();
        }

        private void RefreshResults()
        {
            List<Geopoint> locations = new List<Geopoint>();
            foreach (var pt in Points)
            {
                Geopoint gpt;
                MainMap.GetLocationFromOffset(pt.Point, out gpt);
                locations.Add(gpt);
            }
            PathBox.Text = GooglePolylineConverter.Encode(locations.Select(l => l.ToLatLon()));
            double x = 0, y = 0;
            foreach (var l in locations)
            {
                y += l.Position.Latitude;
                x += l.Position.Longitude;
            }
            x /= locations.Count;
            y /= locations.Count;
            CenterBox.Text = $"{y}, {x}";
        }

    public ObservableRangeCollection<PointWrapper> Points { get; } = new ObservableRangeCollection<PointWrapper>();

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var w = new PointWrapper();
            w.PropertyChanged += (s, s1) => ReAddPoints();
            Points.Add(w);
        }

        private void MainMap_ActualCameraChanged(Windows.UI.Xaml.Controls.Maps.MapControl sender, Windows.UI.Xaml.Controls.Maps.MapActualCameraChangedEventArgs args)
        {
            ZoomLevelBlock.Text = MainMap.ZoomLevel.ToString();
            RefreshResults();
        }
    }
}
