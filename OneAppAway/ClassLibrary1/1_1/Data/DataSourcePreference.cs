using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.Data
{
    [Flags]
    public enum DataSourcePreference
    {
        MemoryCache = 1,
        Offline = 2,
        Online = 4,
        MemoryCacheOnly = 1,
        OfflineSources = 3,
        All = 7
    }
}
