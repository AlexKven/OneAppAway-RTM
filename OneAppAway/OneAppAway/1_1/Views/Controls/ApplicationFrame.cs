using OneAppAway._1_1.Views.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace OneAppAway._1_1.Views.Controls
{
    public class ApplicationFrame : Frame
    {
        public ApplicationFrame()
        {
            Navigated += ApplicationFrame_Navigated;
            
            TransitionCollection transitions = new TransitionCollection();
            transitions.Add(new EntranceThemeTransition() { FromHorizontalOffset = 200 });
            ContentTransitions = transitions;
        }

        private void ApplicationFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            if (e.Content is ApplicationPage)
            {
                SetBinding(CanGoBackWithinPageProperty, new Windows.UI.Xaml.Data.Binding() { Source = e.Content, Path = new PropertyPath("CanGoBack"), Mode = Windows.UI.Xaml.Data.BindingMode.OneWay });
                SetBinding(TitleProperty, new Windows.UI.Xaml.Data.Binding() { Source = e.Content, Path = new PropertyPath("Title"), Mode = Windows.UI.Xaml.Data.BindingMode.OneWay });
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

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ApplicationFrame), new PropertyMetadata(null, OnTitleChangedStatic));
        static void OnTitleChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as ApplicationFrame)?.TitleChanged?.Invoke(sender, new EventArgs());
        }
        public event EventHandler TitleChanged;
    }
}
