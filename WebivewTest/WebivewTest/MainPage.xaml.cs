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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WebivewTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Load();
        }

        private void Load()
        {
            MainWebView.Navigate(new Uri("ms-appx-web:///Assets/DefaultArrivalsView.html"));
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            RealTimeArrival rta = new RealTimeArrival();
            rta.Route = "12345";
            rta.PrevRoute = "12344";
            rta.RouteName = "123";
            rta.PrevRouteName = "122";
            rta.Trip = "987654321";
            rta.Stop = "12345";
            rta.ScheduledArrivalTime = DateTime.Now.AddMinutes(5);
            rta.PredictedArrivalTime = DateTime.Now.AddMinutes(7);
            rta.Vehicle = "12321";
            rta.Destination = "Prosperity";
            rta.DegreeOfConfidence = 0.7;
            rta.IsDropOffOnly = false;
            await MainWebView.InvokeScriptAsync("addArrival", GetParamsFromRTA(rta));
        }

        private string[] GetParamsFromRTA(RealTimeArrival rta)
        {
            return new string[]
            {
                rta.Trip,
                rta.Stop,
                rta.Route,
                rta.PrevRoute,
                rta.RouteName,
                rta.PrevRouteName,
                rta.ScheduledArrivalTime?.ToString(),
                rta.PredictedArrivalTime?.ToString(),
                rta.Vehicle,
                rta.Destination,
                rta.FrequencyMinutes?.ToString(),
                rta.ScheduledVehicleLocation?.ToString(),
                rta.KnownVehicleLocation?.ToString(),
                rta.Orientation?.ToString(),
                rta.DegreeOfConfidence.ToString(),
                rta.IsDropOffOnly.ToString()
            }.Select(e => e ?? "").ToArray();
        }

        private void MainWebView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            MainButton.Content = e.Value;
        }
    }
}
