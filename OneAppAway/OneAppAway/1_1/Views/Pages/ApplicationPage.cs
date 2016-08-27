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
        public virtual void GoBack() { }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ApplicationPage), new PropertyMetadata(null, OnTitleChangedStatic));
        static void OnTitleChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as ApplicationPage)?.TitleChanged?.Invoke(sender, new EventArgs());
        }
        public event EventHandler TitleChanged;

        //public bool GoBack()
        //{
        //    bool handled = false;
        //    if (CanGoBackLocal)
        //        OnGoBackLocal(ref handled);
        //    if (!handled)
        //    {
        //        if (NavigationHelper.CanGoBack())
        //        {
        //            NavigationHelper.GoBack();
        //            handled = true;
        //        }
        //    }
        //    return handled;
        //}

        //protected virtual void OnGoBackLocal(ref bool handled) { }

        //public bool CanGoBackLocal
        //{
        //    get { return _CanGoBack; }
        //    private set
        //    {
        //        _CanGoBack = value;
        //        UpdateBackButtonVisibility();
        //    }
        //}

        //public bool CanGoBack()
        //{
        //    return CanGoBackLocal || NavigationHelper.CanGoBack();
        //}

        //private void UpdateBackButtonVisibility()
        //{
        //    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
        //        (CanGoBackLocal || NavigationHelper.CanGoBack()) ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        //}

        //internal virtual void OnRefreshTitleBarControls(OuterFrame mainFrame, double totalWidth)
        //{

        //}

        ~ApplicationPage()
        {
            Cache.Dispose();
        }
    }
}
