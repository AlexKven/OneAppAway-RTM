using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static System.Math;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace OneAppAway
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RoutesPage : NavigationFriendlyPage
    {
        private bool CanNavigateAway = true;
        private bool IsPhoneProgressBarShown = false;
        private Task<RouteListing[]> DownloadsTask;

        public RoutesPage()
        {
            this.InitializeComponent();
        }

        private bool _AgenciesWarning = false;
        private bool _RoutesWarning = false;

        public bool AgenciesWarning
        {
            get { return _AgenciesWarning; }
            set
            {
                _AgenciesWarning = value;
                SetWarningLabel();
            }
        }

        public bool RoutesWarning
        {
            get { return _RoutesWarning; }
            set
            {
                _RoutesWarning = value;
                SetWarningLabel();
            }
        }

        private void SetWarningLabel()
        {
            WarningBlock.Visibility = (AgenciesWarning || RoutesWarning) ? Visibility.Visible : Visibility.Collapsed;
        }

        private CancellationTokenSource MasterCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource DownloadsCancellationTokenSource = new CancellationTokenSource();

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await FileManager.LoadPendingDownloads();
            var agencies = await Data.GetTransitAgencies(new DataRetrievalOptions(DataSourceDescriptor.Cloud), MasterCancellationTokenSource.Token);
            AgenciesWarning = agencies.Item2.FinalSource != DataSourceDescriptor.Cloud;
            if (agencies.Item1 != null)
            {
                foreach (var agency in (agencies.Item1))
                {
                    AgenciesListView.Items.Add(agency);
                    AgenciesListView.SelectedIndex = 0;
                }
            }
        }

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (!CanNavigateAway)
            {
                e.Cancel = true;
                MessageDialog dialog = new MessageDialog("If you leave this page, the downloads will pause. You can resume them later. Are you sure you want to leave?", "Downloads in Progress");
                dialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler((cmd) => CanNavigateAway = true)));
                dialog.Commands.Add(new UICommand("No", new UICommandInvokedHandler((cmd) => CanNavigateAway = false)));
                await dialog.ShowAsync();

                if (CanNavigateAway)
                    Frame.Navigate(e.SourcePageType, e.Parameter);
            }
            else
            {
                MasterCancellationTokenSource.Cancel();
                DownloadsCancellationTokenSource.Cancel();
            }
        }

        private async void AgenciesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadingRect.Visibility = Visibility.Visible;
            MainProgressRing.IsActive = true;
            var inOrder = (Func<RouteListing, RouteListing, bool>)delegate (RouteListing first, RouteListing second)
            {
                var splitName = (Func<string, Tuple<int, string>>)delegate (string name)
                {
                    string num = new string(name.TakeWhile(chr => char.IsNumber(chr)).ToArray());
                    if (num.Length == 0)
                        return new Tuple<int, string>(0, name);
                    else
                        return new Tuple<int, string>(int.Parse(num), name.Replace(num, ""));
                };
                var splitFirst = splitName(first.Name);
                var splitSecond = splitName(second.Name);
                if (splitFirst.Item1 < splitSecond.Item1)
                    return true;
                else if (splitFirst.Item1 > splitSecond.Item1)
                    return false;
                else
                    return string.Compare(splitFirst.Item2, splitSecond.Item2, StringComparison.CurrentCultureIgnoreCase) < 0;
            };
            MainList.Items.Clear();
            var agency = (TransitAgency)AgenciesListView.SelectedItem;
            SortedSet<RouteListing> routesList = new SortedSet<RouteListing>(Comparer<RouteListing>.Create(new Comparison<RouteListing>((rt1, rt2) => inOrder(rt1, rt2) ? -1 : 1)));
            var routes = await Data.GetBusRoutesForAgency(agency.ID, new DataRetrievalOptions(DataSourceDescriptor.Cloud), MasterCancellationTokenSource.Token);
            RoutesWarning = routes.Item2.FinalSource != DataSourceDescriptor.Cloud;
            if (routes.Item1 != null)
            {
                foreach (var rte in routes.Item1)
                {
                    var listing = new RouteListing(rte);
                    await listing.RefreshIsDownloaded();
                    routesList.Add(listing);
                }
                foreach (var rte in routesList)
                    MainList.Items.Add(rte);
                await FileManager.SaveAgency(agency, routesList.Select(item => item.Route.ID).ToArray());
            }
            MainProgressRing.IsActive = false;
            LoadingRect.Visibility = Visibility.Collapsed;
        }

        private void Route_Clicked(object sender, EventArgs e)
        {
            Frame.Navigate(typeof(RouteViewPage), ((RouteListingControl)sender).Route.ID);
        }

        private async Task SetProgress(double progress, string message)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var bar = StatusBar.GetForCurrentView();
                if (progress != 1 && !IsPhoneProgressBarShown)
                {
                    IsPhoneProgressBarShown = true;
                    await bar.ProgressIndicator.ShowAsync();
                }
                else if (progress == 1 && IsPhoneProgressBarShown)
                {
                    IsPhoneProgressBarShown = false;
                    await bar.ProgressIndicator.HideAsync();
                }
                bar.ProgressIndicator.ProgressValue = progress;
                bar.ProgressIndicator.Text = message;
            }
            else
            {
                MainStatusBar.Visibility = (progress == 1) ? Visibility.Collapsed : Visibility.Visible;
                MasterProgressBar.Value = progress;
                StatusBlock.Text = message;
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MainList.Tag = ActualWidth / (Max((int)ActualWidth / 400, 1.0));
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            var cancellationToken = DownloadsCancellationTokenSource.Token;
            VisualStateManager.GoToState(this, "DownloadingState", true);
            var items = MainList.Items.Where(item => ((item as RouteListing)?.IsChecked).GetValueOrDefault(false)).Select(item => (RouteListing)item).ToArray();
            DownloadsTask = DownloadManager.DownloadAll(async (p, m) => await SetProgress(p, m), cancellationToken, items);
            var errors = await DownloadsTask;
            if (cancellationToken.IsCancellationRequested)
            {
                await SetProgress(1, "Cancelled");
            }
            VisualStateManager.GoToState(this, "NotDownloadingState", true);
            if (errors != null && errors.Length > 0)
            {
                VisualStateManager.GoToState(this, "SelectingState", true);
                foreach (RouteListing listing in MainList.Items)
                    listing.IsChecked = errors.Contains(listing);
                MessageDialog dialog = new MessageDialog("There was an error downloading " + errors.Length.ToString() + " of the routes. The affected routes are selected.", "Error downloading some routes");
                await dialog.ShowAsync();
            }
            else
            {
                bool checkMode = false;
                foreach (RouteListing listing in MainList.Items)
                    checkMode = checkMode || (listing.IsChecked = listing.IsDownloaded == DownloadStatus.Downloading);
                if (checkMode)
                    VisualStateManager.GoToState(this, "SelectingState", true);
                else
                    VisualStateManager.GoToState(this, "NotSelectingState", true);

            }
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckToggle.IsChecked.Value)
                CheckToggle.IsChecked = true;
            bool alreadyChecked = true;
            foreach (RouteListing listing in MainList.Items)
            {
                alreadyChecked &= listing.IsChecked;
                listing.IsChecked = true;
            }
            if (alreadyChecked)
            {
                foreach (RouteListing listing in MainList.Items)
                    listing.IsChecked = false;
            }
        }

        private void CheckToggle_Checked(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "SelectingState", true);
        }

        private void CheckToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "NotSelectingState", true);
        }

        private void DownloadingStates_CurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState?.Name == "DownloadingState")
                VisualStateManager.GoToState(this, "NotSelectingState", true);
        }

        private void DownloadingStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            RefreshControlStates();
        }

        private void SelectionStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            RefreshControlStates();
        }

        private void RefreshControlStates()
        {
            DeleteButton.Visibility = DownloadButton.Visibility = (SelectionStates.CurrentState?.Name == "SelectingState" && !(DownloadingStates.CurrentState?.Name == "DownloadingState")) ? Visibility.Visible : Visibility.Collapsed;
            AgencyBar.Visibility = (SelectionStates.CurrentState?.Name == "SelectingState" || DownloadingStates.CurrentState?.Name == "DownloadingState") ? Visibility.Collapsed : Visibility.Visible;
            InvertSelectionButton.Visibility = SelectDownloadedButton.Visibility = SelectNotDownloadedButton.Visibility = SelectDownloadingButton.Visibility = CheckToggle.Visibility = SelectAllButton.Visibility = (DownloadingStates.CurrentState?.Name == "DownloadingState") ? Visibility.Collapsed : Visibility.Visible;
            PauseButton.Visibility = CancelButton.Visibility = (DownloadingStates.CurrentState?.Name == "DownloadingState") ? Visibility.Visible : Visibility.Collapsed;
            CheckToggle.IsChecked = SelectionStates.CurrentState?.Name == "SelectingState";
            CanNavigateAway = (DownloadingStates.CurrentState?.Name != "DownloadingState");
        }

        private async void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var cancel = true;
            MessageDialog dialog = new MessageDialog("This will stop downloading and delete what was downloaded of the selected routes. Are you sure?", "Cancel Downloads?");
            dialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler((cmd) => cancel = false)));
            dialog.Commands.Add(new UICommand("No", new UICommandInvokedHandler((cmd) => cancel = true)));
            await dialog.ShowAsync();
            if (cancel) return;
            LoadingRect.Visibility = Visibility.Visible;
            MainProgressRing.IsActive = true;
            DownloadsCancellationTokenSource.Cancel();
            DownloadsCancellationTokenSource = new CancellationTokenSource();
            await DownloadsTask;
            var items = MainList.Items.Where(item => ((item as RouteListing)?.IsChecked).GetValueOrDefault(false)).Select(item => (RouteListing)item).ToArray();
            await DownloadManager.DeleteRoutes(items);
            MainProgressRing.IsActive = false;
            LoadingRect.Visibility = Visibility.Collapsed;
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            DownloadsCancellationTokenSource.Cancel();
            DownloadsCancellationTokenSource = new CancellationTokenSource();
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            LoadingRect.Visibility = Visibility.Visible;
            MainProgressRing.IsActive = true;
            var items = MainList.Items.Where(item => ((item as RouteListing)?.IsChecked).GetValueOrDefault(false)).Select(item => (RouteListing)item).ToArray();
            bool sure = false;
            MessageDialog dialog = new MessageDialog("If you delete these routes, you won't be able to see their schedules unless you are connected to the internet.", "Delete routes?");
            dialog.Commands.Add(new UICommand("Delete Routes", new UICommandInvokedHandler((cmd) => sure = true)));
            dialog.Commands.Add(new UICommand("Cancel", new UICommandInvokedHandler((cmd) => sure = false)));
            await dialog.ShowAsync();
            if (sure)
                await DownloadManager.DeleteRoutes(items);
            MainProgressRing.IsActive = false;
            LoadingRect.Visibility = Visibility.Collapsed;
        }

        private void InvertSelectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckToggle.IsChecked.Value)
                CheckToggle.IsChecked = true;
            foreach (RouteListing listing in MainList.Items)
                listing.IsChecked = !listing.IsChecked;
        }

        private void SelectDownloadedButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckToggle.IsChecked.Value)
                CheckToggle.IsChecked = true;
            foreach (RouteListing listing in MainList.Items)
                listing.IsChecked = listing.IsDownloaded == DownloadStatus.Downloaded;
        }

        private void SelectNotDownloadedButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckToggle.IsChecked.Value)
                CheckToggle.IsChecked = true;
            foreach (RouteListing listing in MainList.Items)
                listing.IsChecked = listing.IsDownloaded == DownloadStatus.NotDownloaded;
        }

        private void SelectDownloadingButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckToggle.IsChecked.Value)
                CheckToggle.IsChecked = true;
            foreach (RouteListing listing in MainList.Items)
                listing.IsChecked = listing.IsDownloaded == DownloadStatus.Downloading;
        }
    }
}
