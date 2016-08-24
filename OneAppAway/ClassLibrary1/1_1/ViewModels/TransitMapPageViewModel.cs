﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmHelpers;
using OneAppAway._1_1.Data;
using System.Threading;

namespace OneAppAway._1_1.ViewModels
{
    public class TransitMapPageViewModel : BaseViewModel
    {
        public ObservableRangeCollection<TransitStop> ShownStops { get; } = new ObservableRangeCollection<TransitStop>();
        private CancellationTokenSource TokenSource = new CancellationTokenSource();
        double lastZoomLevel = 0;

        private LatLonRect _Area = LatLonRect.NotAnArea;
        public LatLonRect Area
        {
            get { return _Area; }
            set
            {
                var old = Area;
                SetProperty(ref _Area, value);
                OnAreaChanged(old, value);
            }
        }

        private void OnAreaChanged(LatLonRect oldArea, LatLonRect newArea)
        {
            if (ZoomLevel < SmallThreshold)
            {
                ShownStops.Clear();
                PendingRegions.Clear();
            }
            else
            {
                IEnumerable<LatLonRect> newRegion;
                if (lastZoomLevel < SmallThreshold)
                    newRegion = new LatLonRect[] { newArea };
                else
                    newRegion = oldArea.GetNewRegion(newArea);
                foreach (var piece in newRegion)
                {
                    foreach (var subPiece in piece.Miniaturize(new LatLon(MAX_LAT_RANGE, MAX_LON_RANGE)))
                    {
                        PendingRegions.Enqueue(subPiece);
                    }
                }
                if (!IsBusy)
                    RefreshShownStops(TokenSource.Token);
            }
            lastZoomLevel = ZoomLevel;
        }

        private double _ZoomLevel = 1;
        public double ZoomLevel
        {
            get { return _ZoomLevel; }
            set
            {
                SetProperty(ref _ZoomLevel, value);
            }
        }

        private RectSubset OuterStopCacheMargin = new RectSubset() { Left = -.25, LeftScale = RectSubsetScale.Relative, Right = -.25, RightScale = RectSubsetScale.Relative, Top = -.25, TopScale = RectSubsetScale.Relative, Bottom = -.25, BottomScale = RectSubsetScale.Relative };
        public const double MAX_LAT_RANGE = 0.01;
        public const double MAX_LON_RANGE = 0.015;
        private Queue<LatLonRect> PendingRegions = new Queue<LatLonRect>();
        protected async void RefreshShownStops(CancellationToken token)
        {
            IsBusy = true;
            //ShownStops.Add(new TransitStop() { ID = "FWTC", Position = new LatLon(47.31753, -122.30486), Direction = StopDirection.N /*Path = "_vx_HlnniVF??LJ??MF??nIG??MK??LG?"*/, Name = "Federal Way Transit Center" });
            if (ZoomLevel < SmallThreshold)
            {
                ShownStops.Clear();
            }
            else
            {
                //var toRemove = ShownStops.Where(stop => !Area.ContainsLatLon(stop.Position)).ToArray();
                //ShownStops.RemoveRange(toRemove);
                var area = Area;
                for (int i = 0; i < ShownStops.Count; i++)
                {
                    if (!area.ApplySubset(OuterStopCacheMargin).ContainsLatLon(ShownStops[i].Position))
                    {
                        ShownStops.RemoveAt(i);
                        i--;
                    }
                }
                while (PendingRegions.Count > 0)
                {
                    var region = PendingRegions.Dequeue();
                    //var fwtc = new string[] { "1_80439", "1_80431", "1_80438", "1_80432", "1_80437", "1_80433", "3_27814", "3_29410" };
                    var stops = (await _1_1.Data.ApiLayer.GetTransitStopsForArea(region, token))?.Where(stop => !ShownStops.Contains(stop));
                    //var fwtcStop = new TransitStop() { ID = "FWTC", Position = new LatLon(47.31753, -122.30486), Path = "_vx_HlnniVF??LJ??MF??nIG??MK??LG?", Name = "Federal Way Transit Center" };
                    //TransitStop.SqlProvider.Insert(fwtcStop, DatabaseManager.MemoryDatabase);
                    if (stops == null)
                        return;
                    ShownStops.AddRange(stops);
                    //foreach (var stop in stops)
                    //{
                    //    if (!ShownStops.Any(stp => stp.ID == stop.ID))
                    //        ShownStops.Add(stop);
                    //}
                }
            }
            IsBusy = false;
        }


        //bool startRefreshShownStopsRunning = false;
        //private async void StartRefreshShownStops()
        //{
        //    if (startRefreshShownStopsRunning)
        //        StopsNeedRefreshing = true;
        //    else
        //    {
        //        StopsNeedRefreshing = true;
        //        StopsNeedRefreshing = true;
        //        while (StopsNeedRefreshing)
        //        {
        //            StopsNeedRefreshing = false;
        //            await RefreshShownStops(RefreshShownStopsTokenSource.Token);
        //        }
        //    }
        //}

        public double SmallThreshold => 14;
        public double MediumThreshold => 16.5;
        public double LargeThreshold => 18;
    }
}