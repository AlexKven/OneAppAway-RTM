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

        private CancellationTokenSource MasterCancellationTokenSource = new CancellationTokenSource();

        public static readonly DependencyProperty ScheduleProperty = DependencyProperty.Register("Schedule", typeof(DaySchedule), typeof(ScheduleBrowser), new PropertyMetadata(null, OnScheduleChangedStatic));

        public static async void OnScheduleChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Func<Color, Color> lighten = clr => Color.FromArgb(clr.A, (byte)(128 + clr.R / 2), (byte)(128 + clr.G / 2), (byte)(128 + clr.B / 2));
            Color accentColor = ((Color)App.Current.Resources["SystemColorControlAccentColor"]);
            ScheduleBrowser typedSender = (ScheduleBrowser)sender;
            typedSender.MainStackPanel.Children.Clear();
            if (typedSender.Schedule == null) return;
            string lastRoute = null;
            string lastDestination = null;
            ItemsControl timesControl = null;
            foreach (var item in typedSender.Schedule)
            {
                if (item.Route != lastRoute || item.Destination != lastDestination)
                {
                    lastRoute = item.Route;
                    lastDestination = item.Destination;
                    typedSender.MainStackPanel.Children.Add(new TextBlock() { Text = (await Data.GetRoute(lastRoute, typedSender.MasterCancellationTokenSource.Token)).Name + " to " + lastDestination, FontSize = 18, Foreground = new SolidColorBrush(lighten(accentColor)), TextWrapping = TextWrapping.WrapWholeWords });
                    timesControl = new ItemsControl();
                    typedSender.MainStackPanel.Children.Add(timesControl);
                }
                timesControl.Items.Add(new TextBlock() { Text = item.ScheduledArrivalTime.ToString("h:mm"), HorizontalAlignment = HorizontalAlignment.Center, FontWeight = item.ScheduledArrivalTime.Hour >= 12 ? Windows.UI.Text.FontWeights.Bold : Windows.UI.Text.FontWeights.Normal });
            }
        }

        public DaySchedule Schedule
        {
            get { return (DaySchedule)GetValue(ScheduleProperty); }
            set { SetValue(ScheduleProperty, value); }
        }
    }
}
