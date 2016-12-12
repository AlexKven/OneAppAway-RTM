using OneAppAway._1_1.Abstract;
using OneAppAway._1_1.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace OneAppAway._1_1
{
    public class UwpIntervalExecuter : IntervalExecuterBase
    {
        internal UwpIntervalExecuter()
        {
            MinInterval = TimeSpan.FromSeconds(1);
        }

        private DispatcherTimer Timer = new DispatcherTimer();
        protected override void Initialize()
        {
            Timer.Interval = TimeSpan.FromSeconds(3);
            Timer.Tick += async (s, e) => await App.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => (Instance as UwpIntervalExecuter).Tick());
            Timer.Start();
        }
    }
}
