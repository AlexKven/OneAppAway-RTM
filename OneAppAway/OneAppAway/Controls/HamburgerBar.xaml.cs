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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OneAppAway
{
    public sealed partial class HamburgerBar : ContentControl
    {
        private SplitView MainSplitView = new SplitView();
        private RadioButton MapButton = new RadioButton();
        private RadioButton RoutesButton = new RadioButton();
        private RadioButton SettingsButton = new RadioButton();
        private Frame RootFrame;

        public HamburgerBar()
        {
            this.InitializeComponent();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (Template == null) return;
            MainSplitView = HelperFunctions.FindControl<SplitView>(this, "MainSplitView");
            MapButton = HelperFunctions.FindControl<RadioButton>(MainSplitView.Pane, "MapButton");
            SettingsButton = HelperFunctions.FindControl<RadioButton>(MainSplitView.Pane, "SettingsButton");
            RoutesButton = HelperFunctions.FindControl<RadioButton>(MainSplitView.Pane, "RoutesButton");
            CheckCorrectButton();
        }

        private void EnsureNavigation<T>()
        {
            if (RootFrame?.CurrentSourcePageType != typeof(T))
                RootFrame?.Navigate(typeof(T));
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
        }

        private void BandwidthButton_Click(object sender, RoutedEventArgs e)
        {
            ApplicationSettings settings = (ApplicationSettings)App.Current.Resources["Settings"];
            settings.BandwidthSetting = (BandwidthOptions)(((int)settings.BandwidthSetting + 1) % 4);
            App.Current.Resources["Settings"] = settings;
        }

        private void MapButton_Checked(object sender, RoutedEventArgs e)
        {
            EnsureNavigation<BusMapPage>();
        }

        private void RoutesButton_Checked(object sender, RoutedEventArgs e)
        {
            EnsureNavigation<RoutesPage>();
        }

        private void SettingsButton_Checked(object sender, RoutedEventArgs e)
        {
            EnsureNavigation<SettingsPage>();
        }

        public void SetRootFrame(Frame rootFrame)
        {
            RootFrame = rootFrame;
            RootFrame.Navigated += RootFrame_Navigated;
            CheckCorrectButton();
        }

        private void CheckCorrectButton()
        {
            if (RootFrame?.CurrentSourcePageType == typeof(BusMapPage))
                MapButton.IsChecked = true;
            else if (RootFrame?.CurrentSourcePageType == typeof(RoutesPage))
                RoutesButton.IsChecked = true;
            else if (RootFrame?.CurrentSourcePageType == typeof(SettingsPage))
                SettingsButton.IsChecked = true;
            else
            {
                MapButton.IsChecked = false;
                RoutesButton.IsChecked = false;
                SettingsButton.IsChecked = false;
            }
        }

        private void RootFrame_Navigated(object sender, NavigationEventArgs e)
        {
            CheckCorrectButton();
        }
    }
}
