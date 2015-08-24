using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class ScheduleBrowser : UserControl
    {
        public ScheduleBrowser()
        {
            this.InitializeComponent();
        }

        private List<Control> ShownLabels = new List<Control>();
        private static List<string> HighlightedTrips = new List<string>();

        private CancellationTokenSource MasterCancellationTokenSource = new CancellationTokenSource();

        public static readonly DependencyProperty ScheduleProperty = DependencyProperty.Register("Schedule", typeof(DaySchedule), typeof(ScheduleBrowser), new PropertyMetadata(null, OnScheduleChangedStatic));

        public static async void OnScheduleChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            bool technicalMode = SettingsManager.GetSetting("TechnicalMode", false, true);
            Func<Color, Color> lighten = clr => Color.FromArgb(clr.A, (byte)(128 + clr.R / 2), (byte)(128 + clr.G / 2), (byte)(128 + clr.B / 2));
            Color accentColor = ((Color)App.Current.Resources["SystemColorControlAccentColor"]);
            ScheduleBrowser typedSender = (ScheduleBrowser)sender;
            typedSender.MainStackPanel.Children.Clear();
            if (typedSender.Schedule == null) return;
            string lastRoute = null;
            string lastDestination = null;
            ItemsControl timesControl = null;
            while (typedSender.ShownLabels.Count > 0)
            {
                (typedSender.ShownLabels.First() as Button).Click -= Label_Click;
                typedSender.ShownLabels.RemoveAt(0);
            }
            foreach (var item in typedSender.Schedule)
            {
                if (item.Route != lastRoute || item.Destination != lastDestination)
                {
                    lastRoute = item.Route;
                    lastDestination = item.Destination;
                    Grid panel = new Grid();
                    panel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    panel.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                    TextBlock block = new TextBlock() { Margin = new Thickness(2), VerticalAlignment = VerticalAlignment.Center, Text = (await Data.GetRoute(lastRoute, typedSender.MasterCancellationTokenSource.Token)).Value.Name + " to " + lastDestination, FontSize = 16, Foreground = new SolidColorBrush(lighten(accentColor)), TextWrapping = TextWrapping.WrapWholeWords };
                    Button favoriteButton = new Button() { Foreground = new SolidColorBrush(Colors.Yellow), Margin = new Thickness(5), VerticalAlignment = VerticalAlignment.Center, Content = "", FontFamily = new FontFamily("Segoe MDL2 Assets"), Tag = new string[] { typedSender.Schedule.Stop, lastRoute, lastDestination } };
                    favoriteButton.Click += FavoriteButton_Click;
                    Grid.SetColumn(favoriteButton, 1);
                    panel.Children.Add(block);
                    panel.Children.Add(favoriteButton);
                    typedSender.MainStackPanel.Children.Add(panel);
                    timesControl = new ItemsControl();
                    typedSender.MainStackPanel.Children.Add(timesControl);
                }
                //Button label = new Button() { Content = (item.ScheduledArrivalTime - TimeSpan.FromMinutes(4)).ToString("h:mm") + "" + item.ScheduledArrivalTime.ToString("h:mm"), HorizontalAlignment = HorizontalAlignment.Center, FontWeight = item.ScheduledArrivalTime.Hour >= 12 ? Windows.UI.Text.FontWeights.Bold : Windows.UI.Text.FontWeights.Normal, Tag = item.Trip, FontFamily = new FontFamily("Segoe UI Symbol") };
                Button label = new Button() { HorizontalAlignment = HorizontalAlignment.Center, FontWeight = item.ScheduledDepartureTime.Hour >= 12 ? Windows.UI.Text.FontWeights.Bold : Windows.UI.Text.FontWeights.Normal, Tag = item.Trip, Background = new SolidColorBrush(Colors.Transparent), Template = (ControlTemplate)App.Current.Resources["SimpleButtonTemplate"] };
                label.Foreground = HighlightedTrips.Contains(item.Trip) ? new SolidColorBrush(lighten(accentColor)) : new SolidColorBrush(Colors.White);
                typedSender.ShownLabels.Add(label);
                label.Click += Label_Click;
                if (item.ScheduledArrivalTime == null)
                {
                    label.Content = item.ScheduledDepartureTime.ToString("h:mm");
                    VariableSizedWrapGrid.SetColumnSpan(label, 2);
                }
                else
                {
                    StackPanel panel = new StackPanel() { Orientation = Orientation.Horizontal };
                    string text1 = "(" + (item.ScheduledDepartureTime - item.ScheduledArrivalTime.Value).TotalMinutes.ToString() + ")";
                    string text2 = item.ScheduledDepartureTime.ToString("h:mm");
                    panel.Children.Add(new TextBlock() { Text = text1, Opacity = 0.5, FontWeight = Windows.UI.Text.FontWeights.Normal, VerticalAlignment = VerticalAlignment.Center, FontSize = ((text1.Length + text2.Length) > 7) ? 13 : 15 });
                    panel.Children.Add(new TextBlock() { Text = text2, VerticalAlignment = VerticalAlignment.Center, FontSize = ((text1.Length + text2.Length) > 7) ? 14 : 16 });
                    label.Content = panel;
                    VariableSizedWrapGrid.SetColumnSpan(label, 3);
                }
                if (technicalMode)
                {
                    ToolTip toolTip = new ToolTip() { Content = "Trip=" + item.Trip + ", Route=" + item.Route + ", Stop=" + item.Stop };
                    ToolTipService.SetToolTip(label, toolTip);
                }
                timesControl.Items.Add(label);
            }
        }

        private static async void FavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            await ((App)App.Current).MainHamburgerBar.ShowPopup((UIElement)((Button)sender).Parent, 300, 350, typeof(AddToFavoritesPage), ((Button)sender).Tag);
        }

        private static void Label_Click(object sender, RoutedEventArgs e)
        {
            ToggleTripHighlight(((Control)sender).Tag.ToString(), (Control)sender);
        }

        private static void ToggleTripHighlight(string tripId, Control label)
        {
            if (HighlightedTrips.Contains(tripId))
                HighlightedTrips.Remove(tripId);
            else
                HighlightedTrips.Add(tripId);
            Func<Color, Color> lighten = clr => Color.FromArgb(clr.A, (byte)(128 + clr.R / 2), (byte)(128 + clr.G / 2), (byte)(128 + clr.B / 2));
            Color accentColor = ((Color)App.Current.Resources["SystemColorControlAccentColor"]);
            label.Foreground = HighlightedTrips.Contains(label.Tag.ToString()) ? new SolidColorBrush(lighten(accentColor)) : new SolidColorBrush(Colors.White);
        }

        public DaySchedule Schedule
        {
            get { return (DaySchedule)GetValue(ScheduleProperty); }
            set { SetValue(ScheduleProperty, value); }
        }
    }
}
