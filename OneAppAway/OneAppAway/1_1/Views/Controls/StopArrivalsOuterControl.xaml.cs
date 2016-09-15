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
    public sealed partial class StopArrivalsOuterControl : StopArrivalsControlBase
    {
        public StopArrivalsOuterControl()
        {
            this.InitializeComponent();
            this.TopControlsVisibility = Visibility.Visible;
        }

        //#region Enabled Properties
        //public bool IsExpandEnabled
        //{
        //    get { return (bool)GetValue(IsExpandEnabledProperty); }
        //    set { SetValue(IsExpandEnabledProperty, value); }
        //}
        //public static readonly DependencyProperty IsExpandEnabledProperty =
        //    DependencyProperty.Register("IsExpandEnabled", typeof(bool), typeof(StopArrivalsOuterControl), new PropertyMetadata(false));

        //public bool IsCompressEnabled
        //{
        //    get { return (bool)GetValue(IsCompressEnabledProperty); }
        //    set { SetValue(IsCompressEnabledProperty, value); }
        //}
        //public static readonly DependencyProperty IsCompressEnabledProperty =
        //    DependencyProperty.Register("IsCompressEnabled", typeof(bool), typeof(StopArrivalsOuterControl), new PropertyMetadata(false));
        //#endregion

        #region Visibility Properties
        public bool ShowBottomArrow
        {
            get { return (bool)GetValue(ShowBottomArrowProperty); }
            set { SetValue(ShowBottomArrowProperty, value); }
        }
        public static readonly DependencyProperty ShowBottomArrowProperty =
            DependencyProperty.Register("ShowBottomArrow", typeof(bool), typeof(StopArrivalsOuterControl), new PropertyMetadata(true));
        #endregion

        //#region Command Properties
        //public ICommand ExpandCommand
        //{
        //    get { return (ICommand)GetValue(ExpandCommandProperty); }
        //    set { SetValue(ExpandCommandProperty, value); }
        //}
        //public static readonly DependencyProperty ExpandCommandProperty =
        //    DependencyProperty.Register("ExpandCommand", typeof(ICommand), typeof(StopArrivalsOuterControl), new PropertyMetadata(null));

        //public ICommand CompressCommand
        //{
        //    get { return (ICommand)GetValue(CompressCommandProperty); }
        //    set { SetValue(CompressCommandProperty, value); }
        //}
        //public static readonly DependencyProperty CompressCommandProperty =
        //    DependencyProperty.Register("CompressCommand", typeof(ICommand), typeof(StopArrivalsOuterControl), new PropertyMetadata(null));

        //public ICommand CloseCommand
        //{
        //    get { return (ICommand)GetValue(CloseCommandProperty); }
        //    set { SetValue(CloseCommandProperty, value); }
        //}
        //public static readonly DependencyProperty CloseCommandProperty =
        //    DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(StopArrivalsOuterControl), new PropertyMetadata(null));
        //#endregion

        #region Event Handlers
        private void StopArrivalsControlBase_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!double.IsNaN(e.NewSize.Width) && e.NewSize.Width > 0)
                SubControl.Width = e.NewSize.Width;
        }
        #endregion
    }
}
