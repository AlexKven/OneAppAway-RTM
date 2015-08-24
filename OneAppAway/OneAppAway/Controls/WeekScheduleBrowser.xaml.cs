using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
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
    public sealed partial class WeekScheduleBrowser : UserControl
    {
        private WeekSchedule Schedule;
        private CancellationTokenSource MasterCancellationTokenSource = new CancellationTokenSource();

        public WeekScheduleBrowser()
        {
            this.InitializeComponent();
        }

        public BusStop Stop { get; set; }

        public async Task LoadSchedule(bool forceOnline)
        {
            bool technicalMode = SettingsManager.GetSetting("TechnicalMode", false, false);
            ScheduleProgressIndicator.IsActive = true;
            ScheduleNotAvailableBlock.Visibility = Visibility.Collapsed;
            LoadSchedulesButton.Visibility = Visibility.Collapsed;
            bool checkOnline = forceOnline || BandwidthManager.EffectiveBandwidthOptions != BandwidthOptions.Low || !SettingsManager.GetSetting("LimitedData.DelayDownloadingSchedules", false, true);
            bool allowFallback = checkOnline || (!SettingsManager.GetSetting("LimitedData.DelayDownloadingSchedules", false, true) && BandwidthManager.EffectiveBandwidthOptions == BandwidthOptions.Low);
            var ScheduleResult = await Data.GetScheduleForStop(Stop.ID, new DataRetrievalOptions(checkOnline ? DataSourceDescriptor.Cloud : DataSourceDescriptor.Local, allowFallback), MasterCancellationTokenSource.Token);
            if (ScheduleResult.Item2.FinalSource == null)
            {
                if (checkOnline)
                {
                    CannotConnectButton.Visibility = Visibility.Visible;
                }
                else
                {
                    LoadSchedulesButton.Visibility = Visibility.Visible;
                }
            }
            else
            {
                CachedSchedulesButton.Visibility = checkOnline ? Visibility.Collapsed : Visibility.Visible;
                CannotConnectButton.Visibility = ((checkOnline && !ScheduleResult.Item2.AttemptSucceeded) || (!checkOnline && ScheduleResult.Item2.FallbackAttempted && !ScheduleResult.Item2.FallbackSucceeded)) ? Visibility.Visible : Visibility.Collapsed;
                Schedule = ScheduleResult.Item1;
                DayScheduleSelector.IsEnabled = false;
                DayScheduleSelector.Items.Clear();
                DayScheduleSelector.SelectedIndex = -1;
                foreach (var day in technicalMode ? Schedule.TechnicalDayGroups : Schedule.DayGroups)
                {
                    DayScheduleSelector.Items.Add(new ComboBoxItem() { Content = day.GetFriendlyName(), Tag = day });
                }
                if (DayScheduleSelector.Items.Count == 0)
                {
                    DayScheduleSelector.Items.Add("No Schedules Available");
                    ScheduleNotAvailableBlock.Visibility = Visibility.Visible;
                    DayScheduleSelector.SelectedIndex = 0;
                }
                else
                {
                    DayScheduleSelector.IsEnabled = true;
                    DayScheduleSelector.SelectedIndex = 0;
                }
            }
            ScheduleProgressIndicator.IsActive = false;
        }

        private void DayScheduleSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DayScheduleSelector.IsEnabled)
                MainScheduleBrowser.Schedule = Schedule[(ServiceDay)((ComboBoxItem)DayScheduleSelector.SelectedItem).Tag];
        }

        private async void LoadSchedulesButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadSchedule(true);
        }
    }
}
