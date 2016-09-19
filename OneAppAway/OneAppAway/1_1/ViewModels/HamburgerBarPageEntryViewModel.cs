using MvvmHelpers;
using OneAppAway._1_1.Views.Controls;
using OneAppAway.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace OneAppAway._1_1.ViewModels
{
    class HamburgerBarPageEntryViewModel : BaseViewModel
    {
        private ApplicationFrame Frame;
        private Type PageType;

        public HamburgerBarPageEntryViewModel(ApplicationFrame frame, Type pageType, string title, FontFamily iconFontFamily, double iconFontSize, string icon)
        {
            Frame = frame;
            PageType = pageType;
            Title = title;
            Icon = icon;
            IconFontFamily = iconFontFamily;
            IconFontSize = iconFontSize;
            Frame.Navigated += Frame_Navigated;
            NavigateCommand = new RelayCommand(TryNavigate);
        }

        private void Frame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            IsNavigated = CheckIsFrameNavigated();
        }

        private bool CheckIsFrameNavigated() => Frame.Content?.GetType().Equals(PageType) ?? false;

        private void TryNavigate(object parameter)
        {
            if (!CheckIsFrameNavigated())
                Frame.Navigate(PageType);
        }

        public RelayCommand NavigateCommand { get; }

        public FontFamily IconFontFamily { get; }
        public double IconFontSize { get; }

        private bool _IsNavigated = false;
        public bool IsNavigated
        {
            get { return _IsNavigated; }
            set
            {
                SetProperty(ref _IsNavigated, value);
            }
        }
    }
}
