using MvvmHelpers;
using OneAppAway._1_1.Data;
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
        private static List<WeakReference<StopArrivalsBoxViewModel>> Instances = new List<WeakReference<StopArrivalsBoxViewModel>>();

        static StopArrivalsBoxViewModel()
        {
            TimeDetails.Instance.RegisterTask(() =>
            {
                for (int i = 0; i < Instances.Count; i++)
                {
                    var instance = Instances[i];
                    StopArrivalsBoxViewModel reference;
                    if (instance.TryGetTarget(out reference))
                    {
                        reference.Refresh();
                    }
                    else
                    {
                        Instances.Remove(instance);
                        i--;
                    }
                }
            }, 3);
        }

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
                    Refresh();
            }
        }

        public async void Refresh()
        {
            IsBusy = true;
            try
            {
                if (Stop.ID == null)
                {
                    Items.Clear();
                    return;
                }
                var arrivals = await ApiLayer.GetTransitArrivals(Stop.ID, 5, 35, TokenSource.Token);
                if (arrivals != null)
                {
                    var viewModels = arrivals.Select(arrival => new RealTimeArrivalViewModel(arrival));
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
                    Items.ReplaceRange(viewModels);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private ObservableRangeCollection<RealTimeArrivalViewModel> _Items = new ObservableRangeCollection<RealTimeArrivalViewModel>();
        public ObservableRangeCollection<RealTimeArrivalViewModel> Items
        {
            get { return _Items; }
        }
    }
}
