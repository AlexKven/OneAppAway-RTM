﻿#pragma checksum "C:\Users\kvenvold\Desktop\Watt Crunchers\Alex\GitHub\OneAppAway-RTM\OneAppAway\OneAppAway\Pages\RoutesPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "9CA16D7B336B5B6A5FCCD39E8643FF2F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OneAppAway
{
    partial class RoutesPage : 
        global::OneAppAway.NavigationFriendlyPage, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                {
                    global::OneAppAway.NavigationFriendlyPage element1 = (global::OneAppAway.NavigationFriendlyPage)(target);
                    #line 8 "..\..\..\Pages\RoutesPage.xaml"
                    ((global::OneAppAway.NavigationFriendlyPage)element1).SizeChanged += this.Page_SizeChanged;
                    #line default
                }
                break;
            case 2:
                {
                    this.MainGrid = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 3:
                {
                    this.DownloadingStates = (global::Windows.UI.Xaml.VisualStateGroup)(target);
                    #line 11 "..\..\..\Pages\RoutesPage.xaml"
                    ((global::Windows.UI.Xaml.VisualStateGroup)this.DownloadingStates).CurrentStateChanging += this.DownloadingStates_CurrentStateChanging;
                    #line 11 "..\..\..\Pages\RoutesPage.xaml"
                    ((global::Windows.UI.Xaml.VisualStateGroup)this.DownloadingStates).CurrentStateChanged += this.DownloadingStates_CurrentStateChanged;
                    #line default
                }
                break;
            case 4:
                {
                    this.SelectionStates = (global::Windows.UI.Xaml.VisualStateGroup)(target);
                    #line 15 "..\..\..\Pages\RoutesPage.xaml"
                    ((global::Windows.UI.Xaml.VisualStateGroup)this.SelectionStates).CurrentStateChanged += this.SelectionStates_CurrentStateChanged;
                    #line default
                }
                break;
            case 5:
                {
                    this.NotSelectingState = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 6:
                {
                    this.SelectingState = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 7:
                {
                    this.NotDownloadingState = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 8:
                {
                    this.DownloadingState = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 9:
                {
                    this.AgencyBar = (global::Windows.UI.Xaml.Controls.ScrollViewer)(target);
                }
                break;
            case 10:
                {
                    this.WarningBlock = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 11:
                {
                    this.MainStatusBar = (global::Windows.UI.Xaml.Controls.RelativePanel)(target);
                }
                break;
            case 12:
                {
                    this.LoadingRect = (global::Windows.UI.Xaml.Shapes.Rectangle)(target);
                }
                break;
            case 13:
                {
                    this.MainProgressRing = (global::Windows.UI.Xaml.Controls.ProgressRing)(target);
                }
                break;
            case 14:
                {
                    this.CheckToggle = (global::Windows.UI.Xaml.Controls.AppBarToggleButton)(target);
                    #line 65 "..\..\..\Pages\RoutesPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarToggleButton)this.CheckToggle).Checked += this.CheckToggle_Checked;
                    #line 65 "..\..\..\Pages\RoutesPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarToggleButton)this.CheckToggle).Unchecked += this.CheckToggle_Unchecked;
                    #line default
                }
                break;
            case 15:
                {
                    this.SelectAllButton = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    #line 66 "..\..\..\Pages\RoutesPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.SelectAllButton).Click += this.SelectAllButton_Click;
                    #line default
                }
                break;
            case 16:
                {
                    this.DownloadButton = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    #line 67 "..\..\..\Pages\RoutesPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.DownloadButton).Click += this.DownloadButton_Click;
                    #line default
                }
                break;
            case 17:
                {
                    this.DeleteButton = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    #line 68 "..\..\..\Pages\RoutesPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.DeleteButton).Click += this.DeleteButton_Click;
                    #line default
                }
                break;
            case 18:
                {
                    this.PauseButton = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    #line 69 "..\..\..\Pages\RoutesPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.PauseButton).Click += this.PauseButton_Click;
                    #line default
                }
                break;
            case 19:
                {
                    this.CancelButton = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    #line 70 "..\..\..\Pages\RoutesPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.CancelButton).Click += this.CancelButton_Click;
                    #line default
                }
                break;
            case 20:
                {
                    this.InvertSelectionButton = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    #line 72 "..\..\..\Pages\RoutesPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.InvertSelectionButton).Click += this.InvertSelectionButton_Click;
                    #line default
                }
                break;
            case 21:
                {
                    this.SelectDownloadedButton = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    #line 73 "..\..\..\Pages\RoutesPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.SelectDownloadedButton).Click += this.SelectDownloadedButton_Click;
                    #line default
                }
                break;
            case 22:
                {
                    this.SelectNotDownloadedButton = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    #line 74 "..\..\..\Pages\RoutesPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.SelectNotDownloadedButton).Click += this.SelectNotDownloadedButton_Click;
                    #line default
                }
                break;
            case 23:
                {
                    this.SelectDownloadingButton = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    #line 75 "..\..\..\Pages\RoutesPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.SelectDownloadingButton).Click += this.SelectDownloadingButton_Click;
                    #line default
                }
                break;
            case 24:
                {
                    this.StatusBlock = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 25:
                {
                    this.MasterProgressBar = (global::Windows.UI.Xaml.Controls.ProgressBar)(target);
                }
                break;
            case 26:
                {
                    this.MainList = (global::Windows.UI.Xaml.Controls.ItemsControl)(target);
                }
                break;
            case 27:
                {
                    global::OneAppAway.RouteListingControl element27 = (global::OneAppAway.RouteListingControl)(target);
                    #line 47 "..\..\..\Pages\RoutesPage.xaml"
                    ((global::OneAppAway.RouteListingControl)element27).Click += this.Route_Clicked;
                    #line default
                }
                break;
            case 28:
                {
                    this.AgenciesListView = (global::Windows.UI.Xaml.Controls.ListView)(target);
                    #line 28 "..\..\..\Pages\RoutesPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.ListView)this.AgenciesListView).SelectionChanged += this.AgenciesListView_SelectionChanged;
                    #line default
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

