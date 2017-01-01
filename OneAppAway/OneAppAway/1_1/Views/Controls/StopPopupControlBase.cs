using OneAppAway._1_1.Data;
using OneAppAway._1_1.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OneAppAway._1_1.Views.Controls
{
    public abstract class StopPopupControlBase : UserControl
    {
        public StopPopupControlBase()
        {
        }
        
        public ICommand NavigateToLocationCommand
        {
            get { return (ICommand)GetValue(NavigateToLocationCommandProperty); }
            set { SetValue(NavigateToLocationCommandProperty, value); }
        }
        public static readonly DependencyProperty NavigateToLocationCommandProperty =
            DependencyProperty.Register("NavigateToLocationCommand", typeof(ICommand), typeof(StopPopupControlBase), new PropertyMetadata(null));

        public TransitStop Stop
        {
            get { return (TransitStop)GetValue(StopProperty); }
            set { SetValue(StopProperty, value); }
        }
        public static readonly DependencyProperty StopProperty =
            DependencyProperty.Register("Stop", typeof(TransitStop), typeof(StopPopupControlBase), new PropertyMetadata(new TransitStop(), OnStopChangedStatic));
        private static void OnStopChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as StopPopupControlBase)?.OnStopChanged((TransitStop)e.NewValue);
        }

        public ObservableCollection<RealTimeArrival> ShownArrivals { get; } = new ObservableCollection<RealTimeArrival>();

        protected virtual void OnStopChanged(TransitStop stop) { }

        public bool ShowRoutesList
        {
            get { return (bool)GetValue(ShowRoutesListProperty); }
            set { SetValue(ShowRoutesListProperty, value); }
        }
        public static readonly DependencyProperty ShowRoutesListProperty =
            DependencyProperty.Register("ShowRoutesList", typeof(bool), typeof(StopPopupControlBase), new PropertyMetadata(true));

        public bool ShowCompactMenu
        {
            get { return (bool)GetValue(ShowCompactMenuProperty); }
            set { SetValue(ShowCompactMenuProperty, value); }
        }
        public static readonly DependencyProperty ShowCompactMenuProperty =
            DependencyProperty.Register("ShowCompactMenu", typeof(bool), typeof(StopPopupControlBase), new PropertyMetadata(false));

        public Visibility TopControlsVisibility
        {
            get { return (Visibility)GetValue(TopControlsVisibilityProperty); }
            set { SetValue(TopControlsVisibilityProperty, value); }
        }
        public static readonly DependencyProperty TopControlsVisibilityProperty =
            DependencyProperty.Register("TopControlsVisibility", typeof(Visibility), typeof(StopPopupControlBase), new PropertyMetadata(Visibility.Collapsed));

        public ICommand ExpandCommand
        {
            get { return (ICommand)GetValue(ExpandCommandProperty); }
            set { SetValue(ExpandCommandProperty, value); }
        }
        public static readonly DependencyProperty ExpandCommandProperty =
            DependencyProperty.Register("ExpandCommand", typeof(ICommand), typeof(StopPopupControlBase), new PropertyMetadata(null));

        public ICommand CompressCommand
        {
            get { return (ICommand)GetValue(CompressCommandProperty); }
            set { SetValue(CompressCommandProperty, value); }
        }
        public static readonly DependencyProperty CompressCommandProperty =
            DependencyProperty.Register("CompressCommand", typeof(ICommand), typeof(StopPopupControlBase), new PropertyMetadata(null));

        public ICommand CloseCommand
        {
            get { return (ICommand)GetValue(CloseCommandProperty); }
            set { SetValue(CloseCommandProperty, value); }
        }
        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(StopPopupControlBase), new PropertyMetadata(null));
        
        public ICommand TitleCommand
        {
            get { return (ICommand)GetValue(TitleCommandProperty); }
            set { SetValue(TitleCommandProperty, value); }
        }
        public static readonly DependencyProperty TitleCommandProperty =
            DependencyProperty.Register("TitleCommand", typeof(ICommand), typeof(StopPopupControlBase), new PropertyMetadata(null));
    }
}
