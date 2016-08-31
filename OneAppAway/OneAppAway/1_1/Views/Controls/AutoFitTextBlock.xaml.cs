using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace OneAppAway._1_1.Views.Controls
{
    public sealed partial class AutoFitTextBlock : UserControl
    {
        public AutoFitTextBlock()
        {
            this.InitializeComponent();
            this.RegisterPropertyChangedCallback(FontFamilyProperty, (s, e) => RecalculateSize());
            this.RegisterPropertyChangedCallback(FontWeightProperty, (s, e) => RecalculateSize());
            this.RegisterPropertyChangedCallback(FontStyleProperty, (s, e) => RecalculateSize());
            InnerBlock.SetBinding(TextBlock.TextProperty, new Binding() { Source = this, Path = new PropertyPath("Text") });
        }
        
        void RecalculateSize()
        {
            InnerBlock.FontSize = FontSize;
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(AutoFitTextBlock), new PropertyMetadata("", (s, e) => (s as AutoFitTextBlock)?.RecalculateSize()));

        private void InnerBlock_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (double.IsNaN(InnerBlock.ActualHeight) || InnerBlock.ActualHeight <= 0)
                return;
            if (double.IsNaN(InnerBlock.ActualHeight) || InnerBlock.ActualHeight <= 0)
                return;
            if (InnerBlock.FontSize < 1)
                return;
            if (InnerBlock.ActualHeight > ActualHeight || InnerBlock.ActualWidth > ActualWidth)
                InnerBlock.FontSize -= .5;
        }

        private void Self_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width != e.PreviousSize.Width && HorizontalAlignment == HorizontalAlignment.Stretch)
                RecalculateSize();
        }
    }
}
