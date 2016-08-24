using MvvmHelpers;
using OneAppAway._1_1.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.ViewModels
{
    public class StopArrivalsViewModel : BaseViewModel
    {
        public StopArrivalsViewModel(TransitStop stop)
        {
            if (stop.Children != null)
            {
                foreach (var childID in stop.Children)
                {
                    var child = TransitStop.SqlProvider.Select(DatabaseManager.MemoryDatabase, $"ID = '{childID}'").FirstOrDefault();
                    ChildrenSource.Add(new StopArrivalsViewModel(child)); //Nested ViewModels!
                }
            }
            StopName = stop.Name;
            HasChildren = ChildrenSource.Count > 0;
        }

        private ObservableCollection<StopArrivalsViewModel> _ChildrenSource = new ObservableCollection<StopArrivalsViewModel>();
        public ObservableCollection<StopArrivalsViewModel> ChildrenSource
        {
            get { return _ChildrenSource; }
            private set { _ChildrenSource = value; }
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

        public bool HasChildren { get; }
    }
}
