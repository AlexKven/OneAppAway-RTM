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
        private RadioButton SettingsButton = new RadioButton();
        private Grid InnerGrid = new Grid();
        private Popup PopupControl = new Popup();
        private Frame RootFrame;

        private static int NumPopupRequests = 0;
        Queue<Tuple<UIElement, double, double, Type, object, AutoResetEvent>> PopupQueue = new Queue<Tuple<UIElement, double, double, Type, object, AutoResetEvent>>();

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
            InnerGrid = (Grid)MainSplitView.Content;
            PopupControl = HelperFunctions.FindControl<Popup>(InnerGrid, "PopupControl");
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

        public async Task ShowPopup(UIElement element, double width, double height, Type sourcePageType, object parameter = null)
        {
            AutoResetEvent are = new AutoResetEvent(false);
            PopupQueue.Enqueue(new Tuple<UIElement, double, double, Type, object, AutoResetEvent>(element, width, height, sourcePageType, parameter, are));
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
            OnShowPopup(next.Item1, next.Item2, next.Item3, next.Item4, next.Item5);
            next.Item6.Set();
        }

        private void OnShowPopup(UIElement element, double width, double height, Type sourcePageType, object parameter)
        {
            Frame popupFrame = ((Frame)PopupControl.Child);
            popupFrame.Width = Min(width, this.ActualWidth);
            popupFrame.Height = Min(height, this.ActualHeight);
            var centerTop = element.TransformToVisual(InnerGrid).TransformPoint(new Point(element.RenderSize.Width / 2, element.RenderSize.Height));
            centerTop = new Point(centerTop.X - popupFrame.Width / 2, centerTop.Y);
            if (centerTop.X + popupFrame.Width > this.ActualWidth)
                centerTop = new Point(this.ActualWidth - popupFrame.Width, centerTop.Y);
            else if (centerTop.X < 0)
                centerTop = new Point(0, centerTop.Y);
            if (centerTop.Y + popupFrame.Height > this.ActualHeight)
                centerTop = new Point(centerTop.X, this.ActualHeight - popupFrame.Height);
            else if (centerTop.Y < 0)
                centerTop = new Point(centerTop.X, 0);
            PopupControl.Tag = centerTop;
            popupFrame.Navigate(sourcePageType, parameter);
            PopupControl.IsOpen = true;
        }

        private void PopupControl_Opened(object sender, object e)
        {
            TranslateTransform translation;
            Point relativePoint = PopupControl.Tag is Point ? (Point)PopupControl.Tag : new Point();
            PopupControl.RenderTransform = new TranslateTransform() { X = relativePoint.X };
            translation = (TranslateTransform)PopupControl.RenderTransform;
            DoubleAnimation fadeIn = new DoubleAnimation() { To = 1, From = 0, Duration = TimeSpan.FromSeconds(0.15) };
            DoubleAnimation slideDown = new DoubleAnimation() { To = relativePoint.Y, From = relativePoint.Y - 20, Duration = TimeSpan.FromSeconds(0.15) };
            Storyboard storyboard = new Storyboard();
            Storyboard.SetTarget(fadeIn, PopupControl);
            Storyboard.SetTargetProperty(fadeIn, "Opacity");
            Storyboard.SetTarget(slideDown, translation);
            Storyboard.SetTargetProperty(slideDown, "Y");
            storyboard.Children.Add(fadeIn);
            storyboard.Children.Add(slideDown);
            storyboard.Begin();
        }

        private void PopupControl_Closed(object sender, object e)
        {
            PopupControl.Opacity = 0;
            ShowNextPopup();
        }
    }
}
