﻿using MvvmHelpers;
using OneAppAway._1_1.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OneAppAway._1_1.ViewModels
{
    public class StopPopupViewModel : BaseViewModel
    {
        private TransitStop _Stop;
        public TransitStop Stop
        {
            get { return _Stop; }
            set
            {
                SetProperty(ref _Stop, value);
                LoadStopProperties();
            }
        }

        public StopPopupViewModel()
        {
            //if (Children != null && Children.Count > 0)
            //    Width = Children.Aggregate(0.0, (acc, child) => acc + child.Width);
            //else
            //    Width = 290;
        }

        private ObservableCollection<TransitStop> _Children = new ObservableCollection<TransitStop>();
        public ObservableCollection<TransitStop> Children
        {
            get { return _Children; }
            private set { _Children = value; }
        }

        private async void LoadStopProperties()
        {
            StopName = Stop.Name;
            TitleToolTip = $"Stop ID = {Stop.ID}";
            Children.Clear();
            if (Stop.Children != null)
            {
                foreach (var childID in Stop.Children)
                {
                    var child = await DataSource.GetTransitStopAsync(childID, DataSourcePreference.All, CancellationToken.None);
                    if (child.HasData)
                        Children.Add(child.Data);
                }
            }
            if (Children.Count > 0)
                StopName += $" ({Children.Count.ToString()} stops)";
            HasChildren = Children.Count > 0;
            string postfix = ((Stop.Direction == StopDirection.Unspecified) ? "BusBase" : ("BusDirection" + Stop.Direction.ToString()));
            IconUri = new Uri($"ms-appx:///Assets/Icons/{postfix}40.png");
            LoadRouteNames();
        }

        private async void LoadRouteNames()
        {
            RouteNames.Clear();
            if (Stop.Routes == null)
                return;
            IsBusy = true;
            foreach (var routeId in Stop.Routes)
            {
                //var route = await ApiLayer.GetTransitRoute(routeId, new System.Threading.CancellationToken());
                var route = await DataSource.GetTransitRouteAsync(routeId, DataSourcePreference.All, System.Threading.CancellationToken.None);
                if (route.HasData)
                    RouteNames.Add(route.Data.Name);
            }
            IsBusy = false;
        }

        private ObservableCollection<string> _RouteNames = new ObservableCollection<string>();
        public ObservableCollection<string> RouteNames
        {
            get { return _RouteNames; }
        }

        private string _StopName;
        public string StopName
        {
            get { return _StopName; }
            set
            {
                SetProperty(ref _StopName, value);
            }
        }

        private string _TitleToolTip;
        public string TitleToolTip
        {
            get { return _TitleToolTip; }
            set { SetProperty(ref _TitleToolTip, value); }
        }

        private bool _ShowSizeControls = false;
        public bool ShowSizeControls
        {
            get { return _ShowSizeControls; }
            set { SetProperty(ref _ShowSizeControls, value); }
        }

        private bool _HasChildren = false;
        public bool HasChildren
        {
            get { return _HasChildren; }
            set { SetProperty(ref _HasChildren, value); }
        }

        private Uri _IconUri;
        public Uri IconUri
        {
            get { return _IconUri; }
            set
            {
                SetProperty(ref _IconUri, value);
            }
        }

        //public double Width { get; }

        //public bool IsTopLevel { get; private set; } = true;
    }
}
