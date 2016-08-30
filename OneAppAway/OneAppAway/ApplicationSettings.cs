using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace OneAppAway
{
    public class ApplicationSettings : DependencyObject
    {
        private DispatcherTimer NowTimer;

        public ApplicationSettings()
        {
            NowTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(10) };
            NowTimer.Tick += NowTimer_Tick;
            NowTimer.Start();
            BandwidthSetting = (BandwidthOptions)SettingsManager.GetSetting<int>("BandwidthOptions", false, (int)BandwidthOptions.Auto);
        }

        private async void NowTimer_Tick(object sender, object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () => Now = DateTime.Now);
        }

        public static readonly DependencyProperty BandwidthSettingProperty = DependencyProperty.Register("BandwidthSetting", typeof(BandwidthOptions), typeof(ApplicationSettings), new PropertyMetadata(BandwidthOptions.Auto, OnBandwidthSettingChanged));
        public static readonly DependencyProperty NowProperty = DependencyProperty.Register("Now", typeof(DateTime), typeof(ApplicationSettings), new PropertyMetadata(DateTime.Now));

        public BandwidthOptions BandwidthSetting
        {
            get { return (BandwidthOptions)GetValue(BandwidthSettingProperty); }
            set { SetValue(BandwidthSettingProperty, value); }
        }

        public DateTime Now
        {
            get { return (DateTime)GetValue(NowProperty); }
            set { SetValue(NowProperty, value); } //Not really meant to be used except by NowTimer
        }

        private static void OnBandwidthSettingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BandwidthSettingStatic = (BandwidthOptions)e.NewValue;
        }

        public static BandwidthOptions BandwidthSettingStatic
        {
            get { return (BandwidthOptions)SettingsManager.GetSetting<int>("BandwidthOptions", false); }
            set
            {
                SettingsManager.SetSetting<int>("BandwidthOptions", false, (int)(value));
                BandwidthManager.OnEffectiveBandwidthOptionsChanged();
                FrameworkElement fe;
            }
        }

        public static event EventHandler BandwidthSettingsChanged;
    }
}
