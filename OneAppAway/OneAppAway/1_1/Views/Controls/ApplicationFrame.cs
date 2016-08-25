using OneAppAway._1_1.Views.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OneAppAway._1_1.Views.Controls
{
    public class ApplicationFrame : Frame
    {
        public ApplicationFrame()
        {
            Navigated += ApplicationFrame_Navigated;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
        }

        private void ApplicationFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            if (e.Content is ApplicationPage)
            {
                SetBinding(CanGoBackWithinPageProperty, new Windows.UI.Xaml.Data.Binding() { Source = e.Content, Path = new PropertyPath("CanGoBack"), Mode = Windows.UI.Xaml.Data.BindingMode.OneWay });
            }
        }
        
        public bool CanGoBackWithinPage
        {
            get { return (bool)GetValue(CanGoBackWithinPageProperty); }
            set { SetValue(CanGoBackWithinPageProperty, value); }
        }
        public static readonly DependencyProperty CanGoBackWithinPageProperty =
            DependencyProperty.Register("CanGoBackWithinPage", typeof(bool), typeof(ApplicationFrame), new PropertyMetadata(false, OnCanGoBackWithinPageChangedStatic));
        static void OnCanGoBackWithinPageChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as ApplicationFrame)?.CanGoBackWithinPageChanged?.Invoke(sender, new EventArgs());
        }
        public event EventHandler CanGoBackWithinPageChanged;
        public void GoBackWithinPage()
        {
            (Content as ApplicationPage)?.GoBack();
        }
    }
}
