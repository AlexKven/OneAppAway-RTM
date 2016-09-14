using OneAppAway._1_1.Data;
using OneAppAway._1_1.ViewModels;
using OneAppAway._1_1.Views.Controls;
using OneAppAway._1_1.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.Composition;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.UI;
using System.Reflection.Emit;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace OneAppAway._1_1.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TransitMapPage : ApplicationPage
    {
        private TransitMapPageUwpViewModel VM;
        public TransitMapPage()
        {
            this.InitializeComponent();
            VM = new TransitMapPageUwpViewModel(Cache) { Title = "Transit Map" };
            DataContext = VM;
            //DatabaseManager.Initialize(null);
            //TransitStop.SqlProvider.CreateTable(DatabaseManager.MemoryDatabase, true);
            //MainMap.StopsSource = Stops;
            MainMap.StopsSource = VM.ShownStops;
            MainMap.SelectedStopsSource = VM.SelectedStops;
            MainMap.SmallThreshold = VM.SmallThreshold;
            MainMap.MediumThreshold = VM.MediumThreshold;
            MainMap.LargeThreshold = VM.LargeThreshold;
            VM.BindToControl(MainMap, TransitMap.StopsClickedCommandProperty, "StopsClickedCommand");
            VM.BindToControl(MainMap, TransitMap.AreaDelayProperty, "Area", true);
            VM.BindToControl(MainMap, TransitMap.ZoomLevelDelayProperty, "ZoomLevel", true);
            VM.BindToControl(MainMap, TransitMap.HasSelectedStopsProperty, "HasSelectedStops", true);
            VM.BindToControl(this, CanGoBackProperty, "CanGoBack");
            //SetBinding(CanGoBackProperty, new Binding() { Source = MainMap, Path = new PropertyPath("CanGoBack"), Mode = BindingMode.OneWay });
        }

        #region Methods
        private void LoadFromDatabase()
        {
            //var stops = DatabaseRetriever.RetrieveStops();
            //foreach (var stop in stops)
            //    if (!Stops.Any(stp => stp.ID == stop.ID))
            //        Stops.Add(stop);
        }

        #endregion

        #region Overrides
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            VM.ViewChangeRequested += VM_ViewChangeRequested;
            if (VM.NavigatedToCommand?.CanExecute(e.Parameter) ?? false)
                VM.NavigatedToCommand.Execute(e.Parameter);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            VM.ViewChangeRequested -= VM_ViewChangeRequested;
        }

        public override void GoBack()
        {
            VM.GoBack();
        }
        #endregion

        #region Event Handlers
        private async void VM_ViewChangeRequested(object sender, EventArgs<MapView> e)
        {
            await MainMap.TrySetView(e.Parameter);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var fwtc = new string[] { "1_80439", "1_80431", "1_80438", "1_80432", "1_80437", "1_80433", "3_27814", "3_29410" };
            var stops = await _1_1.Data.ApiLayer.GetTransitStopsForArea(MainMap.Area, new System.Threading.CancellationToken());
            var fwtcStop = new TransitStop() { ID = "FWTC", Position = new LatLon(47.31753, -122.30486), Path = "_vx_HlnniVF??LJ??MF??nIG??MK??LG?", Name = "Federal Way Transit Center" };
            TransitStop.SqlProvider.Insert(fwtcStop, DatabaseManager.MemoryDatabase);
            foreach (var stop in stops)
            {
                var finalStop = stop;
                if (fwtc.Contains(finalStop.ID))
                    finalStop.Parent = "FWTC";
                TransitStop.SqlProvider.Insert(finalStop, DatabaseManager.MemoryDatabase);
            }
            LoadFromDatabase();
            //MainGrid.Children.Add(new Controls.StopArrivalsControl() { DataContext = new StopArrivalsViewModel(new TransitStop() { Children = new string[] { "FWTC" }, Name = "Selected Stops" }), Margin = new Thickness(50) });
        }

        private void CurrentLocationButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            ((Grid)sender).DataContext = VM;
            var textbox = ((Grid)sender).Children.FirstOrDefault(child => child is EnterCommandTextBox) as EnterCommandTextBox;
            var flyout = (Flyout)this.Resources["SearchFlyout"];
            FlyoutHelpers.SetParent(flyout, textbox);
            ((Grid)sender).Loaded -= Grid_Loaded;
        }
    }
}
