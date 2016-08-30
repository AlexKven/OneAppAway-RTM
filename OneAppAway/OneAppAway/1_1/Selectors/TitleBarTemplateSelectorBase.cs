using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace OneAppAway._1_1.Selectors
{
    public abstract class TitleBarTemplateSelectorBase : DependencyObject, INotifyPropertyChanged
    {
        private void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private DataTemplate _TitleTemplate;
        public DataTemplate TitleTemplate
        {
            get { return _TitleTemplate; }
            protected set
            {
                _TitleTemplate = value;
                OnPropertyChanged();
            }
        }

        private DataTemplate _ControlsTemplate;
        public DataTemplate ControlsTemplate
        {
            get { return _ControlsTemplate; }
            protected set
            {
                _ControlsTemplate = value;
                OnPropertyChanged();
            }
        }

        private DataTemplate _OverflowControlsTemplate;
        public DataTemplate OverflowControlsTemplate
        {
            get { return _OverflowControlsTemplate; }
            protected set
            {
                _OverflowControlsTemplate = value;
                OnPropertyChanged();
            }
        }

        private double _TitleWidth;
        public double TitleWidth
        {
            get { return _TitleWidth; }
            set
            {
                _TitleWidth = value;
                OnPropertyChanged();
            }
        }

        private double _ControlsWidth;
        public double ControlsWidth
        {
            get { return _ControlsWidth; }
            set
            {
                _ControlsWidth = value;
                OnPropertyChanged();
            }
        }

        private double _OverflowControlsWidth;
        public double OverflowControlsWidth
        {
            get { return _OverflowControlsWidth; }
            set
            {
                _OverflowControlsWidth = value;
                OnPropertyChanged();
            }
        }

        public abstract void ReceiveAvailableSize(double size, bool onMobile);
    }
}
