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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OneAppAway._1_1.Views.Controls
{
    public sealed partial class StopPopupControl : StopPopupControlBase
    {
        public StopPopupControl()
        {
            this.InitializeComponent();
            //ExpandButton.SetBinding(Button.CommandProperty, new Binding() { Source = this, Path = new PropertyPath("ExpandCommand") });
            //CompressButton.SetBinding(Button.CommandProperty, new Binding() { Source = this, Path = new PropertyPath("CompressCommand") });
            //CloseButton.SetBinding(Button.CommandProperty, new Binding() { Source = this, Path = new PropertyPath("CloseCommand") });
            RefreshButton.Command = new RelayCommand((obj) => ArrivalsBox.Refresh(true));
        }
        
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
            TitleButton.CommandParameter = (DataContext as ViewModels.StopArrivalsControlViewModel)?.Stop.ID;
            ArrivalsBox.Stop = (DataContext as ViewModels.StopArrivalsControlViewModel)?.Stop ?? new Data.TransitStop();
        }
    }
}
