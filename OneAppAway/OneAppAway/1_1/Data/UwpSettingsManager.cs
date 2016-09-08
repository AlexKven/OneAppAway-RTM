using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace OneAppAway._1_1.Data
{
    internal class UwpSettingsManager : SettingsManagerBase
    {
        private UwpSettingsManager() { }

        public static UwpSettingsManager Instance { get; set; } = new UwpSettingsManager();

        public override T GetSetting<T>(string settingName, bool roaming, T def)
        {
            ApplicationDataContainer curContainer = roaming ? ApplicationData.Current.RoamingSettings : ApplicationData.Current.LocalSettings;
            object result = curContainer.Values[settingName];
            if (result == null) return def;
            return (T)result;
        }

        public override void SetSetting<T>(string settingName, bool roaming, T value)
        {
            ApplicationDataContainer curContainer = roaming ? ApplicationData.Current.RoamingSettings : ApplicationData.Current.LocalSettings;
            if (!curContainer.Values.ContainsKey(settingName))
                curContainer.Values.Add(settingName, value);
            else
                curContainer.Values[settingName] = value;
        }
    }
}
