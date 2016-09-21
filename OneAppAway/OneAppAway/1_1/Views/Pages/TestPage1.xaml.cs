using MvvmHelpers;
using OneAppAway._1_1.Data;
using OneAppAway._1_1.Views.Structures;
using OneAppAway.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace OneAppAway._1_1.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TestPage1 : ApplicationPage
    {
        public TestPage1()
        {
            this.InitializeComponent();
            DataContext = new BaseViewModel() { Title = "Developer Page" };
            MainRealTimeArrivalControl.Arrival = new RealTimeArrival()
            {
                Status = AlertStatus.Cancelled,
                RouteName = "193 Express to First Hill, Seattle",
                PredictedArrivalTime = DateTime.Now + TimeSpan.FromMinutes(5),
                ScheduledArrivalTime = DateTime.Now + TimeSpan.FromMinutes(1),
                Destination = "First Hill"
            };
        }

        private static void TestAction(TestPage1 target, object obj)
        {
            target.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ShapeDesignerPage)); Window.Current.CoreWindow.GetKeyState(Windows.System.VirtualKey.Control);
        }
    }
}
