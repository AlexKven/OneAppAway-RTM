﻿#pragma checksum "C:\Users\Alexander\Documents\GitHub\OneAppAway-RTM\OneAppAway\OneAppAway\Pages\BusMapPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "292A6BAB91B6004024575FF37D9B7DDF"
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
    partial class BusMapPage : 
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
                    #line 10 "..\..\..\Pages\BusMapPage.xaml"
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
                    this.ArrivalBoxVisualStates = (global::Windows.UI.Xaml.VisualStateGroup)(target);
                    #line 13 "..\..\..\Pages\BusMapPage.xaml"
                    ((global::Windows.UI.Xaml.VisualStateGroup)this.ArrivalBoxVisualStates).CurrentStateChanged += this.ArrivalBoxVisualStates_CurrentStateChanged;
                    #line 13 "..\..\..\Pages\BusMapPage.xaml"
                    ((global::Windows.UI.Xaml.VisualStateGroup)this.ArrivalBoxVisualStates).CurrentStateChanging += this.ArrivalBoxVisualStates_CurrentStateChanging;
                    #line default
                }
                break;
            case 4:
                {
                    this.WindowSizeVisualStates = (global::Windows.UI.Xaml.VisualStateGroup)(target);
                    #line 36 "..\..\..\Pages\BusMapPage.xaml"
                    ((global::Windows.UI.Xaml.VisualStateGroup)this.WindowSizeVisualStates).CurrentStateChanging += this.WindowSizeVisualStates_CurrentStateChanging;
                    #line default
                }
                break;
            case 5:
                {
                    this.NormalState = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 6:
                {
                    this.NarrowState = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 7:
                {
                    this.ArrivalBoxShown = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 8:
                {
                    this.ArrivalBoxHidden = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 9:
                {
                    this.MainMap = (global::OneAppAway.BusMap)(target);
                    #line 53 "..\..\..\Pages\BusMapPage.xaml"
                    ((global::OneAppAway.BusMap)this.MainMap).StopsClicked += this.MainMap_StopsClicked;
                    #line 53 "..\..\..\Pages\BusMapPage.xaml"
                    ((global::OneAppAway.BusMap)this.MainMap).PropertyChanged += this.BusMap_PropertyChanged;
                    #line default
                }
                break;
            case 10:
                {
                    this.ButtonsPanel = (global::Windows.UI.Xaml.Controls.StackPanel)(target);
                }
                break;
            case 11:
                {
                    this.StopArrivalBox = (global::OneAppAway.PopupStopArrivalsBox)(target);
                    #line 77 "..\..\..\Pages\BusMapPage.xaml"
                    ((global::OneAppAway.PopupStopArrivalsBox)this.StopArrivalBox).CloseRequested += this.StopArrivalBox_CloseRequested;
                    #line default
                }
                break;
            case 12:
                {
                    this.LoadingIndicator = (global::Windows.UI.Xaml.Controls.ProgressBar)(target);
                }
                break;
            case 13:
                {
                    this.StopArrivalBoxTranslation = (global::Windows.UI.Xaml.Media.CompositeTransform)(target);
                }
                break;
            case 14:
                {
                    this.ZoomInButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 64 "..\..\..\Pages\BusMapPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.ZoomInButton).Click += this.ZoomInButton_Click;
                    #line default
                }
                break;
            case 15:
                {
                    this.ZoomOutButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 67 "..\..\..\Pages\BusMapPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.ZoomOutButton).Click += this.ZoomOutButton_Click;
                    #line default
                }
                break;
            case 16:
                {
                    global::Windows.UI.Xaml.Controls.Button element16 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 70 "..\..\..\Pages\BusMapPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)element16).Click += this.CenterButton_Click;
                    #line default
                }
                break;
            case 17:
                {
                    this.RefreshButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 73 "..\..\..\Pages\BusMapPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.RefreshButton).Click += this.RefreshButton_Click;
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

