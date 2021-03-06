﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OneAppAway._1_1.Helpers
{
    //From this page:
    //https://marcominerva.wordpress.com/2013/07/30/using-windows-8-1-flyout-xaml-control-with-mvvm/
    public static class FlyoutHelpers
    {
        public static readonly DependencyProperty IsOpenProperty =
        DependencyProperty.RegisterAttached("IsOpen", typeof(bool),
        typeof(FlyoutHelpers), new PropertyMetadata(false, OnIsOpenPropertyChanged));

        public static readonly DependencyProperty ParentProperty =
        DependencyProperty.RegisterAttached("Parent", typeof(FrameworkElement),
        typeof(FlyoutHelpers), new PropertyMetadata(null, OnParentPropertyChanged));

        public static void SetIsOpen(DependencyObject d, bool value)
        {
            d.SetValue(IsOpenProperty, value);
        }

        public static bool GetIsOpen(DependencyObject d)
        {
            return (bool)d.GetValue(IsOpenProperty);
        }

        public static void SetParent(DependencyObject d, FrameworkElement value)
        {
            d.SetValue(ParentProperty, value);
        }

        public static FrameworkElement GetParent(DependencyObject d)
        {
            return (FrameworkElement)d.GetValue(ParentProperty);
        }

        private static void OnParentPropertyChanged(DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        {
            var flyout = d as Flyout;
            if (flyout != null)
            {
                flyout.Opening += (s, args) =>
                {
                    flyout.SetValue(IsOpenProperty, true);
                };

                flyout.Closed += (s, args) =>
                {
                    flyout.SetValue(IsOpenProperty, false);
                };
            }
        }

        private static void OnIsOpenPropertyChanged(DependencyObject d,
        DependencyPropertyChangedEventArgs e)
        {
            var flyout = d as Flyout;
            var parent = (FrameworkElement)d.GetValue(ParentProperty);

            if (flyout != null && parent != null)
            {
                var newValue = (bool)e.NewValue;

                if (newValue)
                    flyout.ShowAt(parent);
                else
                    flyout.Hide();
            }
        }
    }
}
