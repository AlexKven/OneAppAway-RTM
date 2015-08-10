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

        public HamburgerBar()
        {
            this.InitializeComponent();
            
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (Template == null) return;
            MainSplitView = HelperFunctions.FindControl<SplitView>(this, "MainSplitView");
        }

#pragma warning disable CS1998
        private async void Button_Click(object sender, RoutedEventArgs e)
        {

        }
#pragma warning restore CS1998

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

        private void CenterOnLocationButton_Click(object sender, RoutedEventArgs e)
        {
            if (((App)(App.Current)).RootFrame.Content is BusMapPage)
            {
                ((BusMapPage)((App)(App.Current)).RootFrame.Content).CenterOnCurrentLocation();
            }
            else
            {
                ((App)(App.Current)).RootFrame.Navigate(typeof(BusMapPage), "CurrentLocation");
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            ((App)(App.Current)).RootFrame.Navigate(typeof(SettingsPage));
        }
    }
}
