using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace OneAppAway._1_1.Data
{
    public class TimeDetails : DependencyObject
    {
        //Needs external platform-specific class to manually tick this class
        public static TimeDetails Instance { get; }
        const int TICK_INTERVAL_MS = 10;

        private class TickIntervalTask
        {
            public Action Delegate { get; set; }
            public int TickInterval { get; set; }
            public int CurrentTick { get; set; } = 0;
        }

        private List<TickIntervalTask> CurrentTasks = new List<TickIntervalTask>();

        static TimeDetails()
        {
            Instance = new TimeDetails();
        }

        private TimeDetails()
        {
            Refresh();
        }

        public void Tick()
        {
            foreach (var task in CurrentTasks)
            {
                task.CurrentTick++;
                if (task.CurrentTick >= task.TickInterval)
                {
                    task.CurrentTick = 0;
                    task.Delegate();
                }
            }
        }

        public void Refresh()
        {
            Now = DateTime.Now;
        }
        
        public DateTime Now
        {
            get { return (DateTime)GetValue(NowProperty); }
            set { SetValue(NowProperty, value); }
        }
        public static readonly DependencyProperty NowProperty =
            DependencyProperty.Register("Now", typeof(DateTime), typeof(TimeDetails), new PropertyMetadata(new DateTime()));

        public void RegisterTask(Action action, int tickInterval)
        {
            CurrentTasks.Add(new TickIntervalTask() { Delegate = action, TickInterval = tickInterval });
        }

        public void DeregisterTask(Action action)
        {
            CurrentTasks.RemoveAll(task => task.Delegate == action);
        }
    }
}
