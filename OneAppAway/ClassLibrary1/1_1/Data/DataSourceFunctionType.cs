using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.Data
{
    [Flags]
    public enum DataSourceFunctionType
    {
        None = 0,
        Provision = 1,
        Correction = 2,
        Both = 3
    }
}
