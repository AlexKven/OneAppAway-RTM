using OneAppAway._1_1.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SQLitePCL;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace OneAppAway._1_1.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BusMapPage : Page
    {
        public BusMapPage()
        {
            this.InitializeComponent();
            //MainMap.CenterRegion = new RectSubset() { Left = 300, LeftValueType = RectSubsetValueType.Length, Top = 100, Bottom = 150, Right = 200 };
            MainMap.CenterRegion = new RectSubset() { Left = 200, LeftValueType = RectSubsetValueType.Length };
            
        }
    }
}
