﻿#pragma checksum "C:\GitHub\OneAppAway-RTM\OneAppAway\OneAppAway\Controls\HamburgerBar.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "056EFF6887FD198C6A5B83EE63224632"
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
    partial class HamburgerBar : 
        global::Windows.UI.Xaml.Controls.ContentControl, 
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
                    global::Windows.UI.Xaml.Controls.Primitives.Popup element1 = (global::Windows.UI.Xaml.Controls.Primitives.Popup)(target);
                    #line 103 "..\..\..\Controls\HamburgerBar.xaml"
                    ((global::Windows.UI.Xaml.Controls.Primitives.Popup)element1).Opened += this.PopupControl_Opened;
                    #line 103 "..\..\..\Controls\HamburgerBar.xaml"
                    ((global::Windows.UI.Xaml.Controls.Primitives.Popup)element1).Closed += this.PopupControl_Closed;
                    #line default
                }
                break;
            case 2:
                {
                    global::Windows.UI.Xaml.Controls.Button element2 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 56 "..\..\..\Controls\HamburgerBar.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)element2).Click += this.HamburgerButton_Click;
                    #line default
                }
                break;
            case 3:
                {
                    global::Windows.UI.Xaml.Controls.Button element3 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 57 "..\..\..\Controls\HamburgerBar.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)element3).Click += this.BandwidthButton_Click;
                    #line default
                }
                break;
            case 4:
                {
                    global::Windows.UI.Xaml.Controls.Button element4 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 126 "..\..\..\Controls\HamburgerBar.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)element4).Click += this.BandwidthButton_Click;
                    #line default
                }
                break;
            case 5:
                {
                    global::Windows.UI.Xaml.Controls.RadioButton element5 = (global::Windows.UI.Xaml.Controls.RadioButton)(target);
                    #line 151 "..\..\..\Controls\HamburgerBar.xaml"
                    ((global::Windows.UI.Xaml.Controls.RadioButton)element5).Checked += this.MapButton_Checked;
                    #line default
                }
                break;
            case 6:
                {
                    global::Windows.UI.Xaml.Controls.RadioButton element6 = (global::Windows.UI.Xaml.Controls.RadioButton)(target);
                    #line 162 "..\..\..\Controls\HamburgerBar.xaml"
                    ((global::Windows.UI.Xaml.Controls.RadioButton)element6).Checked += this.RoutesButton_Checked;
                    #line default
                }
                break;
            case 7:
                {
                    global::Windows.UI.Xaml.Controls.RadioButton element7 = (global::Windows.UI.Xaml.Controls.RadioButton)(target);
                    #line 173 "..\..\..\Controls\HamburgerBar.xaml"
                    ((global::Windows.UI.Xaml.Controls.RadioButton)element7).Checked += this.SettingsButton_Checked;
                    #line default
                }
                break;
            case 8:
                {
                    global::Windows.UI.Xaml.Controls.RadioButton element8 = (global::Windows.UI.Xaml.Controls.RadioButton)(target);
                    #line 184 "..\..\..\Controls\HamburgerBar.xaml"
                    ((global::Windows.UI.Xaml.Controls.RadioButton)element8).Checked += this.AboutButton_Checked;
                    #line default
                }
                break;
            case 9:
                {
                    global::Windows.UI.Xaml.Controls.Button element9 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 119 "..\..\..\Controls\HamburgerBar.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)element9).Click += this.HamburgerButton_Click;
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

