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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OneAppAway
{
    public sealed partial class AdRow : UserControl
    {
        public AdRow()
        {
            this.InitializeComponent();
        }
        private bool _ShowAds = false;
        private double AdSpaceWidth = 0;
        private string AppKey = "bef2bb37-a5ad-49d7-9ba6-b1ccaf4be44b";
        private string[] AdUnitIds = new string[] { "168559", "168567", "168571" };

        public bool ShowAds
        {
            get { return _ShowAds; }
            set
            {
                _ShowAds = value;
                RefreshAds();
            }
        }

        private void RefreshAds()
        {
            if (AdSpaceWidth > 0 && ShowAds)
            {
                int numAds = (int)AdSpaceWidth / 320;
                if (numAds > 3) numAds = 3;
                while (AdPanel.Children.Count > numAds)
                {
                    AdPanel.Children.RemoveAt(AdPanel.Children.Count - 1);
                }
                while (AdPanel.Children.Count < numAds)
                {
                    AdDuplex.AdControl ad = new AdDuplex.AdControl() { AdUnitId = AdUnitIds[AdPanel.Children.Count], AppKey = this.AppKey };
                    AdPanel.Children.Add(ad);
                }
            }
            else
            {
                AdPanel.Children.Clear();
            }
        }

        private void AdPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0)
                AdSpaceWidth = e.NewSize.Width;
            RefreshAds();
        }
    }
}
