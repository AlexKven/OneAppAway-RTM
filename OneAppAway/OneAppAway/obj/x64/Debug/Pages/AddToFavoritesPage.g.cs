﻿#pragma checksum "C:\Users\Alexander\Documents\GitHub\OneAppAway-RTM\OneAppAway\OneAppAway\Pages\AddToFavoritesPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "1252B971E19BB35C19A7F2B2C1CE8057"
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
    partial class AddToFavoritesPage : 
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
                    this.MainGrid = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 2:
                {
                    this.DescriptionBlock = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 3:
                {
                    this.TitleBox = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 4:
                {
                    global::Windows.UI.Xaml.Controls.Button element4 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 75 "..\..\..\Pages\AddToFavoritesPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)element4).Click += this.SaveButton_Click;
                    #line default
                }
                break;
            case 5:
                {
                    global::Windows.UI.Xaml.Controls.Button element5 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 76 "..\..\..\Pages\AddToFavoritesPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)element5).Click += this.CancelButton_Click;
                    #line default
                }
                break;
            case 6:
                {
                    this.CityContextBox = (global::Windows.UI.Xaml.Controls.CheckBox)(target);
                    #line 31 "..\..\..\Pages\AddToFavoritesPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.CheckBox)this.CityContextBox).Checked += this.CityContextBox_Checked;
                    #line 31 "..\..\..\Pages\AddToFavoritesPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.CheckBox)this.CityContextBox).Unchecked += this.CityContextBox_Unchecked;
                    #line default
                }
                break;
            case 7:
                {
                    this.MileContextBox = (global::Windows.UI.Xaml.Controls.CheckBox)(target);
                    #line 32 "..\..\..\Pages\AddToFavoritesPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.CheckBox)this.MileContextBox).Checked += this.MileContextBox_Checked;
                    #line 32 "..\..\..\Pages\AddToFavoritesPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.CheckBox)this.MileContextBox).Unchecked += this.MileContextBox_Unchecked;
                    #line default
                }
                break;
            case 8:
                {
                    this.DirectionContextBox = (global::Windows.UI.Xaml.Controls.CheckBox)(target);
                    #line 38 "..\..\..\Pages\AddToFavoritesPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.CheckBox)this.DirectionContextBox).Checked += this.DirectionContextBox_Checked;
                    #line 38 "..\..\..\Pages\AddToFavoritesPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.CheckBox)this.DirectionContextBox).Unchecked += this.DirectionContextBox_Unchecked;
                    #line default
                }
                break;
            case 9:
                {
                    this.CardinalDirectionSelector = (global::Windows.UI.Xaml.Controls.ComboBox)(target);
                    #line 40 "..\..\..\Pages\AddToFavoritesPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.ComboBox)this.CardinalDirectionSelector).SelectionChanged += this.CardinalDirectionSelector_SelectionChanged;
                    #line default
                }
                break;
            case 10:
                {
                    this.MileContextDescription = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 11:
                {
                    this.MileSlider = (global::Windows.UI.Xaml.Controls.Slider)(target);
                    #line 35 "..\..\..\Pages\AddToFavoritesPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Slider)this.MileSlider).ValueChanged += this.MileSlider_ValueChanged;
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

