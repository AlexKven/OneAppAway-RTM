﻿#pragma checksum "C:\GitHub\OneAppAway-RTM\OneAppAway\OneAppAway\1_1\Views\Controls\TransitMap.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "762D75A906096DA1592AD1686869E563"
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
    partial class TransitMap : 
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
                    global::Windows.UI.Xaml.Controls.UserControl element1 = (global::Windows.UI.Xaml.Controls.UserControl)(target);
                    #line 12 "..\..\..\..\..\1_1\Views\Controls\TransitMap.xaml"
                    ((global::Windows.UI.Xaml.Controls.UserControl)element1).Unloaded += this.UserControl_Unloaded;
                    #line 12 "..\..\..\..\..\1_1\Views\Controls\TransitMap.xaml"
                    ((global::Windows.UI.Xaml.Controls.UserControl)element1).Loaded += this.UserControl_Loaded;
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
                    this.MainMap = (global::Windows.UI.Xaml.Controls.Maps.MapControl)(target);
                    #line 14 "..\..\..\..\..\1_1\Views\Controls\TransitMap.xaml"
                    ((global::Windows.UI.Xaml.Controls.Maps.MapControl)this.MainMap).MapElementClick += this.MainMap_MapElementClick;
                    #line 14 "..\..\..\..\..\1_1\Views\Controls\TransitMap.xaml"
                    ((global::Windows.UI.Xaml.Controls.Maps.MapControl)this.MainMap).MapElementPointerEntered += this.MainMap_MapElementPointerEntered;
                    #line 14 "..\..\..\..\..\1_1\Views\Controls\TransitMap.xaml"
                    ((global::Windows.UI.Xaml.Controls.Maps.MapControl)this.MainMap).MapElementPointerExited += this.MainMap_MapElementPointerExited;
                    #line 14 "..\..\..\..\..\1_1\Views\Controls\TransitMap.xaml"
                    ((global::Windows.UI.Xaml.Controls.Maps.MapControl)this.MainMap).SizeChanged += this.MainMap_SizeChanged;
                    #line 14 "..\..\..\..\..\1_1\Views\Controls\TransitMap.xaml"
                    ((global::Windows.UI.Xaml.Controls.Maps.MapControl)this.MainMap).ZoomLevelChanged += this.MainMap_ZoomLevelChanged;
                    #line 14 "..\..\..\..\..\1_1\Views\Controls\TransitMap.xaml"
                    ((global::Windows.UI.Xaml.Controls.Maps.MapControl)this.MainMap).CenterChanged += this.MainMap_CenterChanged;
                    #line default
                }
                break;
            case 4:
                {
                    this.TakeoverOverlayControl = (global::Windows.UI.Xaml.Controls.ContentControl)(target);
                }
                break;
            case 5:
                {
                    this.OnMapPopup = (global::Windows.UI.Xaml.Controls.ContentControl)(target);
                }
                break;
            case 6:
                {
                    this.CoordsBlock = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 7:
                {
                    this.CoordsBlock2 = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 8:
                {
                    this.ZoomLevelBlock = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
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

