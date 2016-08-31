﻿using MvvmHelpers;
using OneAppAway._1_1.Views.Structures;
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

namespace OneAppAway._1_1.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TestPage1 : ApplicationPage
    {
        public TestPage1()
        {
            this.InitializeComponent();
            DataContext = new BaseViewModel() { Title = "Test Page 1" };
            ArrivalControl.Arrival = new Data.RealTimeArrival() { RouteName = "193X", Destination = "Seattle University, First Hill", ScheduledArrivalTime = DateTime.Now - TimeSpan.FromMinutes(10), PredictedArrivalTime = DateTime.Now - TimeSpan.FromMinutes(5) };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(TestPage2));
        }
    }
}
