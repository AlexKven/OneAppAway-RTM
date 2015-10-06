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
    public sealed partial class DataGrid : UserControl
    {
        public DataGrid()
        {
            this.InitializeComponent();
            Rows.CollectionChanged += Routes_CollectionChanged;
            Columns.CollectionChanged += Stops_CollectionChanged;

            this.Columns.Add("35th Ave SW and SW 332nd Pl");
            this.Columns.Add("21st Ave SW and SW 320th St");
            this.Columns.Add("1 Ave S and SW 332nd Pl");
            this.Columns.Add("Fed Way S 320th St P&R");
            this.Columns.Add("5th Ave and Seneca St");
            this.Columns.Add("4th Ave and University St");
            this.Columns.Add("2nd Ave Ext S and S Jackson St");
            this.Columns.Add("Federal Way Transit Center");
            this.Rows.Add("187 to Federal Way TC");
            this.Rows.Add("176 to Downtown Seattle via Fed Way TC");
            this.Rows.Add("903 to Federal Way TC");
            this.Rows.Add("181 to Green River College via Auburn Station");
            this.Rows.Add("179 to Downtown Seattle via Fed Way TC");
            this.Rows.Add("197 to University District via Fed Way TC");
        }

        private double CellWidth = 60;
        private double CellHeight = 30;
        private double CellMargin = 1;
        private List<Rectangle> RowRects = new List<Rectangle>();

        #region Properties
        private ObservableCollection<string> _Columns = new ObservableCollection<string>();
        private ObservableCollection<string> _Rows = new ObservableCollection<string>();

        public ObservableCollection<string> Columns
        {
            get { return _Columns; }
        }

        public ObservableCollection<string> Rows
        {
            get { return _Rows; }
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
            for (int x = 0; x < Columns.Count + 1; x++)
            {
                for (int y = 0; y < Rows.Count + 1; y++)
                {
                    Rectangle rect = new Rectangle() { Fill = ((y) / 2 == (double)(y) / 2.0) ? new SolidColorBrush(Color.FromArgb(255, 56, 56, 56)) : new SolidColorBrush(Color.FromArgb(255, 64, 64, 64)), Margin = new Thickness(0,0,1,0) };
                    Grid.SetColumn(rect, x);
                    Grid.SetRow(rect, y);
                    MainGrid.Children.Add(rect);
                    if (x == 0)
                    {
                        if (y > 0)
                        {
                            string rowTitle = Rows[y - 1];
                            MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(CellHeight) });
                            bool sizeFound = false;
                            double candidateWidth = MainGrid.ColumnDefinitions[0].Width.Value;
                            double candidateHeight;
                            while (!sizeFound)
                            {
                                block = new TextBlock() { Text = rowTitle, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, TextWrapping = TextWrapping.WrapWholeWords, FontSize = 8, Margin = new Thickness(CellMargin) };
                                block.Measure(new Size(candidateWidth - CellMargin * 2, CellHeight * 2));
                                candidateHeight = block.ActualHeight;
                                block.Width = CellWidth;
                                if (candidateHeight <= CellHeight - CellMargin * 2)
                                    sizeFound = true;
                                else
                                    candidateWidth += 5;
                            }
                            block = new TextBlock() { Text = rowTitle, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, TextWrapping = TextWrapping.WrapWholeWords, FontSize = 8, Margin = new Thickness(CellMargin) };
                            MainGrid.ColumnDefinitions[0].Width = new GridLength(candidateWidth);
                            Grid.SetRow(block, y);
                            MainGrid.Children.Add(block);
                        }
                    }
                }
                if (x > 0)
                {
                    string columnTitle = Columns[x - 1];
                    MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(CellWidth) });
                    block = new TextBlock() { Text = columnTitle, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, TextWrapping = TextWrapping.WrapWholeWords, FontSize = 8, Margin = new Thickness(CellMargin) };
                    Grid.SetColumn(block, x);
                    MainGrid.Children.Add(block);
                }
            }
        }
    }
}
