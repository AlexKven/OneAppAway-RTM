﻿using MvvmHelpers;
using OneAppAway._1_1.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OneAppAway._1_1.ViewModels
{
    public class StopArrivalsControlViewModel : BaseViewModel
    {
        public TransitStop Stop { get; }
        public bool ShowRoutesList { get; }
        public StopArrivalsControlViewModel(TransitStop stop, bool showRoutesList)
        {
            Stop = stop;
            ShowRoutesList = showRoutesList;
            if (stop.Children != null)
            {
                foreach (var childID in stop.Children)
                {
                    //var child = TransitStop.SqlProvider.Select(DatabaseManager.MemoryDatabase, $"ID = '{childID}'").FirstOrDefault();
                    //var child = MemoryCache.GetStop(childID);
                    var child = DataSource.GetTransitStop(childID, DataSourcePreference.MemoryCacheOnly);
                    if (child.HasData)
                        ChildrenSource.Add(new StopArrivalsControlViewModel(child.Data, ShowRoutesList) { IsTopLevel = false }); //Nested ViewModels!
                }
            }
            if (ChildrenSource != null && ChildrenSource.Count > 0)
                Width = ChildrenSource.Aggregate(0.0, (acc, child) => acc + child.Width);
            else
                Width = 290;
            StopName = stop.Name;
            TitleToolTip = $"Stop ID = {stop.ID}";
            if (ChildrenSource.Count > 0)
                StopName += $" ({ChildrenSource.Count.ToString()} stops)";
            HasChildren = ChildrenSource.Count > 0;
            string postfix = ((stop.Direction == StopDirection.Unspecified) ? "BusBase" : ("BusDirection" + stop.Direction.ToString()));
            _IconUri = new Uri($"ms-appx:///Assets/Icons/{postfix}40.png");
            LoadRouteNames();
        }

        private ObservableCollection<StopArrivalsControlViewModel> _ChildrenSource = new ObservableCollection<StopArrivalsControlViewModel>();
        public ObservableCollection<StopArrivalsControlViewModel> ChildrenSource
        {
            get { return _ChildrenSource; }
            private set { _ChildrenSource = value; }
        }

        private async void LoadRouteNames()
        {
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

        public bool HasChildren { get; }

        private Uri _IconUri;
        public Uri IconUri
        {
            get { return _IconUri; }
        }

        public double Width { get; }

        public bool IsTopLevel { get; private set; } = true;
    }
}
