﻿#pragma checksum "C:\GitHub\OneAppAway-RTM\OneAppAway\OneAppAway\1_1\Views\Pages\TestPage1.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "AFAAB7FD34D5AE8EFDEE29828154AAF5"
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
    partial class TestPage1 : 
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
                    this.TestImage = (global::Windows.UI.Xaml.Controls.Image)(target);
                }
                break;
            case 2:
                {
                    global::Windows.UI.Xaml.Controls.Button element2 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 15 "..\..\..\..\..\1_1\Views\Pages\TestPage1.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)element2).Click += this.Button_Click_1;
                    #line default
                }
                break;
            case 3:
                {
                    this.AngleSlider = (global::Windows.UI.Xaml.Controls.Slider)(target);
                    #line 16 "..\..\..\..\..\1_1\Views\Pages\TestPage1.xaml"
                    ((global::Windows.UI.Xaml.Controls.Slider)this.AngleSlider).ValueChanged += this.AngleSlider_ValueChanged;
                    #line default
                }
                break;
            case 4:
                {
                    this._Button = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 17 "..\..\..\..\..\1_1\Views\Pages\TestPage1.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this._Button).Click += this.Button_Click;
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

