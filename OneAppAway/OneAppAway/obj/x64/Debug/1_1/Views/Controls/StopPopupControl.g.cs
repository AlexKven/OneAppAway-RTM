﻿#pragma checksum "C:\GitHub\OneAppAway-RTM\OneAppAway\OneAppAway\1_1\Views\Controls\StopPopupControl.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "72B02B97ACDEA94211B3CCCF41F85928"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OneAppAway._1_1.Views.Controls
{
    partial class StopPopupControl : 
        global::OneAppAway._1_1.Views.Controls.StopPopupControlBase, 
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
                    this.self = (global::OneAppAway._1_1.Views.Controls.StopPopupControlBase)(target);
                    #line 13 "..\..\..\..\..\1_1\Views\Controls\StopPopupControl.xaml"
                    ((global::OneAppAway._1_1.Views.Controls.StopPopupControlBase)this.self).DataContextChanged += this.StopArrivalsControlBase_DataContextChanged;
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
                    this.TitleButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                }
                break;
            case 4:
                {
                    this.ScheduleViewer = (global::OneAppAway._1_1.Views.Controls.ScheduleControl)(target);
                }
                break;
            case 5:
                {
                    this.ArrivalsBox = (global::OneAppAway._1_1.Views.Controls.StopArrivalsBox)(target);
                }
                break;
            case 6:
                {
                    this.RefreshButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 51 "..\..\..\..\..\1_1\Views\Controls\StopPopupControl.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.RefreshButton).Click += this.RefreshButton_Click;
                    #line default
                }
                break;
            case 7:
                {
                    this.CompressButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                }
                break;
            case 8:
                {
                    this.ExpandButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                }
                break;
            case 9:
                {
                    this.CloseButton = (global::Windows.UI.Xaml.Controls.Button)(target);
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

