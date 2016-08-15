using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using static OneAppAway.ServiceDay;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=402347&clcid=0x409

namespace OneAppAway
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Allows tracking page views, exceptions and other telemetry through the Microsoft Application Insights service.
        /// </summary>
        public readonly HamburgerBar MainHamburgerBar = new HamburgerBar();
        public OuterFrame MainOuterFrame;
        public Frame RootFrame;
        private bool NoTitleBar = false;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(320, 320));
            await FileManager.EnsureFolders();
            await OneAppAway._1_1.Views.Controls.TransitStopIconWrapper.LoadImages();
            AdDuplex.AdDuplexClient.Initialize("bef2bb37-a5ad-49d7-9ba6-b1ccaf4be44b");
            Common.SuspensionManager.KnownTypes.Add(typeof(string[]));
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            MainOuterFrame = new OuterFrame();

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (RootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                RootFrame = MainOuterFrame.MainFrame;
                Common.SuspensionManager.RegisterFrame(RootFrame, "appFrame");

                RootFrame.NavigationFailed += OnNavigationFailed;

                TransitionCollection transitions = new TransitionCollection();
                transitions.Add(new EntranceThemeTransition() { FromHorizontalOffset = 200 });
                RootFrame.ContentTransitions = transitions;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    await Common.SuspensionManager.RestoreAsync();
                }

                // Place the frame in the current Window
                //MainHamburgerBar.Content = RootFrame;
                //MainHamburgerBar.SetRootFrame(RootFrame);
            }

            if (RootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter

                if (BandwidthManager.EffectiveBandwidthOptions == BandwidthOptions.Low)
                {
                    switch (SettingsManager.GetSetting<int>("LimitedData.LaunchPage", false, 0))
                    {
                        case 0:
                            RootFrame.Navigate(typeof(BusMapPage), "CurrentLocation");
                            break;
                        case 1:
                            RootFrame.Navigate(typeof(FavoritesPage));
                            break;
                        case 2:
                            RootFrame.Navigate(typeof(RoutesPage));
                            break;
                    }
                }
                else
                {
                    switch (SettingsManager.GetSetting<int>("LaunchPage", false, 0))
                    {
                        case 0:
                            RootFrame.Navigate(typeof(BusMapPage), "CurrentLocation");
                            break;
                        case 1:
                            RootFrame.Navigate(typeof(FavoritesPage));
                            break;
                        case 2:
                            RootFrame.Navigate(typeof(RoutesPage));
                            break;
                    }
                }

                //Test Page
                RootFrame.Navigate(typeof(_1_1.Views.Pages.TransitMapPage));
            }
            // Ensure the current window is active
            Window.Current.Content = MainOuterFrame;// MainHamburgerBar;
            Window.Current.SizeChanged += Current_SizeChanged;
            Window.Current.CoreWindow.SizeChanged += CoreWindow_SizeChanged;
            Window.Current.Activate();
            BandwidthManager.Dispatcher = RootFrame.Dispatcher;
            LocationManager.Dispatcher = RootFrame.Dispatcher;
            SetTitleBar();
            
            

            Message.ShowMessage(new Message() { ShortSummary = "Public transit data powered by OneBusAway.", Caption = "Welcome!", FullText="This app uses data provided by the OneBusAway api. OneBusAway also provides its own app for this platform, and is available for free. This app builds on the functions of the official app, and provides additional functionality not available in OneBusAway's own app.", Id = 1 });
            //if (CurrentApp.LicenseInformation.IsTrial)
            //    MainHamburgerBar.ShowAds = true;

            //using (var db = FileManager.GetDatabase())
            //{
            //    db.CreateTable<BusTrip>();
            //    db.Insert(new BusTrip() { Destination = "Federal Way", Route = "187", Shape = "Square" });
            //}
        }

        private void CoreWindow_SizeChanged(CoreWindow sender, WindowSizeChangedEventArgs args)
        {
        }

        private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await Common.SuspensionManager.SaveAsync();
            deferral.Complete();
        }

        internal void SetTitleBar()
        {
            Func<Color, Color> darken = clr => Color.FromArgb(clr.A, (byte)(clr.R / 2), (byte)(clr.G / 2), (byte)(clr.B / 2));
            //Func<Color, Color> lighten = clr => Color.FromArgb(clr.A, (byte)(128 + clr.R / 2), (byte)(128 + clr.G / 2), (byte)(1287 + clr.B / 2));
            //Color accentColor = ((Color)App.Current.Resources["SystemColorControlAccentColor"]);
            Color foreground = Colors.White;
            var background = Color.FromArgb(255, byte.Parse("30", System.Globalization.NumberStyles.HexNumber), byte.Parse("30", System.Globalization.NumberStyles.HexNumber), byte.Parse("30", System.Globalization.NumberStyles.HexNumber));
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = background;
            titleBar.InactiveBackgroundColor = titleBar.BackgroundColor;
            titleBar.ForegroundColor = foreground;
            titleBar.InactiveForegroundColor = darken(foreground);
            titleBar.ButtonBackgroundColor = titleBar.BackgroundColor;
            titleBar.ButtonForegroundColor = titleBar.ForegroundColor;
            titleBar.ButtonInactiveBackgroundColor = titleBar.InactiveBackgroundColor;
            titleBar.ButtonInactiveForegroundColor = titleBar.InactiveForegroundColor;
            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
            

            //if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.ApplicationModel.Core.CoreApplicationViewTitleBar"))
            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            

            ApplicationView.GetForCurrentView().ExitFullScreenMode();

            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                NoTitleBar = true;
                MainOuterFrame.SystemButtonsWidth = 0;
                Action setOcclusion = () =>
                {
                    var bar = StatusBar.GetForCurrentView();
                    var occlusion = bar.OccludedRect;
                    if (occlusion.Width > occlusion.Height)
                        MainHamburgerBar.Margin = new Thickness(0, occlusion.Height, 0, 0);
                    else if (occlusion.X == 0)
                        MainHamburgerBar.Margin = new Thickness(occlusion.Width, 0, 0, 0);
                    else
                        MainHamburgerBar.Margin = new Thickness(0, 0, occlusion.Width, 0);
                
                };
                var statusBar = StatusBar.GetForCurrentView();
                statusBar.BackgroundColor = background;
                statusBar.BackgroundOpacity = 1;
                statusBar.ForegroundColor = foreground;
                setOcclusion();
                ApplicationView.GetForCurrentView().VisibleBoundsChanged += (sender, e) => setOcclusion();
            }
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            MainOuterFrame.SystemButtonsWidth = NoTitleBar ? 0 : sender.SystemOverlayRightInset;
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);
            if (args.Kind == ActivationKind.ToastNotification)
            {
                var toastArgs = args as ToastNotificationActivatedEventArgs;
                if (toastArgs.Argument.StartsWith("messageTapped"))
                {
                    await MainHamburgerBar.ShowPopup(null, AnimationDirection.Bottom, Window.Current.Bounds.Width * 0.9, 100 + 30000 / Window.Current.Bounds.Width, typeof(MessagePopupPage), SettingsManager.GetSetting<string>("Message" + toastArgs.Argument.Substring(13), false));
                }
                if (toastArgs.Argument.StartsWith("suppressMessage"))
                {
                    SettingsManager.SetSetting("SuppressMessage" + toastArgs.Argument.Substring(15), true, true);
                }
            }
        }

        private void App_BackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = GoBack();
        }

        internal bool GoBack()
        {
            if (RootFrame.Content is NavigationFriendlyPage)
            {
                return ((NavigationFriendlyPage)RootFrame.Content).GoBack();
            }
            return false;
        }
    }
}
