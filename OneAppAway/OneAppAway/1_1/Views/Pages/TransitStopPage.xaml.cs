using OneAppAway._1_1.AddIns;
using OneAppAway._1_1.Data;
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
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace OneAppAway._1_1.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TransitStopPage : ApplicationPage
    {
        private ShownStopsAddIn StopsAddIn = new ShownStopsAddIn();

        public TransitStopPage()
        {
            this.InitializeComponent();
            MainMapControl.CenterRegion = new Data.RectSubset() { Left = 250, LeftValueType = Data.RectSubsetValueType.Length };
            MainMapControl.AddIns.Add(StopsAddIn);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var stopResult = await DataSource.GetTransitStopAsync(e.Parameter as string, DataSourcePreference.All, CancellationToken.None);
            if (stopResult.HasData)
            {
                StopsAddIn.StopsSource = stopResult.Data;
                MainMapControl.ZoomLevel = 15;
                MainMapControl.Center = stopResult.Data.Position;
                //await MainMapControl.TrySetView(new MapView(stopResult.Data.Position, 15));
                //MainMapControl.CenterRegion = new Data.RectSubset() { Right = 250, RightValueType = Data.RectSubsetValueType.Length, RightScale = RectSubsetScale.Absolute };
                //await MainMapControl.TrySetView(new MapView(stopResult.Data.Position, 15));
            }
        }
    }
}
