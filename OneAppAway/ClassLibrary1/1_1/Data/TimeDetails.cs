using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace OneAppAway._1_1.Data
{
    public class TimeDetails : DependencyObject
    {
        public static TimeDetails Instance { get; }

        static TimeDetails()
        {
            Instance = new TimeDetails();
        }

        private TimeDetails()
        {
            Refresh();
        }

        public void Refresh()
        {
            Now = DateTime.Now;
        }


        public DateTime Now
        {
            get { return (DateTime)GetValue(NowProperty); }
            set { SetValue(NowProperty, value); }
        }
        public static readonly DependencyProperty NowProperty =
            DependencyProperty.Register("Now", typeof(DateTime), typeof(TimeDetails), new PropertyMetadata(new DateTime()));
    }
}
