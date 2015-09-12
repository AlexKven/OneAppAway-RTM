using AdDuplex;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using static System.Math;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OneAppAway
{
    public sealed partial class HamburgerBar : ContentControl
    {
        private SplitView MainSplitView = new SplitView();
        private RadioButton MapButton = new RadioButton();
        private RadioButton RoutesButton = new RadioButton();
        private RadioButton CompactRoutesButton = new RadioButton();
        private RadioButton SettingsButton = new RadioButton();
        private RadioButton AboutButton = new RadioButton();
        private AdRow MainAdRow = null;
        private Grid InnerGrid = new Grid();
        private Popup PopupControl = new Popup();
        private Frame RootFrame;
        
        private bool _ShowAds = false;

        public bool ShowAds
        {
            get { return _ShowAds; }
            set
            {
                _ShowAds = value;
                if (MainAdRow != null)
                    MainAdRow.ShowAds = ShowAds;
            }
        }

        Queue<Tuple<UIElement, AnimationDirection, double, double, Type, object, AutoResetEvent>> PopupQueue = new Queue<Tuple<UIElement, AnimationDirection, double, double, Type, object, AutoResetEvent>>();

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
            AboutButton = HelperFunctions.FindControl<RadioButton>(MainSplitView.Pane, "AboutButton");
            RoutesButton = HelperFunctions.FindControl<RadioButton>(MainSplitView.Pane, "RoutesButton");
            CompactRoutesButton = HelperFunctions.FindControl<RadioButton>(MainSplitView.Content, "CompactRoutesButton");
            InnerGrid = (Grid)MainSplitView.Content;
            PopupControl = HelperFunctions.FindControl<Popup>(InnerGrid, "PopupControl");
            MainAdRow = HelperFunctions.FindControl<AdRow>(InnerGrid, "MainAdRow");
            MainAdRow.ShowAds = ShowAds;
            CheckCorrectButton();
        }

        private void EnsureNavigation<T>(object param)
        {
            if (RootFrame?.CurrentSourcePageType != typeof(T))
                RootFrame?.Navigate(typeof(T), param);
        }

        private void EnsureNavigation<T>()
        {
            EnsureNavigation<T>(null);
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
            else if (RootFrame?.CurrentSourcePageType == typeof(AboutPage))
                AboutButton.IsChecked = true;
            else
            {
                MapButton.IsChecked = false;
                RoutesButton.IsChecked = false;
                SettingsButton.IsChecked = false;
            }
        }

        private async void RootFrame_Navigated(object sender, NavigationEventArgs e)
        {
            CheckCorrectButton();
            await Task.Delay(1000);
            if (ActualWidth < 500)
            {
                if (!SettingsManager.GetSetting("DownloadRoutesTipShown", true, false) && (new Random()).NextDouble() < 0.05)
                {
                    SettingsManager.SetSetting("DownloadRoutesTipShown", true, true);
                    await ShowPopup(CompactRoutesButton, AnimationDirection.Bottom, 250, 135, typeof(HelpTip), new Tuple<AnimationDirection, Thickness, string>(AnimationDirection.Top, new Thickness(0), "Tap here to download route schedules for offline viewing."));
                }
            }
        }

        public async Task ShowPopup(UIElement element, AnimationDirection position, double width, double height, Type sourcePageType, object parameter = null)
        {
            AutoResetEvent are = new AutoResetEvent(false);
            PopupQueue.Enqueue(new Tuple<UIElement, AnimationDirection, double, double, Type, object, AutoResetEvent>(element, position, width, height, sourcePageType, parameter, are));
            if (!PopupControl.IsOpen)
                ShowNextPopup();
            await Task.Run(() => are.WaitOne());
        }

        public void DismissPopup()
        {
            PopupControl.IsOpen = false;
        }

        private void ShowNextPopup()
        {
            if (PopupQueue.Count == 0) return;
            var next = PopupQueue.Dequeue();
            OnShowPopup(next.Item1, next.Item2, next.Item3, next.Item4, next.Item5, next.Item6);
            next.Item7.Set();
        }

        private void OnShowPopup(UIElement element, AnimationDirection position, double width, double height, Type sourcePageType, object parameter)
        {
            Frame popupFrame = ((Frame)PopupControl?.Child);
            if (popupFrame == null)
            {
                popupFrame = new Frame();
                PopupControl.Child = popupFrame;
            }
            popupFrame.Width = Min(width, InnerGrid.ActualWidth);
            popupFrame.Height = Min(height, InnerGrid.ActualHeight);
            var point = new Point();
            switch (position)
            {
                case AnimationDirection.Top:
                    point = element == null ? new Point(InnerGrid.ActualWidth / 2, InnerGrid.ActualHeight) : element.TransformToVisual(InnerGrid).TransformPoint(new Point(element.RenderSize.Width / 2, 0));
                    point = new Point(point.X - popupFrame.Width / 2, point.Y - popupFrame.Height);
                    break;
                case AnimationDirection.Bottom:
                    point = element == null ? new Point(InnerGrid.ActualWidth / 2, 0) : element.TransformToVisual(InnerGrid).TransformPoint(new Point(element.RenderSize.Width / 2, element.RenderSize.Height));
                    point = new Point(point.X - popupFrame.Width / 2, point.Y);
                    break;
                case AnimationDirection.Left:
                    point = element == null ? new Point(InnerGrid.ActualWidth, InnerGrid.ActualHeight / 2) : element.TransformToVisual(InnerGrid).TransformPoint(new Point(0, element.RenderSize.Height / 2));
                    point = new Point(point.X - popupFrame.Width, point.Y - popupFrame.Height / 2);
                    break;
                case AnimationDirection.Right:
                    point = element == null ? new Point(0, InnerGrid.ActualHeight / 2) : element.TransformToVisual(InnerGrid).TransformPoint(new Point(element.RenderSize.Width, element.RenderSize.Height / 2));
                    point = new Point(point.X, point.Y - popupFrame.Height / 2);
                    break;
            }
            if (point.X + popupFrame.Width > InnerGrid.ActualWidth)
                point = new Point(InnerGrid.ActualWidth - popupFrame.Width, point.Y);
            else if (point.X < 0)
                point = new Point(0, point.Y);
            if (point.Y + popupFrame.Height > InnerGrid.ActualHeight)
                point = new Point(point.X, InnerGrid.ActualHeight - popupFrame.Height);
            else if (point.Y < 0)
                point = new Point(point.X, 0);
            PopupControl.Tag = new Tuple<Point, AnimationDirection>(point, position);
            popupFrame.Navigate(sourcePageType, parameter);
            PopupControl.IsOpen = true;
        }

        private void PopupControl_Opened(object sender, object e)
        {
            TranslateTransform translation;
            Tuple<Point, AnimationDirection> relativePoint = PopupControl.Tag as Tuple<Point, AnimationDirection> ?? new Tuple<Point, AnimationDirection>(new Point(), AnimationDirection.Top);
            PopupControl.RenderTransform = new TranslateTransform() { X = relativePoint.Item1.X, Y = relativePoint.Item1.Y };
            translation = (TranslateTransform)PopupControl.RenderTransform;
            DoubleAnimation fadeIn = new DoubleAnimation() { To = 1, From = 0, Duration = TimeSpan.FromSeconds(0.15) };
            double aniTo = (relativePoint.Item2 == AnimationDirection.Top || relativePoint.Item2 == AnimationDirection.Bottom) ? relativePoint.Item1.Y : relativePoint.Item1.X;
            double aniFrom = (relativePoint.Item2 == AnimationDirection.Bottom || relativePoint.Item2 == AnimationDirection.Right) ? aniTo - 20 : aniTo + 20;
            DoubleAnimation slide = new DoubleAnimation() { To = aniTo, From = aniFrom, Duration = TimeSpan.FromSeconds(0.15) };
            Storyboard storyboard = new Storyboard();
            Storyboard.SetTarget(fadeIn, PopupControl);
            Storyboard.SetTargetProperty(fadeIn, "Opacity");
            Storyboard.SetTarget(slide, translation);
            if (relativePoint.Item2 == AnimationDirection.Top || relativePoint.Item2 == AnimationDirection.Bottom)
                Storyboard.SetTargetProperty(slide, "Y");
            else
                Storyboard.SetTargetProperty(slide, "X");
            storyboard.Children.Add(fadeIn);
            storyboard.Children.Add(slide);
            storyboard.Begin();
        }

        private void PopupControl_Closed(object sender, object e)
        {
            PopupControl.Opacity = 0;
            ShowNextPopup();
        }
    }
}
