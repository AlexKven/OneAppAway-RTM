using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using OneAppAway._1_1.ViewModels;
using OneAppAway._1_1.Views.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace OneAppAway._1_1.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OuterFrame : Page
    {
        private OuterFrameViewModel VM;

        public OuterFrame(ApplicationFrame frame)
        {
            this.InitializeComponent();
            MainSplitView.Content = frame;
            VM = new OuterFrameViewModel((App)App.Current, frame);
            this.DataContext = VM;
        }

        #region Properties
        private double _SystemButtonsWidth;

        public double SystemButtonsWidth
        {
            get { return _SystemButtonsWidth; }
            set
            {
                _SystemButtonsWidth = value;
                RefreshTitleBarSizes();
            }
        }
        #endregion

        private void RefreshTitleBarSizes()
        {
            SystemButtonsColumn.Width = SystemButtonsColumn2.Width = new GridLength(SystemButtonsWidth);
        }

        #region Button Click/Check Events
        private void MapButton_Checked(object sender, RoutedEventArgs e)
        {
            //EnsureNavigation<BusMapPage>("CurrentLocation");
        }

        private void RoutesButton_Checked(object sender, RoutedEventArgs e)
        {
            //EnsureNavigation<RoutesPage>();
        }

        private void SettingsButton_Checked(object sender, RoutedEventArgs e)
        {
            //EnsureNavigation<SettingsPage>();
        }

        private void AboutButton_Checked(object sender, RoutedEventArgs e)
        {
            //EnsureNavigation<AboutPage>();
        }
        #endregion

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshTitleBarControls();
        }

        private void RefreshTitleBarControls()
        {
            NavigationFriendlyPage page;
            double width = this.ActualWidth - 50 - SystemButtonsWidth;
            //if ((page = MainFrame.Content as NavigationFriendlyPage) != null && width > 0)
            //{
            //    //page.OnRefreshTitleBarControls(this, width);
            //}
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SetTitleBar(TitleBar);
        }
    }
}
