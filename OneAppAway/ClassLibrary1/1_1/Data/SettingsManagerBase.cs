using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.Data
{
    public abstract class SettingsManagerBase
    {
        public static SettingsManagerBase Instance { get; set; }

        public abstract T GetSetting<T>(string settingName, bool roaming, T def);

        public T GetSetting<T>(string settingName, bool roaming) => GetSetting<T>(settingName, roaming, default(T));

        public abstract void SetSetting<T>(string settingName, bool roaming, T value);

        public ManuallyDownloadArrivalsMode CurrentDownloadArrivalsMode
        {
            get { return GetSetting($"{(NetworkManagerBase.Instance.UnlimitedNetwork ? "UnlimitedData" : "LimitedData")}.ManuallyDownloadArrivalsMode", false, ManuallyDownloadArrivalsMode.Never); }
            set { SetSetting($"{(NetworkManagerBase.Instance.UnlimitedNetwork ? "UnlimitedData" : "LimitedData")}.ManuallyDownloadArrivalsMode", false, value); }
        }
    }
}
