using MvvmHelpers;
using OneAppAway._1_1.Abstract;
using OneAppAway._1_1.Data;
using OneAppAway.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OneAppAway._1_1.ViewModels
{
    public class StopArrivalsBoxViewModel : BaseViewModel
    {
        #region Static
        private static List<WeakReference<StopArrivalsBoxViewModel>> Instances = new List<WeakReference<StopArrivalsBoxViewModel>>();
        private static TimeSpan Interval = TimeSpan.FromSeconds(10);

        private static IntervalExecuterBase _IntervalExecuter;
        public static IntervalExecuterBase IntervalExecuter
        {
            get { return _IntervalExecuter; }
            set
            {
                if (IntervalExecuter != null)
                    IntervalExecuter.DeregisterTask(IntervalExecuterCommand);
                _IntervalExecuter = value;
                if (IntervalExecuter != null)
                    IntervalExecuter.RegisterTask(IntervalExecuterCommand, TimeSpan.FromSeconds(30), TimeSpan.Zero);
            }
        }

        private static RelayCommand IntervalExecuterCommand = new RelayCommand(async (obj) =>
        {
            if (Instances.Count == 0)
                return;
            int msDelay = (int)(Interval.TotalMilliseconds / Instances.Count);
            for (int i = 0; i < Instances.Count; i++)
            {
                var instance = Instances[i];
                StopArrivalsBoxViewModel reference;
                if (instance.TryGetTarget(out reference))
                {
                    reference.Refresh(false);
                }
                else
                {
                    Instances.Remove(instance);
                    i--;
                }
                if (i < Instances.Count)
                    await Task.Delay(msDelay);
            }
        });

        static StopArrivalsBoxViewModel()
        {
        }
        #endregion

        public bool AutoDownload { get; set; } = false;

        private CancellationTokenSource TokenSource = new CancellationTokenSource();
        public StopArrivalsBoxViewModel()
        {
            Instances.Add(new WeakReference<StopArrivalsBoxViewModel>(this));
        }

        private TransitStop _Stop;
        public TransitStop Stop
        {
            get { return _Stop; }
            set
            {
                var old = Stop;
                SetProperty(ref _Stop, value);
                if (Stop != old)
                {
                    Refresh(false);
                }
            }
        }

        private DataLoadStatus _LoadStatus;
        public DataLoadStatus LoadStatus
        {
            get { return _LoadStatus; }
            set { SetProperty(ref _LoadStatus, value); }
        }

        private string _ErrorMessage = "";
        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set { SetProperty(ref _ErrorMessage, value); }
        }

        private bool _Error = false;
        public bool Error
        {
            get { return _Error; }
            set { SetProperty(ref _Error, value); }
        }

        public async void Refresh(bool forceOnline)
        {
            if (Stop.Children != null)
            {
                Items.Clear();
                return;
            }
            Error = false;
            AutoDownload = AutoDownload || forceOnline;
            IsBusy = true;
            LoadStatus = DataLoadStatus.Loading;
            try
            {
                Items.Clear();
                //if (Stop.ID == null)
                //{
                //    Items.Clear();
                //    return;
                //}
                var arrivals = await DataSource.GetRealTimeArrivalsForStopAsync(Stop.ID, 5, 35, AutoDownload ? DataSourcePreference.All : DataSourcePreference.OfflineSources, TokenSource.Token); //await ApiLayer.GetTransitArrivals(Stop.ID, 5, 35, TokenSource.Token);
                if (arrivals.ErrorMessage != null)
                {
                    Error = true;
                    ErrorMessage = arrivals.ErrorMessage;
                    
                }
                if (arrivals.HasData)
                {
                    //var viewModels = arrivals.Data.Select(arrival => new RealTimeArrivalViewModel(arrival));
                    //var toRemove = new List<RealTimeArrivalViewModel>();
                    //var toChange = new List<RealTimeArrivalViewModel>();
                    //var toAdd = new List<RealTimeArrivalViewModel>();
                    //foreach (var newItem in viewModels)
                    //{
                    //    if (!Items.Contains(newItem))
                    //        toAdd.Add(newItem);
                    //    else
                    //        toChange.Add(newItem);
                    //}
                    //foreach (var oldItem in Items)
                    //{
                    //    if (!viewModels.Contains(oldItem))
                    //        toRemove.Add(oldItem);
                    //}
                    //Items.Add(new RealTimeArrivalViewModel(new RealTimeArrival()));


                    //Items.ReplaceRange(arrivals.Data);


                    foreach (var newItem in arrivals.Data)
                    {
                        int curIndex = -1;
                        int newIndex = 0;
                        for (int i = 0; i < Items.Count; i++)
                        {
                            var bkaeOther = Items[i].BestKnownArrivalTime;
                            var bkaeThis = newItem.BestKnownArrivalTime;
                            if (Items[i].Trip == newItem.Trip)
                            {
                                curIndex = i;
                                if (!bkaeThis.HasValue)
                                    newIndex = i;
                            }
                            if (bkaeOther.HasValue && bkaeThis.HasValue)
                            {
                                if (bkaeOther < bkaeThis || (bkaeOther == bkaeThis && curIndex == i))
                                    newIndex = i;
                            }
                        }
                        if (curIndex >= 0)
                        {
                            Items[curIndex] = newItem;
                            if (curIndex != newIndex)
                            {
                                if (newIndex >= Items.Count)
                                {

                                }
                                Items.Move(curIndex, newIndex);
                            }
                        }
                        else
                        {
                            if (newIndex > Items.Count)
                            {

                            }
                            Items.Insert(newIndex, newItem);
                        }
                    }
                }
            }
            finally
            {
                IsBusy = false;
                LoadStatus = AutoDownload ? DataLoadStatus.All : (Items.Count == 0) ? DataLoadStatus.None : DataLoadStatus.OfflineOnly;
                if (Stop.ID == null)
                    LoadStatus = DataLoadStatus.All;
            }
        }

        private ObservableRangeCollection<RealTimeArrival> _Items = new ObservableRangeCollection<RealTimeArrival>();
        public ObservableRangeCollection<RealTimeArrival> Items
        {
            get { return _Items; }
        }
    }
}
