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
using System.ComponentModel;
using System.Collections.Specialized;
using System.Threading.Tasks;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OneAppAway._1_1.Views.Controls
{
    public sealed partial class StopPopupControl : StopPopupControlBase
    {
        private StopPopupViewModel VM = new StopPopupViewModel();

        private bool ScheduleSet = false;
        private CompositeCollectionBinding<RealTimeArrival, string> ShownArrivalsBinding;

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
            ShownArrivalsBinding = new CompositeCollectionBinding<RealTimeArrival, string>(ShownArrivals);
            WeakEventListener<StopPopupControl, object, NotifyCollectionChangedEventArgs> childrenListener = new WeakEventListener<StopPopupControl, object, NotifyCollectionChangedEventArgs>(this);
            childrenListener.OnEventAction = (t, s, e) => t.Children_CollectionChanged(s, e);
            VM.Children.CollectionChanged += childrenListener.OnEvent;
        }

        private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ShownArrivalsBinding.ClearCollections();
            SubItemsControl.Children.Clear();
            foreach (var item in VM.Children)
            {
                StopPopupControl subControl = new StopPopupControl();
                subControl.Stop = item;
                subControl.IsTopLevel = false;
                subControl.VerticalAlignment = VerticalAlignment.Stretch;
                subControl.MinWidth = 190;
                subControl.SetBinding(ShowRoutesListProperty, new Binding() { Source = this, Path = new PropertyPath("ShowRoutesList") });
                subControl.SetBinding(TitleCommandProperty, new Binding() { Source = this, Path = new PropertyPath("TitleCommand") });
                subControl.SetBinding(ShowCompactMenuProperty, new Binding() { Source = this, Path = new PropertyPath("ShowCompactMenu") });
                ShownArrivalsBinding.AddCollection(item.ID, subControl.ShownArrivals);
                subControl.SetBinding(StopPopupControlBase.NavigateToLocationCommandProperty, new Binding() { Source = this, Path = new PropertyPath("NavigateToLocationCommand") });
                SubItemsControl.Children.Add(subControl);
            }
            if (VM.Children.Count == 0)
            {
                ShownArrivalsBinding.AddCollection("0", ArrivalsBox.ShownArrivals);
            }
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
            VM.ShowArrivals = true;
            VM.Stop = stop;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ArrivalsBox.Refresh(true);
        }

        private void SetSchedule()
        {
            FindName("ScheduleViewer");
            ScheduleSet = true;
        }

        private void IntermediateGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0)
                ManuallySizedGrid.Width = e.NewSize.Width;
            if (e.NewSize.Height > 0)
                ManuallySizedGrid.Height = e.NewSize.Height;
        }
    }
}
