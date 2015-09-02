using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace OneAppAway
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HelpTip : Page
    {
        public HelpTip()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Tuple<AnimationDirection, Thickness, string>)
            {
                Tuple<AnimationDirection, Thickness, string> typedParam = (Tuple<AnimationDirection, Thickness, string>)e.Parameter;
                TopArrow.Visibility = typedParam.Item1 == AnimationDirection.Top ? Visibility.Visible : Visibility.Collapsed;
                BottomArrow.Visibility = typedParam.Item1 == AnimationDirection.Bottom ? Visibility.Visible : Visibility.Collapsed;
                LeftArrow.Visibility = typedParam.Item1 == AnimationDirection.Left ? Visibility.Visible : Visibility.Collapsed;
                RightArrow.Visibility = typedParam.Item1 == AnimationDirection.Right ? Visibility.Visible : Visibility.Collapsed;

                TopMargin.Height = new GridLength(typedParam.Item1 == AnimationDirection.Top ? 65 : 0);
                BottomMargin.Height = new GridLength(typedParam.Item1 == AnimationDirection.Bottom ? 65 : 0);
                LeftMargin.Width = new GridLength(typedParam.Item1 == AnimationDirection.Left ? 65 : 0);
                RightMargin.Width = new GridLength(typedParam.Item1 == AnimationDirection.Right ? 65 : 0);

                TopOverlap.Height = new GridLength(typedParam.Item1 == AnimationDirection.Top ? 20 : 0);
                BottomOverlap.Height = new GridLength(typedParam.Item1 == AnimationDirection.Bottom ? 20 : 0);
                LeftOverlap.Width = new GridLength(typedParam.Item1 == AnimationDirection.Left ? 20 : 0);
                RightOverlap.Width = new GridLength(typedParam.Item1 == AnimationDirection.Right ? 20 : 0);

                switch (typedParam.Item1)
                {
                    case AnimationDirection.Top:
                        TopArrow.Margin = typedParam.Item2;
                        break;
                    case AnimationDirection.Bottom:
                        BottomArrow.Margin = typedParam.Item2;
                        break;
                    case AnimationDirection.Left:
                        LeftArrow.Margin = typedParam.Item2;
                        break;
                    case AnimationDirection.Right:
                        RightArrow.Margin = typedParam.Item2;
                        break;
                }

                MainText.Text = typedParam.Item3;
            }
            base.OnNavigatedTo(e);
        }
    }
}
