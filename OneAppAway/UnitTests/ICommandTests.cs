using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OneAppAway.Common;
using System.Windows.Input;

namespace UnitTests
{
    [TestClass]
    public class ICommandTests
    {
        [TestMethod]
        public void BasicTests()
        {
            TestICommandImplementation((ec, cec) => new RelayCommand(ec, cec), (c) => c.RaiseCanExecuteChanged());
            TestICommandImplementation((ec, cec) => new WeakRelayCommand(ec, cec), (c) => c.RaiseCanExecuteChanged());
        }

        public void TestICommandImplementation<C>(Func<Action<object>, Func<object, bool>, C> newCallback, Action<C> raiseCanExecuteChangedCallback) where C : ICommand
        {
            bool executed = false;
            bool canExecute = false;
            bool canExecuteChanged = false;
            C testCommand = newCallback((obj) => executed = true, (obj) => canExecute);
            testCommand.CanExecuteChanged += (s, e) => canExecuteChanged = true;
            Tests.AssertExpectedVsActual($"{typeof(C).Name}: testCommand.CanExecute()", false, testCommand.CanExecute(null));
            canExecute = true;
            Tests.AssertExpectedVsActual($"{typeof(C).Name}: testCommand.CanExecute()", true, testCommand.CanExecute(null));
            raiseCanExecuteChangedCallback(testCommand);
            Tests.AssertExpectedVsActual($"{typeof(C).Name}: canExecuteChanged", true, canExecuteChanged);
            testCommand.Execute(null);
            Tests.AssertExpectedVsActual($"{typeof(C).Name}: executed", true, executed);
        }
    }
}
