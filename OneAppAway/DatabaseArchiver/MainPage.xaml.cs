using SQLitePCL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DatabaseArchiver
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //SQLiteConnection connection = new SQLiteConnection(Archiver.Platform, Archiver.DBPath);
        SQLiteConnection connection = Archiver.GetConnection();
        bool Archiving = false;
        private CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
        private const string SummaryQuery = @"select AgencyInfo.Agency, RouteInfo.Route, StopRouteLinkInfo.StopRouteLink, StopInfo.Stop, ShapeInfo.Shape from 
(select count(*) as 'Agency', 1 as 'id' from Agency union all select count(*) as 'Agency', 2 as 'id' from PendingAgency) AgencyInfo, 
(select count(*) as 'Route', 1 as 'id' from Route union all select count(*) as 'Route', 2 as 'id' from PendingRoute) RouteInfo, 
(select count(*) as 'StopRouteLink', 1 as 'id' from StopRouteLink union all select count(*) as 'StopRouteLink', 2 as 'id' from PendingStopRouteLink) StopRouteLinkInfo, 
(select count(*) as 'Stop', 1 as 'id' from Stop union all select count(*) as 'Stop', 2 as 'id' from PendingStop) StopInfo, 
(select count(*) as 'Shape', 1 as 'id' from Shape union all select 'N/A' as 'Shape', 2 as 'id') ShapeInfo
where AgencyInfo.id = RouteInfo.id and RouteInfo.id = StopInfo.id and StopInfo.id = StopRouteLinkInfo.id and StopRouteLinkInfo.ID = ShapeInfo.ID";

        public MainPage()
        {
            this.InitializeComponent();
            UpdateCounter();
        }

        public void RunQuery(string query)
        {
            DataGrid.RowDefinitions.Clear();
            DataGrid.ColumnDefinitions.Clear();
            DataGrid.Children.Clear();

            string[] columns;
            int numRows;
            var results = Archiver.ExecuteSQL(connection, query, out columns, out numRows);

            DataGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            for (int i = 0; i < columns.Length; i++)
            {
                DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                TextBlock propBlock = new TextBlock() { Text = columns[i], Margin = new Thickness(3, 2, 3, 3), MinWidth = 70 };
                Grid.SetColumn(propBlock, i);
                DataGrid.Children.Add(propBlock);
            }

            Rectangle divider = new Rectangle() { Fill = new SolidColorBrush(Colors.Black), Height = 1, VerticalAlignment = VerticalAlignment.Bottom, HorizontalAlignment = HorizontalAlignment.Stretch };
            Grid.SetColumnSpan(divider, columns.Length);
            DataGrid.Children.Add(divider);


            object item = null;

            for (int h = 0; h < numRows; h++)
            {
                DataGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                for (int i = 0; i < columns.Length; i++)
                {
                    item = results[i, h];
                    if (item != null)
                    {
                        TextBlock itemBlock = new TextBlock() { Text = item.ToString(), Margin = new Thickness(2, 0, 2, 0) };
                        Grid.SetRow(itemBlock, h + 1);
                        Grid.SetColumn(itemBlock, i);
                        DataGrid.Children.Add(itemBlock);
                    }
                }
            }
        }

        private async void GoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RunQuery(QueryBox.Text);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                QueryBox.BorderBrush = new SolidColorBrush(Colors.Red);
                await System.Threading.Tasks.Task.Delay(500);
                QueryBox.SetValue(TextBox.BorderBrushProperty, DependencyProperty.UnsetValue);
            }
        }

        private void UpdateCounter()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(QueryBox.Text))
                    RunQuery(SummaryQuery);
            }
            catch (Exception) { }
        }

        private async void ContinuallyUpdateCounter()
        {
            while (Archiving)
            {
                UpdateCounter();
                await Task.Delay(200);
            }
            UpdateCounter();
        }

        private async void ArchiveButton_Click(object sender, RoutedEventArgs e)
        {
            if (Archiving)
            {
                CancellationTokenSource.Cancel();
                CancellationTokenSource = new CancellationTokenSource();
            }
            else
            {
                ProgressIndicator.IsIndeterminate = true;
                Archiving = true;
                ContinuallyUpdateCounter();
                try
                {
                    await Archiver.Begin(connection, CancellationTokenSource.Token);
                }
                catch (OperationCanceledException) { }
                Archiving = false;
                ProgressIndicator.IsIndeterminate = false;
            }
        }

        private async void DeleteDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Archiving)
            {
                connection.Dispose();
                await (await StorageFile.GetFileFromPathAsync(Archiver.DBPath)).DeleteAsync();
                connection = Archiver.GetConnection();
                UpdateCounter();
            }

            
        }

        private async void DatabaseSizeButton_Click(object sender, RoutedEventArgs e)
        {
            double size = (double)(await (await StorageFile.GetFileFromPathAsync(Archiver.DBPath)).GetBasicPropertiesAsync()).Size;
            int level = 0;
            while (size >= 1024)
            {
                size /= 1024.0;
                level++;
            }
            string unit = (new string[] { "Bytes", "KB", "MB", "GB", "TB", "PB", "EB" })[level];
            DatabaseSizeButton.Content = size.ToString("F3") + ' ' + unit;
        }
    }
}
