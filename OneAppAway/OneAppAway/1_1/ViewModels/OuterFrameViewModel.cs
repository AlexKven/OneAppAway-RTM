using MvvmHelpers;
using OneAppAway._1_1.Views.Controls;
using OneAppAway._1_1.Views.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.ViewModels
{
    class OuterFrameViewModel : BaseViewModel
    {
        private App CurrentApp;
        private ApplicationFrame Frame;

        public Command GoBackCommand { get; }
        public Command NavigateBackCommand { get; }
        public Command NavigateForwardCommand { get; }

        public OuterFrameViewModel(App currentApp, ApplicationFrame frame)
        {
            CurrentApp = currentApp;
            GoBackCommand = new Command(obj => GoBack(), obj => Frame.CanGoBack || Frame.CanGoBackWithinPage);
            NavigateBackCommand = new Command(obj => Frame.GoBack(), obj => Frame.CanGoBack);
            NavigateForwardCommand = new Command(obj => Frame.GoForward(), obj => Frame.CanGoForward);
            Frame = frame;
            Frame.Navigated += Frame_Navigated;
            Frame.CanGoBackWithinPageChanged += Frame_CanGoBackWithinPageChanged;
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += OuterFrameViewModel_BackRequested;
        }

        private void GoBack()
        {
            if (Frame.CanGoBackWithinPage)
                Frame.GoBackWithinPage();
            else
                Frame.GoBack();
        }

        private void Frame_CanGoBackWithinPageChanged(object sender, EventArgs e)
        {
            GoBackCommand.ChangeCanExecute();
        }

        private void Frame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            GoBackCommand.ChangeCanExecute();
            NavigateBackCommand.ChangeCanExecute();
            NavigateForwardCommand.ChangeCanExecute();
        }

        private void OuterFrameViewModel_BackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            if (GoBackCommand.CanExecute(null))
            {
                e.Handled = true;
                GoBackCommand.Execute(null);
            }
        }
    }
}
