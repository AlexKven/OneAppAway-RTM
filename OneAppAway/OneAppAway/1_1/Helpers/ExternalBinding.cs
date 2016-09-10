using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace OneAppAway._1_1.Helpers
{
    public sealed class ExternalBinding : BindingHelper
    {
        public string PropertyName
        {
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register("PropertyName", typeof(string), typeof(ExternalBinding), new PropertyMetadata(null, OnPropertyNameChangedStatic));
        static void OnPropertyNameChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as ExternalBinding)?.Register();
        }

        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(ExternalBinding), new PropertyMetadata(null));



        internal override void Register()
        {
            FrameworkElement element = null;
            if (PropertyName == null ||  !(Element?.TryGetTarget(out element) ?? false))
                return;
            var typeInfo = element.GetType().GetTypeInfo();
            while (typeInfo.FullName != "System.Object")
            {
                var field = typeInfo.DeclaredFields.FirstOrDefault(fi => fi.IsStatic && fi.Name == PropertyName + "Property");
                if (field != null)
                {
                    element.SetBinding((DependencyProperty)field.GetValue(null), new Binding() { Source = this, Path = new PropertyPath("Value"), Mode = BindingMode.TwoWay });
                    return;
                }
                else
                {
                    var property = typeInfo.DeclaredProperties.Where(pi => pi.Name == PropertyName + "Property").Select(pi => pi.GetMethod).FirstOrDefault(mi => mi.IsStatic);
                    if (property != null)
                    {
                        element.SetBinding((DependencyProperty)property.Invoke(null, null), new Binding() { Source = this, Path = new PropertyPath("Value"), Mode = BindingMode.TwoWay });
                        return;
                    }
                }
                typeInfo = typeInfo.BaseType.GetTypeInfo();
            }
        }

        internal override void Deregister()
        {
            
        }
    }
}
