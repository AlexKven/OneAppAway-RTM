using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OneAppAway._1_1.Helpers
{
    public abstract class EventCommandBindingBase<T, THandler> : BindingHelper
    {
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(EventCommandBindingBase<T, THandler>), new PropertyMetadata(null));


        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(EventCommandBindingBase<T, THandler>), new PropertyMetadata(null));

        internal override void Register()
        {
            FrameworkElement element;
            if (Element.TryGetTarget(out element))
            {
                var events = element?.GetType().GetTypeInfo().DeclaredEvents.First();
                
            }
        }

        internal override void Deregister()
        {
            throw new NotImplementedException();
        }
    }
}

