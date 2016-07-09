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

namespace OneAppAway._1_1.Pages
{
    class ApplicationPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private bool _CanGoBack = false;
        private Dictionary<int, Task> _RunningTasks = new Dictionary<int, Task>();
        private Dictionary<int, CancellationTokenSource> _CancellationTokenSources = new Dictionary<int, CancellationTokenSource>();

        public ApplicationPage()
        {
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState; //Done
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState; //Done
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;
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
            this.navigationHelper.GoBackCommand.CanExecuteChanged += GoBackCommand_CanExecuteChanged; //Done
            UpdateBackButtonVisibility();
            GC.Collect();
        }

        private void GoBackCommand_CanExecuteChanged(object sender, System.EventArgs e)
        {
            UpdateBackButtonVisibility();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
            //TaskManager.CancelPage(this);
            this.navigationHelper.LoadState -= this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState -= this.NavigationHelper_SaveState;
            this.navigationHelper.GoBackCommand.CanExecuteChanged -= GoBackCommand_CanExecuteChanged;
        }

        public bool GoBack()
        {
            bool handled = false;
            if (CanGoBack)
                OnGoBack(ref handled);
            if (!handled)
            {
                if (NavigationHelper.CanGoBack())
                {
                    NavigationHelper.GoBack();
                    handled = true;
                }
            }
            return handled;
        }

        protected virtual void OnGoBack(ref bool handled) { }

        protected bool CanGoBack
        {
            get { return _CanGoBack; }
            set
            {
                _CanGoBack = value;
                UpdateBackButtonVisibility();
            }
        }

        private void UpdateBackButtonVisibility()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                (CanGoBack || NavigationHelper.CanGoBack()) ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }

        public int StartOperation(OperationCallback operation, int opID)
        {
            return 0;
        }

        internal virtual void OnRefreshTitleBarControls(OuterFrame mainFrame, double totalWidth)
        {

        }
    }
}
