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

        public RoutesPage()
        {
            this.InitializeComponent();
        }

        private CancellationTokenSource MasterCancellationTokenSource = new CancellationTokenSource();
        private CancellationTokenSource DownloadsCancellationTokenSource = new CancellationTokenSource();

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            foreach (var agency in await ApiLayer.GetTransitAgencies(MasterCancellationTokenSource.Token))
            {
                AgenciesListView.Items.Add(agency);
                AgenciesListView.SelectedIndex = 0;
            }
        }

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (!CanNavigateAway)
            {
                e.Cancel = true;
                MessageDialog dialog = new MessageDialog("If you leave this page, the downloads will stop. Are you sure you want to leave?", "Downloads in Progress");
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
            foreach (var rte in await ApiLayer.GetBusRoutes(agency.ID, MasterCancellationTokenSource.Token))
            {
                routesList.Add(new RouteListing(rte));
            }
            foreach (var rte in routesList)
                MainList.Items.Add(rte);
            await FileManager.SaveAgency(agency, routesList.Select(item => item.Route.ID).ToArray());
            MainProgressRing.IsActive = false;
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
            var errors = await DownloadManager.DownloadAll(async (p, m) => await SetProgress(p, m), cancellationToken, items);
            if (cancellationToken.IsCancellationRequested)
            {
                await SetProgress(1, "Cancelled");
            }
            //foreach (RouteListing item in items)
            //{
            //    await DownloadManager.Create(item);
            //}
            //BusStop? stop;
            //while ((stop = DownloadManager.DownloadNext()) != null)
            //{
            //    DownloadManager.StopDownloaded(stop.Value);
            //    await Task.Delay(5);
            //}
            VisualStateManager.GoToState(this, "NotDownloadingState", true);
            if (errors != null && errors.Length > 0)
            {
                VisualStateManager.GoToState(this, "SelectingState", true);
                foreach (RouteListing listing in MainList.Items)
                    listing.IsChecked = errors.Contains(listing);
                MessageDialog dialog = new MessageDialog("There was an error downloading " + errors.Length.ToString() + " of the routes. The affected routes are selected.", "Error downloading some routes");
                await dialog.ShowAsync();
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
            CheckToggle.Visibility = SelectAllButton.Visibility = (DownloadingStates.CurrentState?.Name == "DownloadingState") ? Visibility.Collapsed : Visibility.Visible;
            CancelButton.Visibility = (DownloadingStates.CurrentState?.Name == "DownloadingState") ? Visibility.Visible : Visibility.Collapsed;
            CheckToggle.IsChecked = SelectionStates.CurrentState?.Name == "SelectingState";
            CanNavigateAway = (DownloadingStates.CurrentState?.Name != "DownloadingState");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DownloadsCancellationTokenSource.Cancel();
            DownloadsCancellationTokenSource = new CancellationTokenSource();
        }
    }
}
