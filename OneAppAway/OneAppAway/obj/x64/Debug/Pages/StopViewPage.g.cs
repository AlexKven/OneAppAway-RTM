﻿#pragma checksum "C:\Users\Alexander\Documents\GitHub\OneAppAway-RTM\OneAppAway\OneAppAway\Pages\StopViewPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "F4F668F8D30C66C469A7A7AF1BBCE731"
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
    partial class StopViewPage : 
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
                    #line 11 "..\..\..\Pages\StopViewPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Grid)this.MainGrid).SizeChanged += this.MainGrid_SizeChanged;
                    #line default
                }
                break;
            case 2:
                {
                    this.InnerGrid = (global::Windows.UI.Xaml.Controls.Grid)(target);
                    #line 13 "..\..\..\Pages\StopViewPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Grid)this.InnerGrid).SizeChanged += this.InnerGrid_SizeChanged;
                    #line default
                }
                break;
            case 3:
                {
                    this.ArrivalsColumn = (global::Windows.UI.Xaml.Controls.ColumnDefinition)(target);
                }
                break;
            case 4:
                {
                    this.ScheduleColumn = (global::Windows.UI.Xaml.Controls.ColumnDefinition)(target);
                }
                break;
            case 5:
                {
                    this.RoutesColumn = (global::Windows.UI.Xaml.Controls.ColumnDefinition)(target);
                }
                break;
            case 6:
                {
                    this.MainMap = (global::OneAppAway.BusMap)(target);
                }
                break;
            case 7:
                {
                    this.ArrivalsBox = (global::OneAppAway.StopArrivalsBox)(target);
                }
                break;
            case 8:
                {
                    this.ArrivalsProgressIndicator = (global::Windows.UI.Xaml.Controls.ProgressRing)(target);
                }
                break;
            case 9:
                {
                    this.MainScheduleBrowser = (global::OneAppAway.WeekScheduleBrowser)(target);
                }
                break;
            case 10:
                {
                    this.RoutesProgressIndicator = (global::Windows.UI.Xaml.Controls.ProgressRing)(target);
                }
                break;
            case 11:
                {
                    this.RoutesControl = (global::Windows.UI.Xaml.Controls.ItemsControl)(target);
                }
                break;
            case 12:
                {
                    global::Windows.UI.Xaml.Controls.Button element12 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 47 "..\..\..\Pages\StopViewPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)element12).Click += this.RouteButton_Click;
                    #line default
                }
                break;
            case 13:
                {
                    global::Windows.UI.Xaml.Controls.Button element13 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 66 "..\..\..\Pages\StopViewPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)element13).Click += this.RouteButton_Click;
                    #line default
                }
                break;
            case 14:
                {
                    this.RefreshArrivalsButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 33 "..\..\..\Pages\StopViewPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.RefreshArrivalsButton).Click += this.RefreshArrivalsButton_Click;
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

