using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace OneAppAway._1_1.Views.Controls
{
    public class EnterCommandTextBox : TextBox
    {
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter && e.KeyStatus.RepeatCount == 0)
            {
                if (Command?.CanExecute(Text) ?? false)
                {
                    Command.Execute(Text);
                }
            }
            e.Handled = true;
            base.OnKeyDown(e);
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(EnterCommandTextBox), new PropertyMetadata(null));


    }
}
