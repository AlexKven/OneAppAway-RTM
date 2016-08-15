using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OneAppAway._1_1
{
    public class Command : ICommand
    {
        public Command(Action<object> executeCallback, Func<object, bool> canExecuteCallback = null)
        {
            ExecuteCallback = executeCallback;
            CanExecuteCallback = canExecuteCallback ?? new Func<object, bool>((obj) => true);
        }

        private Action<object> ExecuteCallback;
        private Func<object, bool> CanExecuteCallback;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return CanExecuteCallback(parameter);
        }

        public void Execute(object parameter)
        {
            ExecuteCallback(parameter);
        }

        public void ChangeCanExecute()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}
