using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OneAppAway._1_1.Views.Controls
{
    public class StopPopupControlBase : UserControl
    {
        public bool ShowRoutesList
        {
            get { return (bool)GetValue(ShowRoutesListProperty); }
            set { SetValue(ShowRoutesListProperty, value); }
        }
        public static readonly DependencyProperty ShowRoutesListProperty =
            DependencyProperty.Register("ShowRoutesList", typeof(bool), typeof(StopPopupControlBase), new PropertyMetadata(true));

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
