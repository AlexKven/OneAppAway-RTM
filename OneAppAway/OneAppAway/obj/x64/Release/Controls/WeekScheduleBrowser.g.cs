﻿#pragma checksum "C:\Users\Alexander\Documents\GitHub\OneAppAway-RTM\OneAppAway\OneAppAway\Controls\WeekScheduleBrowser.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "B1CECD6B7F16B0840260E43510620AA2"
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
    partial class WeekScheduleBrowser : 
        global::Windows.UI.Xaml.Controls.UserControl, 
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
                    this.MainScheduleBrowser = (global::OneAppAway.ScheduleBrowser)(target);
                }
                break;
            case 2:
                {
                    this.LoadSchedulesButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 20 "..\..\..\Controls\WeekScheduleBrowser.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.LoadSchedulesButton).Click += this.LoadSchedulesButton_Click;
                    #line default
                }
                break;
            case 3:
                {
                    this.ScheduleNotAvailableBlock = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 4:
                {
                    this.CachedSchedulesButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 24 "..\..\..\Controls\WeekScheduleBrowser.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.CachedSchedulesButton).Click += this.LoadSchedulesButton_Click;
                    #line default
                }
                break;
            case 5:
                {
                    this.CannotConnectButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 27 "..\..\..\Controls\WeekScheduleBrowser.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.CannotConnectButton).Click += this.LoadSchedulesButton_Click;
                    #line default
                }
                break;
            case 6:
                {
                    this.DayScheduleSelector = (global::Windows.UI.Xaml.Controls.ComboBox)(target);
                    #line 30 "..\..\..\Controls\WeekScheduleBrowser.xaml"
                    ((global::Windows.UI.Xaml.Controls.ComboBox)this.DayScheduleSelector).SelectionChanged += this.DayScheduleSelector_SelectionChanged;
                    #line default
                }
                break;
            case 7:
                {
                    this.ScheduleProgressIndicator = (global::Windows.UI.Xaml.Controls.ProgressRing)(target);
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
