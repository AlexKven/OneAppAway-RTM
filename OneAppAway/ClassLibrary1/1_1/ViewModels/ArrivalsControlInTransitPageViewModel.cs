using MvvmHelpers;
using OneAppAway._1_1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OneAppAway._1_1.ViewModels
{
    public class ArrivalsControlInTransitPageViewModel : BaseViewModel
    {
        const double COLUMN_SIZE = 310;
        const double MAXIMIZED_MAP_MARGIN = 50;
        const double ARRIVALS_CONTROL_TRIANGLE_HEIGHT = 30;
        const double NORMAL_HEIGHT = 400;

        private double MapWidth;
        private double MapHeight;
        private double NumColsRequested = 1;
        private double MaxColsVisible
        {
            get
            {
                return Math.Floor(Math.Max(0, 2 * (MapWidth - 100 - COLUMN_SIZE / 2) / COLUMN_SIZE)) / 2 + 1;
            }
        }
        bool Maximized = false;

        public ArrivalsControlInTransitPageViewModel()
        {
            ExpandCommand = new Command((obj) =>
            {
                NumColsRequested += .5;
                SetSize(MapWidth, MapHeight);
            }, (obj) => IsExpandEnabled);
            CompressCommand = new Command((obj) =>
            {
                do
                {
                    NumColsRequested -= .5;
                } while (NumColsRequested >= MaxColsVisible && NumColsRequested > 1);
                SetSize(MapWidth, MapHeight);
            }, (obj) => IsCompressEnabled);
            CloseCommand = new Command((obj) => { Stop = null; SetVisibility(); });
        }

        public ICommand ExpandCommand { get; }
        public ICommand CompressCommand { get; }
        public ICommand CloseCommand { get; }

        private void Maximize()
        {
            Maximized = true;
            IsOnMap = false;
            SetCenterRegion();
        }

        private void Restore()
        {
            Maximized = false;
            IsOnMap = true;
            SetCenterRegion();
        }

        private void SetCenterRegion()
        {
            if (Stop.HasValue)
            {
                if (Maximized)
                    CenterRegion = new RectSubset() { Top = MAXIMIZED_MAP_MARGIN, TopValueType = RectSubsetValueType.Length };
                else
                    CenterRegion = new RectSubset() { Top = 0.2, TopValueType = RectSubsetValueType.Length, TopScale = RectSubsetScale.Relative };
            }
            else
                CenterRegion = new RectSubset();
        }

        public void SetSize(double mapWidth, double mapHeight)
        {
            MapWidth = mapWidth;
            MapHeight = mapHeight;
            if (NumColsRequested >= MaxColsVisible && !Maximized)
                Maximize();
            if (NumColsRequested < MaxColsVisible && Maximized)
                Restore();
            IsCompressEnabled = NumColsRequested > 1;
            IsExpandEnabled = NumColsRequested < MaxColsVisible;
            Width = Maximized ? MapWidth : NumColsRequested * COLUMN_SIZE;
            Height = Maximized ? MapHeight - MAXIMIZED_MAP_MARGIN + ARRIVALS_CONTROL_TRIANGLE_HEIGHT : Math.Min(NORMAL_HEIGHT, MapHeight - MAXIMIZED_MAP_MARGIN);
        }

        private LatLon _MapLocation;
        public LatLon MapLocation
        {
            get { return _MapLocation; }
            set
            {
                SetProperty(ref _MapLocation, value);
            }
        }

        public Action<bool> VisibilityChangedCallback { get; set; }
        private void SetVisibilityInternal(bool visible)
        {
            if (VisibilityChangedCallback != null)
                VisibilityChangedCallback(visible);
            else
                IsVisible = visible;
        }

        public void SetVisibility()
        {
            if (Stop.HasValue && !IsVisible)
                SetVisibilityInternal(true);
            else if (!Stop.HasValue && IsVisible)
                SetVisibilityInternal(false);
        }

        private bool _IsExpandEnabled = false;
        public bool IsExpandEnabled
        {
            get { return _IsExpandEnabled; }
            set
            {
                SetProperty(ref _IsExpandEnabled, value);
                (ExpandCommand as Command)?.ChangeCanExecute();
            }
        }

        private bool _IsCompressEnabled = false;
        public bool IsCompressEnabled
        {
            get { return _IsCompressEnabled; }
            set
            {
                SetProperty(ref _IsCompressEnabled, value);
                (CompressCommand as Command)?.ChangeCanExecute();
            }
        }

        private bool _IsVisible = false;
        public bool IsVisible
        {
            get { return _IsVisible; }
            set
            {
                SetProperty(ref _IsVisible, value);
                OnPropertyChanged();
            }
        }

        private double _Width;
        public double Width
        {
            get { return _Width; }
            set
            {
                SetProperty(ref _Width, value);
            }
        }

        private double _Height;
        public double Height
        {
            get { return _Height; }
            set
            {
                SetProperty(ref _Height, value);
            }
        }

        private TransitStop? _Stop;
        public TransitStop? Stop
        {
            get { return _Stop; }
            set
            {
                SetProperty(ref _Stop, value);
                if (value.HasValue)
                {
                    _DataContext = new StopArrivalsViewModel(value.Value);
                    MapLocation = value.Value.Position;
                }
                else
                    _DataContext = new StopArrivalsViewModel(new TransitStop());
                OnPropertyChanged("DataContext");
                SetCenterRegion();
            }
        }

        private StopArrivalsViewModel _DataContext;
        public StopArrivalsViewModel DataContext
        {
            get { return _DataContext; }
        }

        private RectSubset _CenterRegion;
        public RectSubset CenterRegion
        {
            get { return _CenterRegion; }
            set
            {
                SetProperty(ref _CenterRegion, value);
            }
        }

        private bool _IsOnMap = true;
        public bool IsOnMap
        {
            get { return _IsOnMap; }
            set
            {
                SetProperty(ref _IsOnMap, value);
            }
        }
    }
}
