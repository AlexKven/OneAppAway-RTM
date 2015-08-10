using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class MultiStopArrivalsBox : UserControl
    {
        private BusStop[] Stops = new BusStop[0];

        public MultiStopArrivalsBox()
        {
            this.InitializeComponent();
        }

        public BusStop[] GetStops()
        {
            return Stops.ToArray();
        }

        public void SetStops(params BusStop[] stops)
        {
            Stops = stops.ToArray();
            ItemsPanel.Children.Clear();
            SingleStopControl.Children.Clear();
            scrollViewer.HorizontalScrollBarVisibility = stops.Length == 1 ? ScrollBarVisibility.Hidden : ScrollBarVisibility.Auto;
            scrollViewer.HorizontalScrollMode = stops.Length == 1 ? ScrollMode.Disabled : ScrollMode.Enabled;
            foreach (BusStop stop in stops)
            {
                StopArrivalsBox box = new StopArrivalsBox() { Stop = stop };
                if (stops.Length == 1)
                {
                    //Binding sizeBinding = new Binding() { Source = SingleStopControl, Path = new PropertyPath("ActualWidth"), Mode = BindingMode.OneWay };
                    //box.SetBinding(FrameworkElement.WidthProperty, sizeBinding);
                    SingleStopControl.Children.Add(box);
                }
                else
                {
                    box.Width = 285;
                    ItemsPanel.Children.Add(box);
                }
            }
            if (_Caption == null)
                CaptionBox.Text = Stops.Length.ToString() + (Stops.Length == 1 ? " Stop" : " Stops");
        }

        private string _Caption = null;
        public string Caption
        {
            get { return _Caption; }
            set
            {
                _Caption = value;
                if (_Caption == null)
                    CaptionBox.Text = Stops.Length.ToString() + (Stops.Length == 1 ? " Stop" : " Stops");
                else
                    CaptionBox.Text = _Caption;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (CloseRequested != null) CloseRequested(this, new EventArgs());
        }

        public event EventHandler CloseRequested;
    }
}
