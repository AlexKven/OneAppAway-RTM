using System;
using System.Collections.Generic;
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
using OneAppAway._1_1.Helpers;
using OneAppAway.Common;
using OneAppAway._1_1.Data;
using OneAppAway._1_1.ViewModels;
using System.Threading;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OneAppAway._1_1.Views.Controls
{
    public sealed partial class StopPopupControl : StopPopupControlBase
    {
        private StopPopupViewModel VM = new StopPopupViewModel();

        private bool ScheduleSet = false;

        public StopPopupControl()
        {
            this.InitializeComponent();
            MainGrid.DataContext = VM;
            VM.PropertyChanged += VM_PropertyChanged;
            RefreshMainGridWidth();
            //ExpandButton.SetBinding(Button.CommandProperty, new Binding() { Source = this, Path = new PropertyPath("ExpandCommand") });
            //CompressButton.SetBinding(Button.CommandProperty, new Binding() { Source = this, Path = new PropertyPath("CompressCommand") });
            //CloseButton.SetBinding(Button.CommandProperty, new Binding() { Source = this, Path = new PropertyPath("CloseCommand") });
            //RegisterPropertyChangedCallback(WidthProperty, WidthChanged);
        }

        private bool _IsTopLevel = true;
        public bool IsTopLevel
        {
            get { return _IsTopLevel; }
            set
            {
                _IsTopLevel = value;
                RefreshMainGridWidth();
            }
        }

        private void VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "HasChildren")
                RefreshMainGridWidth();
            if (VM.ShowSchedule && !ScheduleSet)
                SetSchedule();
        }

        private void RefreshMainGridWidth()
        {
            if (IsTopLevel)
                return;
            else if (!VM.HasChildren)
                MainGrid.Width = 290;
            else
                MainGrid.Width = double.NaN;
        }

        //protected override Size MeasureOverride(Size availableSize)
        //{
        //    var size = availableSize;
        //    if (double.IsInfinity(size.Width))
        //        size.Width = 290;
        //    return base.MeasureOverride(size);
        //}

        public Visibility TitleVisibility
        {
            get { return (Visibility)GetValue(TitleVisibilityProperty); }
            set { SetValue(TitleVisibilityProperty, value); }
        }
        public static readonly DependencyProperty TitleVisibilityProperty =
            DependencyProperty.Register("TitleVisibility", typeof(Visibility), typeof(StopPopupControl), new PropertyMetadata(Visibility.Visible));
        
        private void StopArrivalsControlBase_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {

            //if ((DataContext as ViewModels.StopArrivalsControlViewModel)?.IsTopLevel ?? false)
            //{
            //    ExpandButton.Visibility = Visibility.Visible;
            //    CompressButton.Visibility = Visibility.Visible;
            //    CloseButton.Visibility = Visibility.Visible;


            //    //ExpandButton.SetBinding(Button.VisibilityProperty, new Binding() { Source = this, Path = new PropertyPath("IsExpandEnabled"), Converter = Converters.BoolToVisibilityConverter.Instance });
            //    //CompressButton.SetBinding(Button.VisibilityProperty, new Binding() { Source = this, Path = new PropertyPath("IsCompressEnabled"), Converter = Converters.BoolToVisibilityConverter.Instance });
            //}
            //else
            //{
            //    ExpandButton.Visibility = Visibility.Collapsed;
            //    CompressButton.Visibility = Visibility.Collapsed;
            //    CloseButton.Visibility = Visibility.Collapsed;
            //}
            //TitleButton.CommandParameter = (DataContext as ViewModels.StopPopupViewModel)?.Stop.ID;
            //ArrivalsBox.Stop = (DataContext as ViewModels.StopPopupViewModel)?.Stop ?? new Data.TransitStop();
        }

        protected override void OnStopChanged(TransitStop stop)
        {
            VM.Stop = stop;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ArrivalsBox.Refresh(true);
        }

        private async void SetSchedule()
        {
            ScheduleSet = true;
            var sch = await DataSource.GetScheduleForStopAsync(VM.Stop.ID, DataSourcePreference.All, CancellationToken.None);
            if (sch.HasData)
            {
                var typed = sch.Data as OneAppAway.WeekSchedule;
                FindName("ScheduleViewer");
                ScheduleViewer.Schedule = typed[ServiceDay.Monday];
            }
        }
    }
}
