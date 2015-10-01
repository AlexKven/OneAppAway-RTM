using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace OneAppAway
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TestPage : NavigationFriendlyPage
    {
        public TestPage()
        {
            this.InitializeComponent();
            MainTripGrid.Stops.Add("35th Ave SW and SW 332nd Pl");
            MainTripGrid.Stops.Add("21st Ave SW and SW 320th St");
            MainTripGrid.Stops.Add("1 Ave S and SW 332nd Pl");
            MainTripGrid.Stops.Add("Fed Way S 320th St P&R");
            MainTripGrid.Stops.Add("5th Ave and Seneca St");
            MainTripGrid.Stops.Add("4th Ave and University St");
            MainTripGrid.Stops.Add("2nd Ave Ext S and S Jackson St");
            MainTripGrid.Stops.Add("Federal Way Transit Center");
            MainTripGrid.Routes.Add("187 to Federal Way TC");
            MainTripGrid.Routes.Add("176 to Downtown Seattle via Fed Way TC");
            MainTripGrid.Routes.Add("903 to Federal Way TC");
            MainTripGrid.Routes.Add("181 to Green River College via Auburn Station");
            MainTripGrid.Routes.Add("179 to Downtown Seattle via Fed Way TC");
            MainTripGrid.Routes.Add("197 to University District via Fed Way TC");
        }
    }
}
