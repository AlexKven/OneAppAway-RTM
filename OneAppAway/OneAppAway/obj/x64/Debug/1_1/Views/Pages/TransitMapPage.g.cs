﻿#pragma checksum "C:\GitHub\OneAppAway-RTM\OneAppAway\OneAppAway\1_1\Views\Pages\TransitMapPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "CBE0EC9B66D28C4544B7EC4E0D610E93"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OneAppAway._1_1.Views.Pages
{
    partial class TransitMapPage : 
        global::OneAppAway._1_1.Views.Pages.ApplicationPage, 
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
                    this.Self = (global::OneAppAway._1_1.Views.Pages.ApplicationPage)(target);
                }
                break;
            case 2:
                {
                    global::Windows.UI.Xaml.Controls.Grid element2 = (global::Windows.UI.Xaml.Controls.Grid)(target);
                    #line 15 "..\..\..\..\..\1_1\Views\Pages\TransitMapPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Grid)element2).Loaded += this.Grid_Loaded;
                    #line default
                }
                break;
            case 3:
                {
                    this.MainGrid = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 4:
                {
                    this.MainMap = (global::OneAppAway._1_1.Views.Controls.TransitMap)(target);
                }
                break;
            case 5:
                {
                    this.LoadingIndicator = (global::Windows.UI.Xaml.Controls.ProgressBar)(target);
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

