using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OneAppAway
{
    public sealed partial class OuterFrame : Page
    {
        public OuterFrame()
        {
            this.InitializeComponent();
            Loaded += OuterFrame_Loaded;
        }

        #region Properties
        private double _SystemButtonsWidth;

        public double SystemButtonsWidth
        {
            get { return _SystemButtonsWidth; }
            set
            {
                _SystemButtonsWidth = value;
                RefreshTitleBarSizes();
            }
        }
        #endregion

        private void RefreshTitleBarSizes()
        {
            SystemButtonsColumn.Width = SystemButtonsColumn2.Width = new GridLength(SystemButtonsWidth);
        }

        private void OuterFrame_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SetTitleBar(TitleBar);
        }

        private void EnsureNavigation<T>(object param)
        {
            if (MainFrame?.CurrentSourcePageType != typeof(T))
                MainFrame?.Navigate(typeof(T), param);
        }

        private void EnsureNavigation<T>()
        {
            EnsureNavigation<T>(null);
        }

        #region Button Click/Check Events
        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
        }

        private void MapButton_Checked(object sender, RoutedEventArgs e)
        {
            EnsureNavigation<BusMapPage>("CurrentLocation");
        }

        private void RoutesButton_Checked(object sender, RoutedEventArgs e)
        {
            EnsureNavigation<RoutesPage>();
        }

        private void SettingsButton_Checked(object sender, RoutedEventArgs e)
        {
            EnsureNavigation<SettingsPage>();
        }

        private void AboutButton_Checked(object sender, RoutedEventArgs e)
        {
            EnsureNavigation<AboutPage>();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                var frame = new Frame();
                frame.Navigate(typeof(BusMapPage));
                Window.Current.Content = frame;
                newViewId = ApplicationView.GetForCurrentView().Id;
                Window.Current.Activate();
            });
            await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
        }
        #endregion

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            CheckCorrectButton();
            RefreshBackButton();
            RefreshTitleBarControls();
        }

        void RefreshBackButton()
        {
            BackButton.IsEnabled = (MainFrame.CanGoBack);
        }

        private void CheckCorrectButton()
        {
            if (MainFrame?.CurrentSourcePageType == typeof(BusMapPage))
                MapButton.IsChecked = true;
            else if (MainFrame?.CurrentSourcePageType == typeof(RoutesPage))
                RoutesButton.IsChecked = true;
            else if (MainFrame?.CurrentSourcePageType == typeof(SettingsPage))
                SettingsButton.IsChecked = true;
            else if (MainFrame?.CurrentSourcePageType == typeof(AboutPage))
                AboutButton.IsChecked = true;
            else
            {
                MapButton.IsChecked = false;
                RoutesButton.IsChecked = false;
                SettingsButton.IsChecked = false;
            }
        }

        private void MainFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            TitleOverlay.Content = null;
            TitleContent.Content = null;
        }

        private void TitleContent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TitleCompensationColumn.Width = new GridLength(TitleContent.ActualWidth);
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshTitleBarControls();
        }

        private void RefreshTitleBarControls()
        {
            NavigationFriendlyPage page;
            double width = this.ActualWidth - 50 - SystemButtonsWidth;
            if ((page = MainFrame.Content as NavigationFriendlyPage) != null && width > 0)
            {
                page.OnRefreshTitleBarControls(this, width);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ((App)App.Current).GoBack();
        }
    }
}
