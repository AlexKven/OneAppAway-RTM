using OneAppAway._1_1.Data;
using OneAppAway._1_1.ViewModels;
using OneAppAway._1_1.Views.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OneAppAway._1_1.Views.Controls
{
    public sealed partial class StopArrivalsBox : UserControl
    {
        private StopArrivalsBoxViewModel VM
        {
            get { return DataContext as StopArrivalsBoxViewModel; }
            set { DataContext = value; }
        }

        public ObservableCollection<RealTimeArrival> ShownArrivals => VM.Items;

        public StopArrivalsBox()
        {
            this.InitializeComponent();
            VM = new StopArrivalsBoxViewModel();
        }

        public void Refresh(bool forceOnline)
        {
            VM?.Refresh(forceOnline);
        }

        public TransitStop Stop
        {
            get { return (TransitStop)GetValue(StopProperty); }
            set { SetValue(StopProperty, value); }
        }
        public static readonly DependencyProperty StopProperty =
            DependencyProperty.Register("Stop", typeof(TransitStop), typeof(StopArrivalsBox), new PropertyMetadata(new TransitStop(), OnStopsChangedStatic));
        static void OnStopsChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var autoDownloadMode = SettingsManagerBase.Instance.CurrentDownloadArrivalsMode;
            var stop = (TransitStop)e.NewValue;
            bool autoDownload = false;
            if (autoDownloadMode == ManuallyDownloadArrivalsMode.Never || (autoDownloadMode == ManuallyDownloadArrivalsMode.GroupsOnly && stop.Parent == null))
                autoDownload = true;
            ((StopArrivalsBox)sender).VM.AutoDownload = autoDownload;
            ((StopArrivalsBox)sender).VM.Stop = stop;
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0)
                MainScrollViewer.Width = e.NewSize.Width;
        }
        
        public ICommand NavigateToLocationCommand
        {
            get { return (ICommand)GetValue(NavigateToLocationCommandProperty); }
            set { SetValue(NavigateToLocationCommandProperty, value); }
        }
        public static readonly DependencyProperty NavigateToLocationCommandProperty =
            DependencyProperty.Register("NavigateToLocationCommand", typeof(ICommand), typeof(StopArrivalsBox), new PropertyMetadata(null));

        //private void IntermediateGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    if (e.NewSize.Width > 0)
        //        MainScrollViewer.Width = e.NewSize.Width;
        //    if (e.NewSize.Height > 0)
        //        MainScrollViewer.Height = e.NewSize.Height;
        //}
    }
}
