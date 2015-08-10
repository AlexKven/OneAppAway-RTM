using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace OneAppAway
{
    public static class SettingsManager
    {
        public static T GetSetting<T>(string settingName, bool roaming, T def)
        {
            ApplicationDataContainer curContainer = roaming ? ApplicationData.Current.RoamingSettings : ApplicationData.Current.LocalSettings;
            object result = curContainer.Values[settingName];
            if (result == null) return def;
            return (T)result;
        }

        public static T GetSetting<T>(string settingName, bool roaming) => GetSetting<T>(settingName, roaming, default(T));

        public static void SetSetting<T>(string settingName, bool roaming, T value)
        {
            ApplicationDataContainer curContainer = roaming ? ApplicationData.Current.RoamingSettings : ApplicationData.Current.LocalSettings;
            if (!curContainer.Values.ContainsKey(settingName))
                curContainer.Values.Add(settingName, value);
            else
                curContainer.Values[settingName] = value;
        }
    }
}
