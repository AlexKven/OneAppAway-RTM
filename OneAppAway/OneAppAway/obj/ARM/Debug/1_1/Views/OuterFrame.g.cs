﻿#pragma checksum "C:\GitHub\OneAppAway-RTM\OneAppAway\OneAppAway\1_1\Views\OuterFrame.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "AEB802761D3D3ADF5F7004ABD45087C6"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OneAppAway._1_1.Views
{
    partial class OuterFrame : 
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
                    global::Windows.UI.Xaml.Controls.Page element1 = (global::Windows.UI.Xaml.Controls.Page)(target);
                    #line 11 "..\..\..\..\1_1\Views\OuterFrame.xaml"
                    ((global::Windows.UI.Xaml.Controls.Page)element1).Loaded += this.Page_Loaded;
                    #line default
                }
                break;
            case 2:
                {
                    global::Windows.UI.Xaml.Controls.Grid element2 = (global::Windows.UI.Xaml.Controls.Grid)(target);
                    #line 12 "..\..\..\..\1_1\Views\OuterFrame.xaml"
                    ((global::Windows.UI.Xaml.Controls.Grid)element2).SizeChanged += this.Grid_SizeChanged;
                    #line default
                }
                break;
            case 3:
                {
                    this.WindowSizeStates = (global::Windows.UI.Xaml.VisualStateGroup)(target);
                }
                break;
            case 4:
                {
                    this.NarrowWindowState = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 5:
                {
                    this.NormalWindowState = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 6:
                {
                    this.MainSplitView = (global::Windows.UI.Xaml.Controls.SplitView)(target);
                }
                break;
            case 7:
                {
                    this.TitleCompensationColumn = (global::Windows.UI.Xaml.Controls.ColumnDefinition)(target);
                }
                break;
            case 8:
                {
                    this.SystemButtonsColumn2 = (global::Windows.UI.Xaml.Controls.ColumnDefinition)(target);
                }
                break;
            case 9:
                {
                    this.TitleOverlay = (global::Windows.UI.Xaml.Controls.ContentControl)(target);
                }
                break;
            case 10:
                {
                    this.HamburgerBar = (global::Windows.UI.Xaml.Controls.StackPanel)(target);
                }
                break;
            case 11:
                {
                    this.MapButton = (global::Windows.UI.Xaml.Controls.RadioButton)(target);
                    #line 92 "..\..\..\..\1_1\Views\OuterFrame.xaml"
                    ((global::Windows.UI.Xaml.Controls.RadioButton)this.MapButton).Checked += this.MapButton_Checked;
                    #line default
                }
                break;
            case 12:
                {
                    this.RoutesButton = (global::Windows.UI.Xaml.Controls.RadioButton)(target);
                    #line 103 "..\..\..\..\1_1\Views\OuterFrame.xaml"
                    ((global::Windows.UI.Xaml.Controls.RadioButton)this.RoutesButton).Checked += this.RoutesButton_Checked;
                    #line default
                }
                break;
            case 13:
                {
                    this.SettingsButton = (global::Windows.UI.Xaml.Controls.RadioButton)(target);
                    #line 114 "..\..\..\..\1_1\Views\OuterFrame.xaml"
                    ((global::Windows.UI.Xaml.Controls.RadioButton)this.SettingsButton).Checked += this.SettingsButton_Checked;
                    #line default
                }
                break;
            case 14:
                {
                    this.AboutButton = (global::Windows.UI.Xaml.Controls.RadioButton)(target);
                    #line 125 "..\..\..\..\1_1\Views\OuterFrame.xaml"
                    ((global::Windows.UI.Xaml.Controls.RadioButton)this.AboutButton).Checked += this.AboutButton_Checked;
                    #line default
                }
                break;
            case 15:
                {
                    this.TitleBar = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 16:
                {
                    global::Windows.UI.Xaml.Controls.Button element16 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 57 "..\..\..\..\1_1\Views\OuterFrame.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)element16).Click += this.HamburgerButton_Click;
                    #line default
                }
                break;
            case 17:
                {
                    this.SystemButtonsColumn = (global::Windows.UI.Xaml.Controls.ColumnDefinition)(target);
                }
                break;
            case 18:
                {
                    this.TitleContent = (global::Windows.UI.Xaml.Controls.ContentControl)(target);
                }
                break;
            case 19:
                {
                    this.TitleElement = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
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
