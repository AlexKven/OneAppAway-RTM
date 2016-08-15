using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.ObjectModel;
using Windows.System.Threading;
using Windows.Foundation;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace OneAppAway
{
    public static class TaskManager
    {
        private static List<Tuple<NavigationFriendlyPage, int, IAsyncAction, AppTask>> RunningTasks = new List<Tuple<NavigationFriendlyPage, int, IAsyncAction, AppTask>>();
        private static int CurrentID = 0;

        public static int RunTask(NavigationFriendlyPage page, AppTask task)
        {
            int id = CurrentID++;
            Tuple<NavigationFriendlyPage, int, IAsyncAction, AppTask> item = new Tuple<NavigationFriendlyPage, int, IAsyncAction, AppTask>(page, id, ThreadPool.RunAsync(task.GetWorkItemHandler(page.Dispatcher)), task);
            
            return id;
        }

        public static void CancelTask(int id)
        {
            var target = (from task in RunningTasks where task.Item2 == id select task).FirstOrDefault();
            if (target != null)
            {
                target.Item3.Cancel();
                RunningTasks.Remove(target);
            }
        }

        public static void CancelPage(NavigationFriendlyPage page)
        {
            var targets = from task in RunningTasks where task.Item1 == page select task;
            foreach (var task in targets)
            {
                task.Item3.Cancel();
                RunningTasks.Remove(task);
            }
        }

        private static WorkItemHandler GetWorkItemHandler(this AppTask task, CoreDispatcher dispacher)
        {
            return (action) =>
                {
                    task.Run(action, dispacher);
                };
        }
    }

    public delegate bool AppTaskDelegate(object parameter, IAsyncAction action, AppTaskResult? prevResult, out object result);

    public class AppTask
    {
        private Stack<AppTaskDelegate> TaskStack = new Stack<AppTaskDelegate>();
        private int CurrentLevel = 0;
        private int SuccessFallbackLimit;
        private object Parameter;

        public readonly Action<AppTaskResult> CompletedCallback;

        public AppTask(Action<AppTaskResult> completedCallback, object parameter, params AppTaskDelegate[] tasks)
            : this(-1, completedCallback, parameter, tasks)
        { }

        public AppTask(int successFallbackLimit, Action<AppTaskResult> completedCallback, object parameter, params AppTaskDelegate[] tasks)
        {
            foreach (var task in tasks)
                TaskStack.Push(task);
            SuccessFallbackLimit = successFallbackLimit;
            CompletedCallback = completedCallback;
        }

        internal void Run(IAsyncAction action, CoreDispatcher uiDispatcher)
        {
            bool cont = true;
            AppTaskResult? lastResult = null;
            while (cont)
            {
                var task = TaskStack.Pop();
                object data;
                bool success = task(Parameter, action, lastResult, out data);
                lastResult = new AppTaskResult(success, data, lastResult);
                cont = (SuccessFallbackLimit == -1 || SuccessFallbackLimit > CurrentLevel) && TaskStack.Count > 0;
                CurrentLevel++;
            }
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            if (action.Status != AsyncStatus.Canceled)
                uiDispatcher.RunAsync(CoreDispatcherPriority.High, () => { CompletedCallback(lastResult.Value); });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }

    //public class OnlineOfflineTask : AppTask
    //{

    //}

    public struct AppTaskResult
    {
        public bool Success { get; private set; }
        public object Data { get; private set; }
        public object PreviousResult { get; private set; }

        public AppTaskResult(bool success, object data, AppTaskResult? previousResult)
        {
            Success = success;
            Data = data;
            PreviousResult = previousResult;
        }

        public object[] GetDataChain()
        {
            List<object> result = new List<object>();
            Action<AppTaskResult> AddResultDelegate = null;
            AddResultDelegate = (atr) =>
            {
                result.Insert(0, atr.Data);
                if (atr.PreviousResult != null)
                    AddResultDelegate((AppTaskResult)atr.PreviousResult);
            };
            AddResultDelegate(this);
            return result.ToArray();
        }
    }
}
