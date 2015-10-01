using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OneAppAway
{
    public sealed partial class TripGrid : UserControl
    {
        public TripGrid()
        {
            this.InitializeComponent();
            Routes.CollectionChanged += Routes_CollectionChanged;
            Stops.CollectionChanged += Stops_CollectionChanged;
        }

        private double CellWidth = 60;
        private double CellHeight = 30;
        private double CellMargin = 1;

        #region Properties
        private ObservableCollection<string> _Stops = new ObservableCollection<string>();
        private ObservableCollection<string> _Routes = new ObservableCollection<string>();

        public ObservableCollection<string> Stops
        {
            get { return _Stops; }
        }

        public ObservableCollection<string> Routes
        {
            get { return _Routes; }
        }
        #endregion

        #region Event Handlers
        private void Stops_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            temp();
        }

        private void Routes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            temp();
        }
        #endregion

        private void temp()
        {
            MainGrid.Children.Clear();
            MainGrid.ColumnDefinitions.Clear();
            MainGrid.RowDefinitions.Clear();
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(CellWidth) });
            MainGrid.RowDefinitions.Add(new RowDefinition());
            TextBlock block = null;
            for (int x = 0; x < Stops.Count + 1; x++)
            {
                for (int y = 0; y < Routes.Count + 1; y++)
                {
                    Rectangle rect = new Rectangle() { Fill = ((x + y) / 2 == (double)(x + y) / 2.0) ? new SolidColorBrush(Color.FromArgb(255, 48, 48, 48)) : new SolidColorBrush(Color.FromArgb(255, 64, 64, 64)) };
                    Grid.SetColumn(rect, x);
                    Grid.SetRow(rect, y);
                    MainGrid.Children.Add(rect);
                    if (x == 0)
                    {
                        if (y > 0)
                        {
                            string route = Routes[y - 1];
                            MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(CellHeight) });
                            bool sizeFound = false;
                            double candidateWidth = MainGrid.ColumnDefinitions[0].Width.Value;
                            double candidateHeight;
                            while (!sizeFound)
                            {
                                block = new TextBlock() { Text = route, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, TextWrapping = TextWrapping.WrapWholeWords, FontSize = 8, Margin = new Thickness(CellMargin) };
                                block.Measure(new Size(candidateWidth - CellMargin * 2, CellHeight * 2));
                                candidateHeight = block.ActualHeight;
                                block.Width = CellWidth;
                                if (candidateHeight <= CellHeight - CellMargin * 2)
                                    sizeFound = true;
                                else
                                    candidateWidth += 5;
                            }
                            block = new TextBlock() { Text = route, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, TextWrapping = TextWrapping.WrapWholeWords, FontSize = 8, Margin = new Thickness(CellMargin) };
                            MainGrid.ColumnDefinitions[0].Width = new GridLength(candidateWidth);
                            Grid.SetRow(block, y);
                            MainGrid.Children.Add(block);
                        }
                    }
                }
                if (x > 0)
                {
                    string stop = Stops[x - 1];
                    MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(CellWidth) });
                    block = new TextBlock() { Text = stop, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, TextWrapping = TextWrapping.WrapWholeWords, FontSize = 8, Margin = new Thickness(CellMargin) };
                    Grid.SetColumn(block, x);
                    MainGrid.Children.Add(block);
                }
            }
        }
    }
}
