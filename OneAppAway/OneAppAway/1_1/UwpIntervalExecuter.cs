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
        public static UwpIntervalExecuter Instance { get; } = new UwpIntervalExecuter();
        public static void InitializeDispatcher(CoreDispatcher dispatcher)
        {
            Instance.Dispatcher = dispatcher;
            RealTimeArrivalViewModel.IntervalExecuter = Instance;
            StopArrivalsBoxViewModel.IntervalExecuter = Instance;
        }

        private CoreDispatcher Dispatcher;
        private UwpIntervalExecuter() { }

        private DispatcherTimer Timer = new DispatcherTimer();
        protected override void Initialize()
        {
            Timer.Interval = TimeSpan.FromSeconds(3);
            Timer.Tick += async (s, e) => await Instance?.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Instance.Tick());
            Timer.Start();
        }
    }
}
