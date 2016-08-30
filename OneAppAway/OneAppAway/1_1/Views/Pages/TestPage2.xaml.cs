using MvvmHelpers;
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
    public sealed partial class TestPage2 : ApplicationPage
    {
        public TestPage2()
        {
            this.InitializeComponent();
            DataContext = new BaseViewModel() { Title = "Test Page 2" };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainStackPanel.Children.Add(new Button() { HorizontalAlignment = HorizontalAlignment.Center, Content = "Button!" });
            CanGoBack = true;
            RefreshTitle();
        }

        public override void GoBack()
        {
            if (MainStackPanel.Children.Count > 2)
            {
                MainStackPanel.Children.RemoveAt(MainStackPanel.Children.Count - 1);
                CanGoBack = MainStackPanel.Children.Count > 2;
            }
            RefreshTitle();
        }

        void RefreshTitle()
        {
            ((BaseViewModel)DataContext).Title = $"{MainStackPanel.Children.Count} buttons!";
        }
    }
}
