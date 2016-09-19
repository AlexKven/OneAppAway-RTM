using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OneAppAway.Common
{
    public class WeakRelayCommand : ICommand
    {
        public WeakRelayCommand(Action<object> executeCallback, Func<object, bool> canExecuteCallback = null)
        {
            ExecuteTargetReference = new WeakReference<object>(executeCallback.Target);
            if (canExecuteCallback != null)
                CanExecuteTargetReference = new WeakReference<object>(canExecuteCallback.Target);
            ExecuteCallbackInfo = executeCallback.GetMethodInfo();
            CanExecuteCallbackInfo = canExecuteCallback?.GetMethodInfo();
        }

        private WeakReference<object> ExecuteTargetReference;
        private WeakReference<object> CanExecuteTargetReference;
        private MethodInfo ExecuteCallbackInfo;
        private MethodInfo CanExecuteCallbackInfo;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (CanExecuteCallbackInfo == null)
                return true;
            object canExecuteTarget = null;
            if (CanExecuteTargetReference?.TryGetTarget(out canExecuteTarget) ?? false)
            {
                return (bool)CanExecuteCallbackInfo.Invoke(canExecuteTarget, new[] { parameter });
            }
            else
                return false;
        }

        public void Execute(object parameter)
        {
            object executeTarget = null;
            if (ExecuteTargetReference?.TryGetTarget(out executeTarget) ?? false)
            {
                ExecuteCallbackInfo.Invoke(executeTarget, new[] { parameter });
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
