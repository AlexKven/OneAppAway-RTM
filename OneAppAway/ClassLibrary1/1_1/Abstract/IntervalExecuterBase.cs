using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OneAppAway._1_1.Abstract
{
    public abstract class IntervalExecuterBase
    {
        private class TickIntervalTask
        {
            public ICommand Command { get; set; }
            public TimeSpan TickInterval { get; set; }
            public TimeSpan RemainingTime { get; set; } = TimeSpan.Zero;
            public DateTime? LastTickTime { get; set; } = null;

            public void Tick(DateTime now)
            {
                TimeSpan? timeSinceLastTick = now - LastTickTime;
                if (Command?.CanExecute(timeSinceLastTick) ?? false)
                    Command.Execute(timeSinceLastTick);
                LastTickTime = now;
            }
        }
        private List<TickIntervalTask> RegisteredTasks = new List<TickIntervalTask>();
        private DateTime? LastTickTime = null;

        protected abstract void Initialize();
        public IntervalExecuterBase()
        {
            Initialize();
        }

        protected void Tick()
        {
            var now = DateTime.Now;
            var timeSinceLastTick = now - LastTickTime ?? TimeSpan.Zero;
            foreach (var task in RegisteredTasks)
            {
                while (task.RemainingTime <= TimeSpan.Zero)
                {
                    task.Tick(now);
                    task.RemainingTime += task.TickInterval;
                }
                task.RemainingTime -= timeSinceLastTick;
            }
            LastTickTime = now;
        }

        public void RegisterTask(ICommand command, TimeSpan tickInterval)
        {
            RegisteredTasks.Add(new TickIntervalTask() { Command = command, TickInterval = tickInterval });
        }

        public void DeregisterTask(ICommand command)
        {
            RegisteredTasks.RemoveAll(task => task.Command == command);
        }
    }
}
