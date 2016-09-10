using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace OneAppAway._1_1.Helpers
{
    public abstract class BindingHelper : DependencyObject
    {
        protected WeakReference<FrameworkElement> Element;

        internal abstract void Register();
        internal abstract void Deregister();

        public static BindingHelper GetAppliedBindingHelper(DependencyObject obj)
        {
            return (BindingHelper)obj.GetValue(AppliedBindingHelperProperty);
        }

        public static void SetAppliedBindingHelper(DependencyObject obj, BindingHelper value)
        {
            obj.SetValue(AppliedBindingHelperProperty, value);
        }
        public static readonly DependencyProperty AppliedBindingHelperProperty =
            DependencyProperty.RegisterAttached("AppliedBindingHelper", typeof(BindingHelper), typeof(FrameworkElement), new PropertyMetadata(null, OnAppliedBindingHelperChangedStatic));
        private static void OnAppliedBindingHelperChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var typedSender = sender as FrameworkElement;
            if (typedSender == null)
                return;
            var oldHelper = e.OldValue as BindingHelper;
            var newHelper = e.NewValue as BindingHelper;
            oldHelper?.Deregister();
            newHelper.Element = new WeakReference<FrameworkElement>(typedSender);
            newHelper?.Register();
        }
    }
}
