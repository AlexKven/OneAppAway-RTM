﻿using OneAppAway._1_1.Data;
using OneAppAway._1_1.ViewModels;
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

namespace OneAppAway._1_1.Views.Controls
{
    public sealed partial class StopArrivalsBox : UserControl
    {
        private StopArrivalsBoxViewModel VM
        {
            get { return DataContext as StopArrivalsBoxViewModel; }
            set { DataContext = value; }
        }

        public StopArrivalsBox()
        {
            this.InitializeComponent();
            VM = new StopArrivalsBoxViewModel();
        }

        public void Refresh()
        {
            VM?.Refresh();
        }
        
        public TransitStop Stop
        {
            get { return (TransitStop)GetValue(StopProperty); }
            set { SetValue(StopProperty, value); }
        }
        public static readonly DependencyProperty StopProperty =
            DependencyProperty.Register("Stop", typeof(TransitStop), typeof(StopArrivalsBox), new PropertyMetadata(new TransitStop(), (s, e) => ((StopArrivalsBox)s).VM.Stop = (TransitStop)e.NewValue));

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 0)
                MainScrollViewer.Width = e.NewSize.Width;
            if (e.NewSize.Height > 0)
                MainScrollViewer.Height = e.NewSize.Height;
        }
    }
}
