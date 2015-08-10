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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OneAppAway
{
    public sealed partial class RouteListingControl : UserControl
    {
        public RouteListingControl()
        {
            this.InitializeComponent();
            MainPanel.DataContext = this;
        }

        public static readonly DependencyProperty RouteProperty = DependencyProperty.Register("Route", typeof(BusRoute), typeof(RouteListingControl), new PropertyMetadata(new BusRoute(), OnRouteChangedStatic));
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(RouteListingControl), new PropertyMetadata(false));
        public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register("Progress", typeof(double), typeof(RouteListingControl), new PropertyMetadata(0.25));
        public static readonly DependencyProperty ShowCheckBoxProperty = DependencyProperty.Register("ShowCheckBox", typeof(bool), typeof(RouteListingControl), new PropertyMetadata(false, OnShowCheckBoxChangedStatic));
        public static readonly DependencyProperty ShowProgressProperty = DependencyProperty.Register("ShowProgress", typeof(bool), typeof(RouteListingControl), new PropertyMetadata(false, OnShowProgressChangedStatic));

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public BusRoute Route
        {
            get { return (BusRoute)GetValue(RouteProperty); }
            set { SetValue(RouteProperty, value); }
        }

        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        public bool ShowCheckBox
        {
            get { return (bool)GetValue(ShowCheckBoxProperty); }
            set { SetValue(ShowCheckBoxProperty, value); }
        }

        public bool ShowProgress
        {
            get { return (bool)GetValue(ShowProgressProperty); }
            set { SetValue(ShowProgressProperty, value); }
        }

        private static void OnRouteChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RouteListingControl typedSender = (RouteListingControl)sender;
            BusRoute route = (BusRoute)e.NewValue;
            typedSender.RouteNameBlock.Text = route.Name;
            typedSender.RouteDescriptionBlock.Text = route.Description;
        }

        private static void OnShowCheckBoxChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue != (bool)e.OldValue)
            {
                RouteListingControl typedSender = (RouteListingControl)sender;
                //DoubleAnimation animation = new DoubleAnimation() { To = (bool)e.NewValue ? 30 : 0, Duration = TimeSpan.FromSeconds(0.25) };
                //Storyboard.SetTarget(animation, typedSender.MainCheckBox);
                //Storyboard.SetTargetProperty(animation, "Width");
                //Storyboard sb = new Storyboard();
                //sb.Children.Add(animation);
                //sb.Begin();
                typedSender.MainCheckBox.Width = (bool)e.NewValue ? 30 : 0;
            }
        }

        private static void OnShowProgressChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue != (bool)e.OldValue)
            {
                RouteListingControl typedSender = (RouteListingControl)sender;
                //DoubleAnimation animation = new DoubleAnimation() { To = (bool)e.NewValue ? 3 : 0, Duration = TimeSpan.FromSeconds(0.25) };
                //Storyboard.SetTarget(animation, typedSender.MainProgressBar);
                //Storyboard.SetTargetProperty(animation, "Height");
                //Storyboard sb = new Storyboard();
                //sb.Children.Add(animation);
                //sb.Begin();
                typedSender.MainProgressBar.Height = (bool)e.NewValue ? 3 : 0;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ShowCheckBox)
                MainCheckBox.IsChecked = !MainCheckBox.IsChecked;
            else
                OnClick();
        }

        private void OnClick()
        {
            if (Click != null) Click(this, new EventArgs());
        }

        public event EventHandler Click;
    }
}
