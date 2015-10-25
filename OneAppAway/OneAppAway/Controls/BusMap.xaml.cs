using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using static System.Math;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OneAppAway
{
    public sealed partial class BusMap : UserControl, INotifyPropertyChanged
    {
        #region Fields
        private const double LAT_OVER_LON = .67706;

        private List<MapIcon> BusStopIcons = new List<MapIcon>();
        private Dictionary<MapIcon, BusStop> Stops = new Dictionary<MapIcon, BusStop>();
        private Dictionary<MapIcon, string> Buses = new Dictionary<MapIcon, string>();
        private double PreviousZoomLevel;

        private Polygon MyLocationIcon = new Polygon();
        #endregion 

        public BusMap()
        {
            this.InitializeComponent();
            _ShownStops.CollectionChanged += _ShownStops_CollectionChanged;
            MainMap.MapServiceToken = Keys.BingMapKey;
            MyLocationIcon.Stroke = new SolidColorBrush(((Color)App.Current.Resources["SystemColorControlAccentColor"]));
            MyLocationIcon.StrokeThickness = 2;
            MyLocationIcon.Visibility = Visibility.Collapsed;
            MyLocationIcon.Fill = new SolidColorBrush(Colors.White);
            PointCollection circlePoints = new PointCollection() { new Point(10, 0), new Point(3, 3), new Point(0, 10), new Point(-3, 3), new Point(-10, 0), new Point(-3, -3), new Point(0, -10), new Point(3, -3) };
            //for (double theta = 0; theta < 2 * PI; theta += PI / 16.0)
            //{
            //    double r = 8.0;
            //    if (theta >= PI / 4.0 && theta <= PI / 2.0)
            //        r = r / Cos(theta - PI / 4.0);
            //    else if (theta > PI / 2.0 && theta <= 3.0 * PI / 2.0)
            //        r = r / Cos(theta - 3.0 * PI / 4.0);
            //    circlePoints.Add(new Point(r * Cos(theta), r * Sin(theta)));
            //}
            MyLocationIcon.Points = circlePoints;
            MainMap.Children.Add(MyLocationIcon);
        }

        private ObservableCollection<BusStop> _ShownStops = new ObservableCollection<BusStop>();
        private double _StopSizeThreshold = 16;
        private double _StopVisibilityThreshold = 14;

        public ICollection<BusStop> ShownStops
        {
            get
            {
                return _ShownStops;
            }
        }

        public BasicGeoposition Center
        {
            get
            {
                return MainMap.Center.Position;
            }
            set
            {
                MainMap.Center = new Geopoint(value);
                OnPropertyChanged("Center", "TopLeft", "BottomRight");
            }
        }

        public BasicGeoposition TopLeft
        {
            get
            {
                try
                {
                    Geopoint result;
                    MainMap.GetLocationFromOffset(new Point(0, 0), out result);
                    return result.Position;
                }
                catch (ArgumentException)
                {
                    return new BasicGeoposition();
                }
            }
        }

        public BasicGeoposition BottomRight
        {
            get
            {
                try
                {
                    Geopoint result;
                    MainMap.GetLocationFromOffset(new Point(ActualWidth - 1, ActualHeight - 1), out result);
                    return result.Position;
                }
                catch (ArgumentException)
                {
                    return new BasicGeoposition();
                }
            }
        }

        public double LatitudePerPixel
        {
            get
            {
                return (TopLeft.Latitude - BottomRight.Latitude) / ActualHeight;
            }
        }

        public double LongitudePerPixel
        {
            get
            {
                return (BottomRight.Longitude - TopLeft.Longitude) / ActualWidth;
            }
        }

        public double ZoomLevel
        {
            get
            {
                return MainMap.ZoomLevel;
            }
            set
            {
                MainMap.ZoomLevel = value;
            }
        }

        public double StopSizeThreshold
        {
            get { return _StopSizeThreshold; }
            set
            {
                _StopSizeThreshold = value;
                RefreshIconSizes();
                OnPropertyChanged("StopSizeThreshold");
            }
        }
        public double StopVisibilityThreshold
        {
            get { return _StopVisibilityThreshold; }
            set
            {
                _StopVisibilityThreshold = value;
                RefreshIconSizes();
                OnPropertyChanged("StopVisibilityThreshold");
            }
        }

        public MapControl MapControl => MainMap;

        #region Methods
        private void AddStopToMap(BusStop stop)
        {
            MapIcon mico = new MapIcon();
            mico.ZIndex = 10;
            mico.Location = new Geopoint(stop.Position);
            mico.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
            string size = ZoomLevel < StopSizeThreshold ? "20" : "40";
            bool visibility = ZoomLevel >= StopVisibilityThreshold;
            
            mico.Image = RandomAccessStreamReference.CreateFromUri(new Uri(stop.Direction == StopDirection.Unspecified ? "ms-appx:///Assets/Icons/BusBase" + size + ".png" : "ms-appx:///Assets/Icons/BusDirection" + stop.Direction.ToString() + size + ".png"));
            mico.NormalizedAnchorPoint = new Point(0.5, 0.5);
            mico.Visible = visibility;
            MainMap.MapElements.Add(mico);
            Stops[mico] = stop;
            BusStopIcons.Add(mico);
        }

        private void RemoveStopFromMap(BusStop stop)
        {
            MapIcon mico = Stops.FirstOrDefault(kvp => kvp.Value == stop).Key;
            Stops.Remove(mico);
            if (mico == null) return;
            MainMap.MapElements.Remove(mico);
        }

        private void RefreshIconSizes()
        {
            foreach (MapIcon img in BusStopIcons)
            {
                BusStop stop = Stops[img];
                string size = ZoomLevel < StopSizeThreshold ? "20" : "40";
                img.Image = RandomAccessStreamReference.CreateFromUri(new Uri(stop.Direction == StopDirection.Unspecified ? "ms-appx:///Assets/Icons/BusBase" + size + ".png" : "ms-appx:///Assets/Icons/BusDirection" + stop.Direction.ToString() + size + ".png"));
                //img.Source = new BitmapMapIcon(new Uri(stop.Direction == StopDirection.Unspecified ? "ms-appx:///Assets/Icons/BusBase" + size + ".png" : "ms-appx:///Assets/Icons/BusDirection" + stop.Direction.ToString() + size + ".png"));
            }
        }

        private void RefreshIconVisibilities()
        {
            bool visibility = ZoomLevel >= StopVisibilityThreshold;
            foreach (MapIcon img in BusStopIcons)
            {
                BusStop stop = Stops[img];
                img.Visible = visibility;
                //img.Source = new BitmapMapIcon(new Uri(stop.Direction == StopDirection.Unspecified ? "ms-appx:///Assets/Icons/BusBase" + size + ".png" : "ms-appx:///Assets/Icons/BusDirection" + stop.Direction.ToString() + size + ".png"));
            }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(params string[] propertyNames)
        {
            if (PropertyChanged != null)
            {
                foreach (string propertyName in propertyNames)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event EventHandler<StopClickedEventArgs> StopsClicked;

        private void OnStopsClicked(BusStop[] stops, BasicGeoposition location)
        {
            if (StopsClicked != null) StopsClicked(this, new StopClickedEventArgs(location, stops));
        }
        #endregion

        #region Event Handlers
        private void _ShownStops_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                Stops.Clear();
                foreach (var item in BusStopIcons)
                    MainMap.Children.Remove(item);
                BusStopIcons.Clear();
            }
            if (e.NewItems != null)
            {
                foreach (BusStop item in e.NewItems)
                {
                    AddStopToMap(item);
                }
            }
            else if (e.OldItems != null)
            {
                foreach (BusStop item in e.OldItems)
                {
                    RemoveStopFromMap(item);
                }
            }
        }

        private void MainMap_ZoomLevelChanged(MapControl sender, object args)
        {
            if ((ZoomLevel < StopSizeThreshold) != (PreviousZoomLevel < StopSizeThreshold))
                RefreshIconSizes();
            if ((ZoomLevel < StopVisibilityThreshold) != (PreviousZoomLevel < StopVisibilityThreshold))
                RefreshIconVisibilities();
            OnPropertyChanged("ZoomLevel", "TopLeft", "BottomRight");
            PreviousZoomLevel = ZoomLevel;
        }

        private void MainMap_CenterChanged(MapControl sender, object args)
        {
            OnPropertyChanged("Center", "TopLeft", "BottomRight");
        }

        private void MainMap_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            OnPropertyChanged("Center", "TopLeft", "BottomRight");
        }
        #endregion

        public async void CenterOnLocation(BasicGeoposition location, double zoomLevel)
        {
            await MainMap.TrySetViewAsync(new Geopoint(location), zoomLevel, null, null, MapAnimationKind.None);
        }

        private void MainMap_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            var mapIcons = args.MapElements.Where(me => me is MapIcon && BusStopIcons.Contains(me));
            BusStop[] stops = new BusStop[mapIcons.Count()];
            for (int i = 0; i < stops.Length; i++)
            {
                stops[i] = Stops[(MapIcon)mapIcons.ElementAt(i)];
            }
            OnStopsClicked(stops, args.Location.Position);
        }

        public void HookLocationEvents()
        {
            LocationManager.LocationChanged += LocationManager_LocationChanged;
            RefreshMyLocation();
        }

        public void UnhookLocationEvents()
        {
            LocationManager.LocationChanged -= LocationManager_LocationChanged;
        }

        private void LocationManager_LocationChanged(object sender, EventArgs e)
        {
            RefreshMyLocation();
        }

        private async void RefreshMyLocation()
        {
            var loc = await LocationManager.GetPosition();
            if (loc == null)
                MyLocationIcon.Visibility = Visibility.Collapsed;
            else
            {
                MapControl.SetLocation(MyLocationIcon, new Geopoint(loc.Value));
                MyLocationIcon.Visibility = Visibility.Visible;
            }
            WriteableBitmap bmp = new WriteableBitmap(96, 96);
            

        }
    }

    public class StopClickedEventArgs : EventArgs
    {
        public StopClickedEventArgs(BasicGeoposition center, params BusStop[] stops)
        {
            Center = center;
            Stops = stops;
        }

        public BasicGeoposition Center { get; private set; }

        public BusStop[] Stops { get; private set; }
    }
}