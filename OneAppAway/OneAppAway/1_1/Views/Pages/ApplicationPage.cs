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
            TitleTemplate = App.Current.Resources["SimpleTitleTemplate"] as DataTemplate;
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

        public DataTemplate TitleTemplate
        {
            get { return (DataTemplate)GetValue(TitleTemplateProperty); }
            set { SetValue(TitleTemplateProperty, value); }
        }
        public static readonly DependencyProperty TitleTemplateProperty =
            DependencyProperty.Register("TitleTemplate", typeof(DataTemplate), typeof(ApplicationPage), new PropertyMetadata(null, TitleTemplateChangedStatic));
        static void TitleTemplateChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as ApplicationPage)?.TitleTemplateChanged?.Invoke(sender, EventArgs.Empty);
        }
        public event EventHandler TitleTemplateChanged;

        public DataTemplate TitleControlsTemplate
        {
            get { return (DataTemplate)GetValue(TitleControlsTemplateProperty); }
            set { SetValue(TitleControlsTemplateProperty, value); }
        }
        public static readonly DependencyProperty TitleControlsTemplateProperty =
            DependencyProperty.Register("TitleControlsTemplate", typeof(DataTemplate), typeof(ApplicationPage), new PropertyMetadata(null, TitleControlsTemplateChangedStatic));
        static void TitleControlsTemplateChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as ApplicationPage)?.TitleControlsTemplateChanged?.Invoke(sender, EventArgs.Empty);
        }
        public event EventHandler TitleControlsTemplateChanged;

        public TitleBarElementSize TitleSize
        {
            get { return (TitleBarElementSize)GetValue(TitleSizeProperty); }
            set { SetValue(TitleSizeProperty, value); }
        }
        public static readonly DependencyProperty TitleSizeProperty =
            DependencyProperty.Register("TitleSize", typeof(TitleBarElementSize), typeof(ApplicationPage), new PropertyMetadata(new TitleBarElementSize(1, 0), TitleSizeChangedStatic));
        static void TitleSizeChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as ApplicationPage)?.TitleSizeChanged?.Invoke(sender, EventArgs.Empty);
        }
        public event EventHandler TitleSizeChanged;

        public TitleBarElementSize TitleControlsSize
        {
            get { return (TitleBarElementSize)GetValue(TitleControlsSizeProperty); }
            set { SetValue(TitleControlsSizeProperty, value); }
        }
        public static readonly DependencyProperty TitleControlsSizeProperty =
            DependencyProperty.Register("TitleControlsSize", typeof(TitleBarElementSize), typeof(ApplicationPage), new PropertyMetadata(new TitleBarElementSize(), TitleControlsSizeChangedStatic));
        static void TitleControlsSizeChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as ApplicationPage)?.TitleControlsSizeChanged?.Invoke(sender, EventArgs.Empty);
        }
        public event EventHandler TitleControlsSizeChanged;

        public TitleBarElementSize TitleSpaceSize
        {
            get { return (TitleBarElementSize)GetValue(TitleSpaceSizeProperty); }
            set { SetValue(TitleSpaceSizeProperty, value); }
        }
        public static readonly DependencyProperty TitleSpaceSizeProperty =
            DependencyProperty.Register("TitleSpaceSize", typeof(TitleBarElementSize), typeof(ApplicationPage), new PropertyMetadata(new TitleBarElementSize(), TitleSpaceSizeChangedStatic));
        static void TitleSpaceSizeChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as ApplicationPage)?.TitleSpaceSizeChanged?.Invoke(sender, EventArgs.Empty);
        }
        public event EventHandler TitleSpaceSizeChanged;
        
        ~ApplicationPage()
        {
            Cache.Dispose();
        }
    }
}
