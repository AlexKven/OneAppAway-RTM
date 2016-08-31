using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1.Data
{
    public class TimeDetails : INotifyPropertyChanged
    {
        public static TimeDetails Instance { get; } = new TimeDetails();
        
        private TimeDetails()
        {
            Refresh();
        }

        public void Refresh()
        {
            Now = DateTime.Now;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Now"));
        }

        public DateTime Now { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
