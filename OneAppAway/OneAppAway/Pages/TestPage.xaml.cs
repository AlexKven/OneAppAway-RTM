using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace OneAppAway
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TestPage : NavigationFriendlyPage
    {
        public TestPage()
        {
            this.InitializeComponent();

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.FileTypeFilter.Add(".txt");
            var res = await openPicker.PickSingleFileAsync();
            var stream = await res.OpenAsync(Windows.Storage.FileAccessMode.Read);
            byte[] buffer = new byte[stream.Size];
            stream.AsStreamForRead().Read(buffer, 0, buffer.Length);
            string str = new string(buffer.Select(bte => (char)bte).ToArray());
            DaySchedule schedule = new DaySchedule();
            schedule.LoadFromVerboseString(str);
            MainScheduleBrowser.Schedule = schedule;
        }
    }
}
