using OneAppAway._1_1.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.ViewModels
{
    public class StopArrivalsViewModel : LightweightViewModelBase
    {
        public StopArrivalsViewModel(TransitStop stop)
        {
            if (stop.Children != null)
            {
                foreach (var childID in stop.Children)
                {
                    var child = TransitStop.SqlProvider.Select(query => DatabaseManager.ExecuteSQL(DatabaseManager.MemoryDatabase, query), $"ID = '{childID}'").FirstOrDefault();
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
                _StopName = value;
                OnPropertyChanged();
            }
        }

        public bool HasChildren { get; }
    }
}
