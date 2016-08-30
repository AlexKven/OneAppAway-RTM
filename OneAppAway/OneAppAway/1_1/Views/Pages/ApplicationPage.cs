using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneAppAway.Common;
using System.Collections.ObjectModel;
using System.Threading;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using OneAppAway._1_1.Data;
using MvvmHelpers;
using OneAppAway._1_1.Views.Structures;
using OneAppAway._1_1.Selectors;

namespace OneAppAway._1_1.Views.Pages
{
    public class ApplicationPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        protected MemoryCache Cache;

        public ApplicationPage()
        {
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState; //Done
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState; //Done
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Disabled;
            Cache = new MemoryCache();
            TitleTemplateSelector = new TitleOnlyTitleBarTemplateSelector();
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            OnLoadState(e.PageState, e.NavigationParameter);
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            OnSaveState(e.PageState);
        }

        protected virtual void OnLoadState(Dictionary<string, object> state, object navigationParameter) { }
        protected virtual void OnSaveState(Dictionary<string, object> state) { }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
            this.navigationHelper.LoadState -= this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState -= this.NavigationHelper_SaveState;
            Cache.Dispose();
        }

        public bool CanGoBack
        {
            get { return (bool)GetValue(CanGoBackProperty); }
            set { SetValue(CanGoBackProperty, value); }
        }
        public static readonly DependencyProperty CanGoBackProperty =
            DependencyProperty.Register("CanGoBack", typeof(bool), typeof(ApplicationPage), new PropertyMetadata(false, OnCanGoBackChangedStatic));
        static void OnCanGoBackChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as ApplicationPage)?.CanGoBackChanged?.Invoke(sender, new EventArgs());
        }
        public event EventHandler CanGoBackChanged;
        public virtual void GoBack()
        {
            
        }
        
        public TitleBarTemplateSelectorBase TitleTemplateSelector
        {
            get { return (TitleBarTemplateSelectorBase)GetValue(TitleTemplateSelectorProperty); }
            set { SetValue(TitleTemplateSelectorProperty, value); }
        }
        public static readonly DependencyProperty TitleTemplateSelectorProperty =
            DependencyProperty.Register("TitleTemplateSelector", typeof(TitleBarTemplateSelectorBase), typeof(ApplicationPage), new PropertyMetadata(new TitleOnlyTitleBarTemplateSelector(), OnTitleTemplateSelectorChangedStatic));
        static void OnTitleTemplateSelectorChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as ApplicationPage)?.TitleTemplateSelectorChanged?.Invoke(sender, EventArgs.Empty);
            (sender as ApplicationPage)?.RefreshTitleTemplateBindings();
        }
        public event EventHandler TitleTemplateSelectorChanged;

        private void RefreshTitleTemplateBindings()
        {
            SetBinding(TitleControlsOverflowTemplateProperty, new Windows.UI.Xaml.Data.Binding() { Source = TitleTemplateSelector, Path = new PropertyPath("OverflowControlsTemplate"), Mode = Windows.UI.Xaml.Data.BindingMode.OneWay });
            SetBinding(TitleControlsOverflowWidthProperty, new Windows.UI.Xaml.Data.Binding() { Source = TitleTemplateSelector, Path = new PropertyPath("OverflowControlsWidth"), Mode = Windows.UI.Xaml.Data.BindingMode.OneWay });
        }

        public DataTemplate TitleControlsOverflowTemplate
        {
            get { return (DataTemplate)GetValue(TitleControlsOverflowTemplateProperty); }
            set { SetValue(TitleControlsOverflowTemplateProperty, value); }
        }
        public static readonly DependencyProperty TitleControlsOverflowTemplateProperty =
            DependencyProperty.Register("TitleControlsOverflowTemplate", typeof(DataTemplate), typeof(ApplicationPage), new PropertyMetadata(null));

        public double TitleControlsOverflowWidth
        {
            get { return (double)GetValue(TitleControlsOverflowWidthProperty); }
            set { SetValue(TitleControlsOverflowWidthProperty, value); }
        }
        public static readonly DependencyProperty TitleControlsOverflowWidthProperty =
            DependencyProperty.Register("TitleControlsOverflowWidth", typeof(double), typeof(ApplicationPage), new PropertyMetadata(0));

        ~ApplicationPage()
        {
            Cache.Dispose();
        }
    }
}
