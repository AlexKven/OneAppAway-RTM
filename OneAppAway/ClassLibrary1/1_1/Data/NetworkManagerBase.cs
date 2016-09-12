using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.Data
{
    public abstract class NetworkManagerBase
    {
        public static NetworkManagerBase Instance { get; set; }

        public abstract NetworkType NetworkType { get; }
        public abstract bool UnlimitedNetwork { get; }
        public event EventHandler NetworkTypeChanged;
        protected void OnNetworkTypeChanged()
        {
            NetworkTypeChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
