﻿#pragma checksum "C:\GitHub\OneAppAway-RTM\OneAppAway\OneAppAway\1_1\Views\Controls\RealTimeArrivalControl.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "9CBABA999DF739C81F4CF4C61D388811"
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
    partial class RealTimeArrivalControl : 
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
                    this.MainButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                }
                break;
            case 2:
                {
                    this.RouteNameLengthGroup = (global::Windows.UI.Xaml.VisualStateGroup)(target);
                }
                break;
            case 3:
                {
                    this.RouteFrequencyTypeGroup = (global::Windows.UI.Xaml.VisualStateGroup)(target);
                }
                break;
            case 4:
                {
                    this.FrequencyBasedArrivalState = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 5:
                {
                    this.LongRouteNameArrivalState = (global::Windows.UI.Xaml.VisualState)(target);
                }
                break;
            case 6:
                {
                    global::Windows.UI.Xaml.Controls.Flyout element6 = (global::Windows.UI.Xaml.Controls.Flyout)(target);
                    #line 41 "..\..\..\..\..\1_1\Views\Controls\RealTimeArrivalControl.xaml"
                    ((global::Windows.UI.Xaml.Controls.Flyout)element6).Opened += this.Flyout_Opened;
                    #line default
                }
                break;
            case 7:
                {
                    global::Windows.UI.Xaml.Controls.MenuFlyoutItem element7 = (global::Windows.UI.Xaml.Controls.MenuFlyoutItem)(target);
                    #line 49 "..\..\..\..\..\1_1\Views\Controls\RealTimeArrivalControl.xaml"
                    ((global::Windows.UI.Xaml.Controls.MenuFlyoutItem)element7).Click += this.FindVehicle_Click;
                    #line default
                }
                break;
            case 8:
                {
                    global::Windows.UI.Xaml.Controls.MenuFlyoutItem element8 = (global::Windows.UI.Xaml.Controls.MenuFlyoutItem)(target);
                    #line 50 "..\..\..\..\..\1_1\Views\Controls\RealTimeArrivalControl.xaml"
                    ((global::Windows.UI.Xaml.Controls.MenuFlyoutItem)element8).Click += this.FindVehicleScheduled_Click;
                    #line default
                }
                break;
            case 9:
                {
                    this.RouteNameRow = (global::Windows.UI.Xaml.Controls.ColumnDefinition)(target);
                }
                break;
            case 10:
                {
                    this.RingColumn = (global::Windows.UI.Xaml.Controls.ColumnDefinition)(target);
                }
                break;
            case 11:
                {
                    this.MinutesColumn = (global::Windows.UI.Xaml.Controls.ColumnDefinition)(target);
                }
                break;
            case 12:
                {
                    this.ShortRouteNameBlock = (global::OneAppAway._1_1.Views.Controls.AutoFitTextBlock)(target);
                }
                break;
            case 13:
                {
                    this.LongRouteNameBlock = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 14:
                {
                    this.MinutesBlock = (global::OneAppAway._1_1.Views.Controls.AutoFitTextBlock)(target);
                }
                break;
            case 15:
                {
                    this.PredictedArrivalTimeBlock = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 16:
                {
                    this.ScheduledArrivalTimeBlock = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 17:
                {
                    this.FrequencyWarningBlock = (global::OneAppAway._1_1.Views.Controls.AutoFitTextBlock)(target);
                }
                break;
            case 18:
                {
                    this.CancelImage = (global::Windows.UI.Xaml.Controls.Image)(target);
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

