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

        private void LoadInnerGrid()
        {
            if (SingleStopControl == null)
                SingleStopControl = (Grid)MainGrid.FindName("SingleStopControl");
        }

        private void LoadInnerScrollViewer()
        {
            if (scrollViewer == null)
            {
                scrollViewer = (ScrollViewer)MainGrid.FindName("scrollViewer");
                //ItemsPanel = (StackPanel)scrollViewer.FindName("ItemsPanel");
            }
        }

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
            //scrollViewer.HorizontalScrollBarVisibility = stops.Length == 1 ? ScrollBarVisibility.Hidden : ScrollBarVisibility.Auto;
            //scrollViewer.HorizontalScrollMode = stops.Length == 1 ? ScrollMode.Disabled : ScrollMode.Enabled;
            foreach (BusStop stop in stops)
            {
                StopArrivalsBox box = new StopArrivalsBox() { Stop = stop };
                if (SingleStopControl != null)
                    SingleStopControl.Children.Clear();
                if (ItemsPanel != null)
                    ItemsPanel.Children.Clear();
                if (stops.Length == 1)
                {
                    //Binding sizeBinding = new Binding() { Source = SingleStopControl, Path = new PropertyPath("ActualWidth"), Mode = BindingMode.OneWay };
                    //box.SetBinding(FrameworkElement.WidthProperty, sizeBinding);
                    LoadInnerGrid();
                    SingleStopControl.Children.Add(box);
                }
                else
                {
                    box.Width = 285;
                    LoadInnerScrollViewer();
                    ItemsPanel.Children.Add(box);
                }
            }
        }
    }
}
