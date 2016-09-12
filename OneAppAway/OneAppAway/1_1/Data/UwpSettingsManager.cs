using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace OneAppAway._1_1.Data
{
    internal class UwpSettingsManager : SettingsManagerBase
    {
        internal UwpSettingsManager() { }

        public override T GetSetting<T>(string settingName, bool roaming, T def)
        {
            ApplicationDataContainer curContainer = roaming ? ApplicationData.Current.RoamingSettings : ApplicationData.Current.LocalSettings;
            object result = curContainer.Values[settingName];
            if (result == null) return def;
            if (typeof(T).GetTypeInfo().IsEnum)
                return (T)Enum.Parse(typeof(T), result.ToString());
            return (T)result;
        }

        public override void SetSetting<T>(string settingName, bool roaming, T value)
        {
            object finalValue = value;
            if (typeof(T).GetTypeInfo().IsEnum)
                finalValue = value.ToString();
            ApplicationDataContainer curContainer = roaming ? ApplicationData.Current.RoamingSettings : ApplicationData.Current.LocalSettings;
            if (!curContainer.Values.ContainsKey(settingName))
                curContainer.Values.Add(settingName, finalValue);
            else
                curContainer.Values[settingName] = finalValue;
        }
    }
}
