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
using System.Windows.Input;
using OneAppAway.Common;
using OneAppAway._1_1.AddIns;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace OneAppAway._1_1.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TransitMapPage : ApplicationPage
    {
        private TransitMapPageUwpViewModel VM;
        private ShownStopsAddIn StopsAddIn = new ShownStopsAddIn();
        private StopDetailsPopupAddIn StopDetailsAddIn = new StopDetailsPopupAddIn();

        public TransitMapPage()
        {
            this.InitializeComponent();
            ChangeViewCommand = new WeakRelayCommand(ChangeViewCommand_Execute);
            NavigateToStopPageCommand = new WeakRelayCommand(NavigateToStopPageCommand_Execute);
            MainMap.AddIns.Add(StopsAddIn);
            MainMap.AddIns.Add(StopDetailsAddIn);

            VM = new TransitMapPageUwpViewModel(Cache) { Title = "Transit Map" };
            DataContext = VM;
            //DatabaseManager.Initialize(null);
            //TransitStop.SqlProvider.CreateTable(DatabaseManager.MemoryDatabase, true);
            //MainMap.StopsSource = Stops;
            StopsAddIn.StopsSource = VM.ShownStops;
            StopDetailsAddIn.SelectedStopsSource = VM.SelectedStops;
            VM.BindToDependencyObject(StopsAddIn, ShownStopsAddIn.StopsClickedCommandProperty, "StopsClickedCommand");
            VM.BindToDependencyObject(StopsAddIn, ShownStopsAddIn.StopSizeProperty, "ShownStopSize");
            VM.BindToControl(MainMap, TransitMap.AreaDelayProperty, "Area", true);
            VM.BindToControl(MainMap, TransitMap.ZoomLevelDelayProperty, "ZoomLevel", true);
            VM.BindToDependencyObject(StopDetailsAddIn, StopDetailsPopupAddIn.HasSelectedStopsProperty, "HasSelectedStops", true);
            VM.BindToControl(this, CanGoBackProperty, "CanGoBack");
            VM.ChangeViewBackCommand = ChangeViewCommand;
            VM.NavigateToStopPageBackCommand = NavigateToStopPageCommand;
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

        #region Properties
        public ICommand ChangeViewCommand { get; }
        public ICommand NavigateToStopPageCommand { get; }
        #endregion

        #region Overrides
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //
            if (VM.NavigatedToCommand?.CanExecute(e.Parameter) ?? false)
                VM.NavigatedToCommand.Execute(e.Parameter);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }

        public override void GoBack()
        {
            VM.GoBack();
        }
        #endregion

        #region Event Handlers
        private async void ChangeViewCommand_Execute(object parameter)
        {
            await MainMap.TrySetView((MapView)parameter);
        }

        private void NavigateToStopPageCommand_Execute(object parameter)
        {
            string stop = parameter as string;
            if (stop != null)
            {
                Frame.Navigate(typeof(TransitStopPage), stop);
            }
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
