﻿#pragma checksum "C:\Users\Alexander\Documents\GitHub\OneAppAway-RTM\OneAppAway\OneAppAway\Pages\RouteViewPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "2E0E3D4306F2D267D0F041B1664B7D47"
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
    partial class RouteViewPage : 
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
                    this.MainGrid = (global::Windows.UI.Xaml.Controls.Grid)(target);
                    #line 10 "..\..\..\Pages\RouteViewPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Grid)this.MainGrid).SizeChanged += this.MainGrid_SizeChanged;
                    #line default
                }
                break;
            case 2:
                {
                    this.DisplayStates = (global::Windows.UI.Xaml.VisualStateGroup)(target);
                    #line 12 "..\..\..\Pages\RouteViewPage.xaml"
                    ((global::Windows.UI.Xaml.VisualStateGroup)this.DisplayStates).CurrentStateChanged += this.DisplayStates_CurrentStateChanged;
                    #line default
                }
                break;
            case 3:
                {
                    this.MapState = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 4:
                {
                    this.ArrivalsStateNormal = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 5:
                {
                    this.ArrivalsStateThin = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 6:
                {
                    this.MapColumn = (global::Windows.UI.Xaml.Controls.ColumnDefinition)(target);
                }
                break;
            case 7:
                {
                    this.ArrivalsColumn = (global::Windows.UI.Xaml.Controls.ColumnDefinition)(target);
                }
                break;
            case 8:
                {
                    this.MapGrid = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 9:
                {
                    this.ArrivalsBox = (global::OneAppAway.MultiStopArrivalsBox)(target);
                }
                break;
            case 10:
                {
                    this.RouteNameBlock = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 11:
                {
                    this.MainMap = (global::OneAppAway.BusMap)(target);
                    #line 54 "..\..\..\Pages\RouteViewPage.xaml"
                    ((global::OneAppAway.BusMap)this.MainMap).StopsClicked += this.MainMap_StopsClicked;
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

