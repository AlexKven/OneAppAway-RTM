using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace OneAppAway._1_1.Helpers
{
    public static class AttachedProperties
    {
        public static Color GetClosedBackgroundColor(DependencyObject obj)
        {
            return (Color)obj.GetValue(ClosedBackgroundColorProperty);
        }
        public static void SetClosedBackgroundColor(DependencyObject obj, Color value)
        {
            obj.SetValue(ClosedBackgroundColorProperty, value);
        }
        public static readonly DependencyProperty ClosedBackgroundColorProperty =
            DependencyProperty.RegisterAttached("ClosedBackgroundColor", typeof(Color), typeof(CommandBar), new PropertyMetadata(Colors.Transparent));


        public static bool GetUseBackgroundColorHelper(DependencyObject obj)
        {
            return (bool)obj.GetValue(UseBackgroundColorHelperProperty);
        }

        public static void SetUseBackgroundColorHelper(DependencyObject obj, bool value)
        {
            obj.SetValue(UseBackgroundColorHelperProperty, value);
        }

        // Using a DependencyProperty as the backing store for UseBackgroundColorHelper.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseBackgroundColorHelperProperty =
            DependencyProperty.RegisterAttached("UseBackgroundColorHelper", typeof(bool), typeof(CommandBar), new PropertyMetadata(false, OnUseBackgroundColorHelperChangedStatic));
        static void OnUseBackgroundColorHelperChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.OldValue is bool) || !(e.NewValue is bool))
                return;
            var _old = (bool)e.OldValue;
            var _new = (bool)e.NewValue;
            var _bar = sender as CommandBar;
            if (_new && !_old)
            {
                _bar.Opening += _bar_Opening;
                _bar.Closing += _bar_Closing;
            }
            if (_old && !_new)
            {
                _bar.Opening -= _bar_Opening;
                _bar.Closing -= _bar_Closing;
            }
        }



        public static Color GetOpenedBackgroundColor(DependencyObject obj)
        {
            return (Color)obj.GetValue(OpenedBackgroundColorProperty);
        }
        public static void SetOpenedBackgroundColor(DependencyObject obj, Color value)
        {
            obj.SetValue(OpenedBackgroundColorProperty, value);
        }
        public static readonly DependencyProperty OpenedBackgroundColorProperty =
            DependencyProperty.RegisterAttached("OpenedBackgroundColor", typeof(Color), typeof(CommandBar), new PropertyMetadata(Colors.Transparent));

        private static void _bar_Closing(object sender, object e)
        {
            var bar = sender as CommandBar;
            var brush = bar?.Background as SolidColorBrush;
            if (brush == null)
                return;
            var color = GetClosedBackgroundColor(bar);
            if (color == null)
                return;
            ColorAnimation ca = new ColorAnimation() { To = color, Duration = TimeSpan.FromMilliseconds(200) };
            Storyboard.SetTarget(ca, brush);
            Storyboard.SetTargetProperty(ca, "Color");
            Storyboard sb = new Storyboard();
            sb.Children.Add(ca);
            sb.Begin();
        }

        private static void _bar_Opening(object sender, object e)
        {
            var bar = sender as CommandBar;
            var brush = bar?.Background as SolidColorBrush;
            if (brush == null)
                return;
            var color = GetOpenedBackgroundColor(bar);
            if (color == null)
                return;
            ColorAnimation ca = new ColorAnimation() { To = color, Duration = TimeSpan.FromMilliseconds(200) };
            Storyboard.SetTarget(ca, brush);
            Storyboard.SetTargetProperty(ca, "Color");
            Storyboard sb = new Storyboard();
            sb.Children.Add(ca);
            sb.Begin();
        }
    }
}
