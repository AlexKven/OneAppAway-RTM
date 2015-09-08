using System;
using System.Collections.Generic;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace OneAppAway
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        public AboutPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            SetState(AppWidthStateGroup.CurrentState?.Name);
        }

        private void AppWidthStateGroup_CurrentStateChanging(object sender, VisualStateChangedEventArgs e)
        {
            SetState(e.NewState?.Name);
        }

        private void SetState(string stateName)
        {
            if (stateName == "ThinState")
            {
                if (LeftScrollViewer.Content != null)
                    LeftScrollViewer.Content = null;
                if (RightScrollViewer.Content != null)
                    RightScrollViewer.Content = null;
                InnerPanel.Children.Add(LeftPanel);
                InnerPanel.Children.Add(RightPanel);
            }
            else
            {
                InnerPanel.Children.Clear();
                LeftScrollViewer.Content = LeftPanel;
                RightScrollViewer.Content = RightPanel;
            }
        }
    }
}
