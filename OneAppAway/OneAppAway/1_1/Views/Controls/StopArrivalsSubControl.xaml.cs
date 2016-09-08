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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OneAppAway._1_1.Views.Controls
{
    public sealed partial class StopArrivalsSubControl : StopArrivalsControlBase
    {
        public StopArrivalsSubControl()
        {
            this.InitializeComponent();
        }

        public bool IsExpandEnabled
        {
            get { return (bool)GetValue(IsExpandEnabledProperty); }
            set { SetValue(IsExpandEnabledProperty, value); }
        }
        public static readonly DependencyProperty IsExpandEnabledProperty =
            DependencyProperty.Register("IsExpandEnabled", typeof(bool), typeof(StopArrivalsControl), new PropertyMetadata(false));

        public bool IsCompressEnabled
        {
            get { return (bool)GetValue(IsCompressEnabledProperty); }
            set { SetValue(IsCompressEnabledProperty, value); }
        }
        public static readonly DependencyProperty IsCompressEnabledProperty =
            DependencyProperty.Register("IsCompressEnabled", typeof(bool), typeof(StopArrivalsControl), new PropertyMetadata(false));

        public ICommand ExpandCommand
        {
            get { return (ICommand)GetValue(ExpandCommandProperty); }
            set { SetValue(ExpandCommandProperty, value); }
        }
        public static readonly DependencyProperty ExpandCommandProperty =
            DependencyProperty.Register("ExpandCommand", typeof(ICommand), typeof(StopArrivalsControl), new PropertyMetadata(null));

        public ICommand CompressCommand
        {
            get { return (ICommand)GetValue(CompressCommandProperty); }
            set { SetValue(CompressCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CompressCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CompressCommandProperty =
            DependencyProperty.Register("CompressCommand", typeof(ICommand), typeof(StopArrivalsControl), new PropertyMetadata(null));

        public ICommand CloseCommand
        {
            get { return (ICommand)GetValue(CloseCommandProperty); }
            set { SetValue(CloseCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CloseCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(StopArrivalsControl), new PropertyMetadata(null));

        private void StopArrivalsControlBase_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if ((DataContext as ViewModels.StopArrivalsControlViewModel)?.IsTopLevel ?? false)
            {
                ExpandButton.Visibility = Visibility.Visible;
                CompressButton.Visibility = Visibility.Visible;
                CloseButton.Visibility = Visibility.Visible;
                

                //ExpandButton.SetBinding(Button.VisibilityProperty, new Binding() { Source = this, Path = new PropertyPath("IsExpandEnabled"), Converter = Converters.BoolToVisibilityConverter.Instance });
                //CompressButton.SetBinding(Button.VisibilityProperty, new Binding() { Source = this, Path = new PropertyPath("IsCompressEnabled"), Converter = Converters.BoolToVisibilityConverter.Instance });
                ExpandButton.SetBinding(Button.CommandProperty, new Binding() { Source = this, Path = new PropertyPath("ExpandCommand") });
                CompressButton.SetBinding(Button.CommandProperty, new Binding() { Source = this, Path = new PropertyPath("CompressCommand") });
                CloseButton.SetBinding(Button.CommandProperty, new Binding() { Source = this, Path = new PropertyPath("CloseCommand") });
            }
            else
            {
                ExpandButton.Visibility = Visibility.Collapsed;
                CompressButton.Visibility = Visibility.Collapsed;
                CloseButton.Visibility = Visibility.Collapsed;
            }
            RefreshButton.Command = new Command((obj) => ArrivalsBox.Refresh());
            ArrivalsBox.Stop = (DataContext as ViewModels.StopArrivalsControlViewModel)?.Stop ?? new Data.TransitStop();
        }
    }
}
