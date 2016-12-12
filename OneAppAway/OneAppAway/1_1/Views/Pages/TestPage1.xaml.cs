using MvvmHelpers;
using OneAppAway._1_1.Data;
using OneAppAway._1_1.Imaging;
using OneAppAway._1_1.Views.Structures;
using OneAppAway.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using static System.Math;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace OneAppAway._1_1.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TestPage1 : ApplicationPage
    {
        private TransformedSprite TestTransformedSprite;
        private SpriteBase TestSprite;

        public TestPage1()
        {
            this.InitializeComponent();
            DataContext = new BaseViewModel() { Title = "Developer Page" };


            //MainVehicleDetailControl.Value = PugetSoundVehicleDetailSource.Instance.GetVehicleDetails(new RealTimeArrival() { Vehicle = "1_7180" });
        }

        private static void TestAction(TestPage1 target, object obj)
        {
            target.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ShapeDesignerPage)); Window.Current.CoreWindow.GetKeyState(Windows.System.VirtualKey.Control);
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Color accentColor = ((Color)App.Current.Resources["SystemColorControlAccentColor"]);
            TestTransformedSprite = new TransformedSprite() { AppliedSprite = new Sprite() { ImageUri = new Uri("ms-appx:///Assets/Icons/BusArrow.png") }, RelativeTransformOrigin = new Point(0.5, 0.473) };
            TestSprite = new TransformedSprite() { AppliedSprite = new CompositeSprite(new Sprite() { ImageUri = new Uri("ms-appx:///Assets/Icons/BusBase.png") }, TestTransformedSprite), Transform = new ScaleTransform() { /*ScaleX = .234375 * 1 / 3, ScaleY = .234375 * 1 / 3*/ } };
            //TestSprite = new CompositeSprite(new Sprite() { ImageUri = new Uri("ms-appx:///Assets/Icons/BusBase.png") }, TestTransformedSprite);
            await TestSprite.Load();
            TestSprite.Unlock();
            SetImageRotation();
        }

        private void SetImageRotation()
        {
            if (TestSprite?.IsLoaded ?? false)
            {
                TestTransformedSprite.Transform = new RotateTransform() { Angle = AngleSlider.Value }; // new RotateTransform() { Angle = AngleSlider.Value }; //new RotateTransform() { Angle = AngleSlider.Value };
                TestSprite.Render();
                TestImage.Source = TestSprite.Bitmap;
            }
        }

        private void AngleSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SetImageRotation();
        }
    }
}
