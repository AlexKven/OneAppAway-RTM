using OneAppAway._1_1.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace OneAppAway._1_1.ViewModels
{
    class StopViewModel_BusMap : DependencyObject
    {
        BusStop Model;
        public StopViewModel_BusMap(BusStop model)
        {
            Model = model;
        }

        public MapStopSize StopSize
        {
            get { return (MapStopSize)GetValue(StopSizeProperty); }
            set { SetValue(StopSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StopSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StopSizeProperty =
            DependencyProperty.Register("StopSize", typeof(MapStopSize), typeof(StopViewModel_BusMap), new PropertyMetadata(MapStopSize.Medium, OnStopSizeChangedStatic));
        static void OnStopSizeChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as StopViewModel_BusMap)?.SetIcon();
        }

        private void SetIcon()
        {
            string size = StopSize == MapStopSize.Small ? "20" : "40";
            System.Diagnostics.Debug.WriteLine(size);
            IconSource = Model.Direction == StopDirection.Unspecified ? "ms-appx:///Assets/Icons/BusBase" + size + ".png" : "ms-appx:///Assets/Icons/BusDirection" + Model.Direction.ToString() + size + ".png";
        }

        public object IconSource
        {
            get { return (object)GetValue(IconSourceProperty); }
            set { SetValue(IconSourceProperty, value); }
        }
        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register("IconSource", typeof(object), typeof(StopViewModel_BusMap), new PropertyMetadata(null));
    }
}
