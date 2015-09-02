using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
            if (_Caption == null)
                CaptionBox.Text = ArrivalsBox.GetStops().Length.ToString() + (ArrivalsBox.GetStops().Length == 1 ? " Stop" : " Stops");
        }

        private string _Caption = null;
        public string Caption
        {
            get { return _Caption; }
            set
            {
                _Caption = value;
                if (_Caption == null)
                    CaptionBox.Text = ArrivalsBox.GetStops().Length.ToString() + (ArrivalsBox.GetStops().Length == 1 ? " Stop" : " Stops");
                else
                    CaptionBox.Text = _Caption;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (CloseRequested != null) CloseRequested(this, new EventArgs());
        }

        public async Task ShowHelpTip()
        {
            await ArrivalsBox.ShowHelpTip();
        }

        public event EventHandler CloseRequested;
    }
}
