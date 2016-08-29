using MvvmHelpers;
using OneAppAway._1_1.Views.Controls;
using OneAppAway._1_1.Views.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using static System.Math;

namespace OneAppAway._1_1.ViewModels
{
    class OuterFrameViewModel : BaseViewModel, IDisposable
    {
        private App CurrentApp;
        private ApplicationFrame Frame;
        private ApplicationPage Page;
        //private double LeftControlsInset;
        private double RightControlsInset;
        private double WindowWidth;

        public OuterFrameViewModel(App currentApp, ApplicationFrame frame)
        {
            CurrentApp = currentApp;
            GoBackCommand = new Command(obj => GoBack(), obj => Frame.CanGoBack || (Page != null && Page.CanGoBack));
            ToggleMenuCommand = new Command(obj => IsMenuOpen = !IsMenuOpen);
            NavigateBackCommand = new Command(obj => Frame.GoBack(), obj => Frame.CanGoBack);
            NavigateForwardCommand = new Command(obj => Frame.GoForward(), obj => Frame.CanGoForward);
            Frame = frame;
            RegisterFrameEvents();
            RefreshPage();

            Window.Current.SizeChanged += Current_SizeChanged;

            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
            var titleBar = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar;
            titleBar.LayoutMetricsChanged += TitleBar_LayoutMetricsChanged;
            RightControlsInset = titleBar.SystemOverlayRightInset;
            //RefreshTitleElementSizes();

            PageEntries.Add(new HamburgerBarPageEntryViewModel(Frame, typeof(TransitMapPage), "Map", new FontFamily("Segoe UI Symbol"), 20, ""));
        }

        private void GoBack()
        {
            if (Page != null && Page.CanGoBack)
                Page.GoBack();
            else
                Frame.GoBack();
        }

        public void Dispose()
        {
            DeregisterPageDataContextEvents();
            DeregisterPageEvents();
            DeregisterFrameEvents();
            PageDataContext = null;
            Page = null;
            Frame = null;
        }

        #region Register/Deregister Events
        private void RegisterPageDataContextEvents()
        {
            if (PageDataContext != null)
                PageDataContext.PropertyChanged += PageDataContext_PropertyChanged;
        }

        private void DeregisterPageDataContextEvents()
        {
            if (PageDataContext != null)
                PageDataContext.PropertyChanged -= PageDataContext_PropertyChanged;
        }

        private void RegisterPageEvents()
        {
            if (Page != null)
            {
                Page.DataContextChanged += Page_DataContextChanged;
                Page.CanGoBackChanged += Page_CanGoBackChanged;
                Page.TitleTemplateChanged += Page_TitleTemplateChanged;
                Page.TitleControlsTemplateChanged += Page_TitleControlsTemplateChanged;
                Page.TitleSizeChanged += Page_TitleSizeChanged;
                Page.TitleControlsSizeChanged += Page_TitleControlsSizeChanged;
                Page.TitleSpaceSizeChanged += Page_TitleSpaceSizeChanged;
            }
        }

        private void DeregisterPageEvents()
        {
            if (Page != null)
            {
                Page.DataContextChanged -= Page_DataContextChanged;
                Page.CanGoBackChanged -= Page_CanGoBackChanged;
                Page.TitleTemplateChanged -= Page_TitleTemplateChanged;
                Page.TitleControlsTemplateChanged -= Page_TitleControlsTemplateChanged;
                Page.TitleSizeChanged -= Page_TitleSizeChanged;
                Page.TitleControlsSizeChanged -= Page_TitleControlsSizeChanged;
                Page.TitleSpaceSizeChanged -= Page_TitleSpaceSizeChanged;
            }
        }

        private void RegisterFrameEvents()
        {
            Frame.Navigated += Frame_Navigated;
        }

        private void DeregisterFrameEvents()
        {
            Frame.Navigated -= Frame_Navigated;
        }
        #endregion

        #region Event Handlers
        private void Page_CanGoBackChanged(object sender, EventArgs e)
        {
            GoBackCommand.ChangeCanExecute();
        }

        private void Frame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            DeregisterPageEvents();
            RefreshPage();
            RegisterPageEvents();
        }

        private void App_BackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            if (GoBackCommand.CanExecute(null))
            {
                e.Handled = true;
                GoBackCommand.Execute(null);
            }
        }//

        private void Page_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            DeregisterPageDataContextEvents();
            PageDataContext = Page?.DataContext as BaseViewModel;
            RegisterPageDataContextEvents();
            RefreshTitle();
        }

        private void PageDataContext_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Title":
                    RefreshTitle();
                    break;
            }
        }

        private void Page_TitleSpaceSizeChanged(object sender, EventArgs e)
        {
            RefreshTitleElementSizes();
        }

        private void Page_TitleControlsSizeChanged(object sender, EventArgs e)
        {
            RefreshTitleElementSizes();
        }

        private void Page_TitleSizeChanged(object sender, EventArgs e)
        {
            RefreshTitleElementSizes();
        }

        private void Page_TitleControlsTemplateChanged(object sender, EventArgs e)
        {
            RefreshTitleControlsTemplate();
        }

        private void Page_TitleTemplateChanged(object sender, EventArgs e)
        {
            RefreshTitleTemplate();
        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            WindowWidth = e.Size.Width;
            RefreshTitleElementSizes();
        }

        private void TitleBar_LayoutMetricsChanged(Windows.ApplicationModel.Core.CoreApplicationViewTitleBar sender, object args)
        {
            var titleBar = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar;
            RightControlsInset = titleBar.SystemOverlayRightInset;
            RefreshTitleElementSizes();
        }
        #endregion

        #region Property Refreshers
        private void RefreshTitle()
        {
            Title = PageDataContext?.Title ?? "OneAppAway";
        }

        private void RefreshPageDataContext()
        {
            DeregisterPageDataContextEvents();
            PageDataContext = Page?.DataContext as BaseViewModel;
            RefreshTitle();
            RegisterPageDataContextEvents();
        }

        private void RefreshPage()
        {
            DeregisterPageEvents();
            Page = Frame.Content as ApplicationPage;
            RefreshPageDataContext();
            RefreshTitleTemplate();
            RefreshTitleControlsTemplate();
            RefreshTitleElementSizes();
            GoBackCommand.ChangeCanExecute();
            NavigateBackCommand.ChangeCanExecute();
            NavigateForwardCommand.ChangeCanExecute();
            RegisterPageEvents();
        }

        private void RefreshTitleTemplate()
        {
            TitleTemplate = Page?.TitleTemplate;
        }

        private void RefreshTitleControlsTemplate()
        {
            TitleControlsTemplate = Page?.TitleControlsTemplate;
        }

        private void RefreshTitleElementSizes()
        {
            if (Page == null)
                return;
            double remainingWidth = -1;
            bool includeTitle = true;
            bool includeTitleControls = true;
            bool includeTitleSpace = true;
            double titleSize = 0;
            double titleControlsSize = 0;
            double titleSpaceSize = 0;
            #region Passes
            while (remainingWidth <= -1)
            {
                remainingWidth = WindowWidth - 50 - RightControlsInset;
                titleSize = Page.TitleSize.GuaranteedWidth;
                titleControlsSize = Page.TitleControlsSize.GuaranteedWidth;
                titleSpaceSize = Page.TitleSpaceSize.GuaranteedWidth;
                double relativeTotal = 0;
                if (includeTitle && Page.TitleSize.RelativeWidth != null)
                    relativeTotal += Page.TitleSize.RelativeWidth.Value;
                if (includeTitleControls && Page.TitleControlsSize.RelativeWidth != null)
                    relativeTotal += Page.TitleControlsSize.RelativeWidth.Value;
                if (includeTitleSpace && Page.TitleSpaceSize.RelativeWidth != null)
                    relativeTotal += Page.TitleSpaceSize.RelativeWidth.Value;

                if (includeTitle && Page.TitleSize.RelativeWidth == null)
                    remainingWidth -= titleSize;
                if (includeTitleControls && Page.TitleControlsSize.RelativeWidth == null)
                    remainingWidth -= titleControlsSize;
                if (includeTitleSpace && Page.TitleSpaceSize.RelativeWidth == null)
                    remainingWidth -= titleSpaceSize;

                double desiredSize;

                if (includeTitle && Page.TitleSize.RelativeWidth != null)
                {
                    desiredSize = remainingWidth * Page.TitleSize.RelativeWidth.Value / relativeTotal;
                    if (Page.TitleSize.MaxWidth != null)
                        desiredSize = Min(desiredSize, Page.TitleSize.MaxWidth.Value);
                    titleSize = Max(titleSize, desiredSize);
                }
                if (includeTitleControls && Page.TitleControlsSize.RelativeWidth != null)
                {
                    desiredSize = remainingWidth * Page.TitleControlsSize.RelativeWidth.Value / relativeTotal;
                    if (Page.TitleControlsSize.MaxWidth != null)
                        desiredSize = Min(desiredSize, Page.TitleControlsSize.MaxWidth.Value);
                    titleControlsSize = Max(titleControlsSize, desiredSize);
                }
                if (includeTitleSpace && Page.TitleSpaceSize.RelativeWidth != null)
                {
                    desiredSize = remainingWidth * Page.TitleSpaceSize.RelativeWidth.Value / relativeTotal;
                    if (Page.TitleSpaceSize.MaxWidth != null)
                        desiredSize = Min(desiredSize, Page.TitleSpaceSize.MaxWidth.Value);
                    titleSpaceSize = Max(titleSpaceSize, desiredSize);
                }

                if (includeTitle && Page.TitleSize.RelativeWidth != null)
                    remainingWidth -= titleSize;
                if (includeTitleControls && Page.TitleControlsSize.RelativeWidth != null)
                    remainingWidth -= titleControlsSize;
                if (includeTitleSpace && Page.TitleSpaceSize.RelativeWidth != null)
                    remainingWidth -= titleSpaceSize;

                //default priority: controls, title, space
                if (remainingWidth <= -1)
                {
                    if (includeTitleSpace)
                    {
                        if (includeTitle)
                        {
                            if (includeTitleControls)
                            {
                                #region controls, title, space
                                if (Page.TitleSpaceSize.Prioritize)
                                {
                                    if (Page.TitleSize.Prioritize)
                                    {
                                        if (Page.TitleControlsSize.Prioritize)
                                        {
                                            includeTitleSpace = false;
                                        }
                                        else
                                            includeTitleControls = false;
                                    }
                                    else
                                        includeTitle = false;
                                }
                                else
                                    includeTitleSpace = false;
                                #endregion
                            }
                            else
                            {
                                #region title, space
                                if (Page.TitleSpaceSize.Prioritize)
                                {
                                    if (Page.TitleSize.Prioritize)
                                    {
                                        includeTitleSpace = false;
                                    }
                                    else
                                        includeTitle = false;
                                }
                                else
                                    includeTitleSpace = false;
                                #endregion
                            }
                        }
                        else
                        {
                            if (includeTitleControls)
                            {
                                #region controls, space
                                if (Page.TitleSpaceSize.Prioritize)
                                {
                                    if (Page.TitleControlsSize.Prioritize)
                                    {
                                        includeTitleSpace = false;
                                    }
                                    else
                                        includeTitleControls = false;
                                }
                                else
                                    includeTitleSpace = false;
                                #endregion
                            }
                            else
                            {
                                #region space
                                includeTitleSpace = false;
                                #endregion
                            }
                        }
                    }
                    else
                    {
                        if (includeTitle)
                        {
                            if (includeTitleControls)
                            {
                                #region controls, title
                                if (Page.TitleSize.Prioritize)
                                {
                                    if (Page.TitleControlsSize.Prioritize)
                                    {
                                        includeTitleSpace = false;
                                    }
                                    else
                                        includeTitleControls = false;
                                }
                                else
                                    includeTitle = false;
                                #endregion
                            }
                            else
                            {
                                #region title
                                    includeTitle = false;
                                #endregion
                            }
                        }
                        else
                        {
                            if (includeTitleControls)
                            {
                                #region controls
                                includeTitleControls = false;
                                #endregion
                            }
                            else
                            {
                                remainingWidth = 0;
                            }
                        }
                    }
                }
            }
            #endregion

            TitleActualWidth = includeTitle ? titleSize : 0;
            TitleControlsActualWidth = includeTitleControls ? titleControlsSize : 0;
            TitleVisibility = includeTitle ? Visibility.Visible : Visibility.Collapsed;
            TitleControlsVisibility = includeTitleControls ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion

        #region Properties

        public Command GoBackCommand { get; }
        public Command NavigateBackCommand { get; }
        public Command NavigateForwardCommand { get; }
        public Command ToggleMenuCommand { get; }

        public ObservableRangeCollection<HamburgerBarPageEntryViewModel> PageEntries { get; } = new ObservableRangeCollection<HamburgerBarPageEntryViewModel>();

        private BaseViewModel _PageDataContext;
        public BaseViewModel PageDataContext
        {
            get { return _PageDataContext; }
            private set
            {
                SetProperty(ref _PageDataContext, value);
            }
        }

        private bool _IsMenuOpen = false;
        public bool IsMenuOpen
        {
            get { return _IsMenuOpen; }
            set
            {
                SetProperty(ref _IsMenuOpen, value);
            }
        }

        private DataTemplate _TitleTemplate;
        public DataTemplate TitleTemplate
        {
            get { return _TitleTemplate; }
            private set { SetProperty(ref _TitleTemplate, value); }
        }

        private DataTemplate _TitleControlsTemplate;
        public DataTemplate TitleControlsTemplate
        {
            get { return _TitleControlsTemplate; }
            private set { SetProperty(ref _TitleControlsTemplate, value); }
        }

        private Visibility _TitleVisibility;
        public Visibility TitleVisibility
        {
            get { return _TitleVisibility; }
            private set { SetProperty(ref _TitleVisibility, value); }
        }

        private Visibility _TitleControlsVisibility;
        public Visibility TitleControlsVisibility
        {
            get { return _TitleControlsVisibility; }
            private set { SetProperty(ref _TitleControlsVisibility, value); }
        }

        private double _TitleActualWidth;
        public double TitleActualWidth
        {
            get { return _TitleActualWidth; }
            set { SetProperty(ref _TitleActualWidth, value); }
        }

        private double _TitleControlsActualWidth;
        public double TitleControlsActualWidth
        {
            get { return _TitleControlsActualWidth; }
            set { SetProperty(ref _TitleControlsActualWidth, value); }
        }
        #endregion
    }
}
