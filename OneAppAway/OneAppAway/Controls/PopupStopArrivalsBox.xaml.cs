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
    public sealed partial class PopupStopArrivalsBox : UserControl
    {
        public PopupStopArrivalsBox()
        {
            this.InitializeComponent();
        }

        public BusStop[] GetStops()
        {
            return ArrivalsBox.GetStops();
        }

        public void SetStops(params BusStop[] stops)
        {
            ArrivalsBox.SetStops(stops);
        }

        private string _Caption = null;
        public string Caption
        {
            get { return ArrivalsBox.Caption; }
            set { ArrivalsBox.Caption = value; }
        }

        private void OnCloseRequested()
        {
            if (CloseRequested != null) CloseRequested(this, new EventArgs());
        }

        public event EventHandler CloseRequested;

        private void ArrivalsBox_CloseRequested(object sender, EventArgs e)
        {
            OnCloseRequested();
        }
    }
}
