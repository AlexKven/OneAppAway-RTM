﻿#pragma checksum "C:\Users\Alexander\Documents\GitHub\OneAppAway-RTM\OneAppAway\OneAppAway\AboutPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "E1968DB11C4208FAB855D40D8E8131C4"
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
    partial class AboutPage : 
        global::Windows.UI.Xaml.Controls.Page, 
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
                    this.OuterGrid = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 2:
                {
                    this.AppWidthStateGroup = (global::Windows.UI.Xaml.VisualStateGroup)(target);
                    #line 12 "..\..\..\AboutPage.xaml"
                    ((global::Windows.UI.Xaml.VisualStateGroup)this.AppWidthStateGroup).CurrentStateChanging += this.AppWidthStateGroup_CurrentStateChanging;
                    #line default
                }
                break;
            case 3:
                {
                    this.NormalState = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 4:
                {
                    this.ThinState = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 5:
                {
                    this.OuterScrollViewer = (global::Windows.UI.Xaml.Controls.ScrollViewer)(target);
                }
                break;
            case 6:
                {
                    this.LeftScrollViewer = (global::Windows.UI.Xaml.Controls.ScrollViewer)(target);
                }
                break;
            case 7:
                {
                    this.RightScrollViewer = (global::Windows.UI.Xaml.Controls.ScrollViewer)(target);
                }
                break;
            case 8:
                {
                    this.RightPanel = (global::Windows.UI.Xaml.Controls.StackPanel)(target);
                }
                break;
            case 9:
                {
                    this.LeftPanel = (global::Windows.UI.Xaml.Controls.StackPanel)(target);
                }
                break;
            case 10:
                {
                    this.TitlePanel = (global::Windows.UI.Xaml.Controls.StackPanel)(target);
                }
                break;
            case 11:
                {
                    this.InnerPanel = (global::Windows.UI.Xaml.Controls.Grid)(target);
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

