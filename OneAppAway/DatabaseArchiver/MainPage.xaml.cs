using SQLitePCL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DatabaseArchiver
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //SQLiteConnection connection = new SQLiteConnection(Archiver.Platform, Archiver.DBPath);
        SQLiteConnection connection = new SQLiteConnection(Archiver.DBPath);

        public MainPage()
        {
            this.InitializeComponent();
            
        }

        private async void GoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataGrid.RowDefinitions.Clear();
                DataGrid.ColumnDefinitions.Clear();
                DataGrid.Children.Clear();

                var results = connection.Prepare(QueryBox.Text);

                List<int> shownProperties = new List<int>();

                DataGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                string[] columns = new string[results.ColumnCount];
                for (int i = 0; i < results.ColumnCount; i++)
                {
                    columns[i] = results.ColumnName(i);
                    DataGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                }

                Action<int> addRowFunc = rowNum =>
                {
                    TextBlock propBlock = new TextBlock() { Text = columns[rowNum], Margin = new Thickness(3, 2, 3, 3), MinWidth = 70 };
                    Grid.SetColumn(propBlock, rowNum);
                    DataGrid.Children.Add(propBlock);
                };

                Rectangle divider = new Rectangle() { Fill = new SolidColorBrush(Colors.Black), Height = 1, VerticalAlignment = VerticalAlignment.Bottom, HorizontalAlignment = HorizontalAlignment.Stretch };
                Grid.SetColumnSpan(divider, results.ColumnCount);
                DataGrid.Children.Add(divider);


                object item = null;

                int h = 0;
                do
                {
                    DataGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                    for (int i = 0; i < results.ColumnCount; i++)
                    {
                        item = results[i];
                        if (item != null)
                        {
                            if (!shownProperties.Contains(i))
                                addRowFunc(i);
                            TextBlock itemBlock = new TextBlock() { Text = item.ToString(), Margin = new Thickness(2, 0, 2, 0) };
                            Grid.SetRow(itemBlock, h + 1);
                            Grid.SetColumn(itemBlock, i);
                            DataGrid.Children.Add(itemBlock);
                        }
                    }
                    h++;
                } while (results.Step() != SQLiteResult.DONE);

                //for (int h = 0; h < results.Count; h++)
                //{
                //    for (int i = 0; i < properties.Length; i++)
                //    {
                //    }
                //}

                //string res = "";
                //foreach (var item in connection.Query<UniversalTableObject>(QueryBox.Text))
                //{
                //    res += item.ConvertToSpecificObject().ToString() + "\n";
                //}
                //ResponseBlock.Text = res;
                
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                QueryBox.BorderBrush = new SolidColorBrush(Colors.Red);
                await System.Threading.Tasks.Task.Delay(500);
                QueryBox.SetValue(TextBox.BorderBrushProperty, DependencyProperty.UnsetValue);
            }
        }

        private async void ArchiveButton_Click(object sender, RoutedEventArgs e)
        {
            //ProgressIndicator.IsIndeterminate = true;
            //await Archiver.Begin(connection);
            //ProgressIndicator.IsIndeterminate = false;
        }
    }
}
