using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace OneAppAway
{
    public sealed class DownloadManager
    {
        #region Static
        private static List<DownloadManager> DownloadsInProgress = new List<DownloadManager>();

        public static async Task<DownloadManager> Create(RouteListing listing, CancellationToken cancellationToken)
        {
            listing.Progress = 0;
            listing.ShowProgress = true;
            DownloadManager result = new DownloadManager();
            result.Listing = listing;
            result._Route = result.Listing.Route;
            var stopsAndShapes = await ApiLayer.GetStopsForRoute(result.Listing.Route.ID, cancellationToken);
            result._Shapes = stopsAndShapes.Item2;
            result._StopsPending = new ObservableCollection<BusStop>(stopsAndShapes.Item1);
            if (cancellationToken.IsCancellationRequested)
                return null;
            result.Initialize();
            result._TotalStops = result.StopsPending.Count;
            DownloadsInProgress.Add(result);
            return result;
        }

        public static async Task<RouteListing[]> DownloadAll(Action<double, string> statusChangedCallback, CancellationToken cancellationToken, params RouteListing[] routeListings)
        {
            List<RouteListing> errorList = new List<RouteListing>();
            List<BusStop> allStops = new List<BusStop>();
            try
            {
                for (int i = 0; i < routeListings.Length; i++)
                {
                    statusChangedCallback(0.15 * i / routeListings.Length, "Getting stops (" + (i + 1).ToString() + " of " + routeListings.Length.ToString() + ")" + " " + (routeListings[i].Name.All(chr => char.IsDigit(chr)) ? "Route " : "") + routeListings[i].Name);
                    try
                    {
                        var manager = await DownloadManager.Create(routeListings[i], cancellationToken);
                        await FileManager.SaveRoute(manager.Route, manager.StopsPending.Select(item => item.ID).ToArray(), manager.Shapes);
                        foreach (var stop in manager.StopsPending)
                        {
                            if (!allStops.Contains(stop))
                                allStops.Add(stop);
                        }
                    }
                    catch (Exception ex)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        errorList.Add(routeListings[i]);
                        ex.ToString();
                    }
                }

                WeekSchedule schedule;
                string[] routeFilters = DownloadsInProgress.Select(download => download.Listing.Route.ID).ToArray();

                for (int i = 0; i < allStops.Count; i++)
                {
                    statusChangedCallback(0.15 + 0.85 * i / allStops.Count, "Downloading schedules (" + (i + 1).ToString() + " of " + allStops.Count.ToString() + ") " + allStops[i].Name);
                    try
                    {
                        schedule = await Data.GetScheduleForStop(allStops[i].ID, cancellationToken);
                        schedule.FilterByRoute(routeFilters);
                        await FileManager.SaveScheduleAsync(schedule, allStops[i]);
                        foreach (var manager in DownloadsInProgress.ToArray())
                        {
                            if (manager.StopsPending.Contains(allStops[i]))
                                manager.StopsPending.Remove(allStops[i]);
                        }
                    }
                    catch (Exception ex)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        foreach (var download in DownloadsInProgress.Where(item => item.StopsPending.Contains(allStops[i])))
                        {
                            if (!errorList.Contains(download.Listing))
                                errorList.Add(download.Listing);
                        }
                        ex.ToString();
                    }
                }
                statusChangedCallback(1, "Download complete.");
            }
            catch (OperationCanceledException)
            {
                statusChangedCallback(1, "Download cancelled.");
                while (DownloadsInProgress.Count > 0)
                    DownloadsInProgress[0].StopsPending.Clear();
            }
            return errorList.ToArray();
        }
        #endregion

        #region Instance
        private DownloadManager() { }

        private void Initialize()
        {
            _StopsPending.CollectionChanged += _StopsPending_CollectionChanged;
        }

        private void _StopsPending_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            double progress = 1.0 - (double)StopsPending.Count / (double)TotalStops;
            Listing.Progress = progress;
            Listing.ShowProgress = (progress != 1.0);
            if (StopsPending.Count == 0)
                DownloadsInProgress.Remove(this);
        }

        private RouteListing Listing;

        private ObservableCollection<BusStop> _StopsPending = new ObservableCollection<BusStop>();
        private string[] _Shapes;
        private BusRoute _Route;
        private int _TotalStops;

        public IList<BusStop> StopsPending
        {
            get { return _StopsPending; }
        }

        public BusRoute Route
        {
            get { return _Route; }
        }

        public string[] Shapes
        {
            get { return _Shapes.ToArray(); }
        }

        public int TotalStops
        {
            get { return _TotalStops; }
        }
        #endregion
    }

    public class RouteListing : DependencyObject
    {
        public RouteListing(BusRoute route)
        {
            Name = route.Name;
            Description = route.Description;
            Route = route;
        }

        public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register("Progress", typeof(double), typeof(RouteListing), new PropertyMetadata(0.0));
        public static readonly DependencyProperty ShowProgressProperty = DependencyProperty.Register("ShowProgress", typeof(bool), typeof(RouteListing), new PropertyMetadata(false));
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(RouteListing), new PropertyMetadata(false));

        public BusRoute Route { get; private set; }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        public bool ShowProgress
        {
            get { return (bool)GetValue(ShowProgressProperty); }
            set { SetValue(ShowProgressProperty, value); }
        }

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }
    }
}
