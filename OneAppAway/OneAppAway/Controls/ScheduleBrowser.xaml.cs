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
                    typedSender.MainStackPanel.Children.Add(new TextBlock() { Text = (await Data.GetRoute(lastRoute, typedSender.MasterCancellationTokenSource.Token)).Value.Name + " to " + lastDestination, FontSize = 18, Foreground = new SolidColorBrush(lighten(accentColor)), TextWrapping = TextWrapping.WrapWholeWords });
                    timesControl = new ItemsControl();
                    typedSender.MainStackPanel.Children.Add(timesControl);
                }
                //Button label = new Button() { Content = (item.ScheduledArrivalTime - TimeSpan.FromMinutes(4)).ToString("h:mm") + "" + item.ScheduledArrivalTime.ToString("h:mm"), HorizontalAlignment = HorizontalAlignment.Center, FontWeight = item.ScheduledArrivalTime.Hour >= 12 ? Windows.UI.Text.FontWeights.Bold : Windows.UI.Text.FontWeights.Normal, Tag = item.Trip, FontFamily = new FontFamily("Segoe UI Symbol") };
                Button label = new Button() { Content = item.ScheduledArrivalTime.ToString("h:mm"), HorizontalAlignment = HorizontalAlignment.Center, FontWeight = item.ScheduledArrivalTime.Hour >= 12 ? Windows.UI.Text.FontWeights.ExtraBold : Windows.UI.Text.FontWeights.Normal, Tag = item.Trip, FontFamily = new FontFamily("Segoe UI Symbol") };
                label.Foreground = HighlightedTrips.Contains(item.Trip) ? new SolidColorBrush(lighten(accentColor)) : new SolidColorBrush(Colors.White);
                typedSender.ShownLabels.Add(label);
                label.Click += Label_Click;
                VariableSizedWrapGrid.SetColumnSpan(label, 1);
                if (technicalMode)
                {
                    ToolTip toolTip = new ToolTip() { Content = "Trip=" + item.Trip + ", Route=" + item.Route + ", Stop=" + item.Stop };
                    ToolTipService.SetToolTip(label, toolTip);
                }
                timesControl.Items.Add(label);
            }
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
