using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace OneAppAway._1_1.Data
{
    public class PointWrapper : INotifyPropertyChanged
    {
        private Point _Point;
        public Point Point
        {
            get { return _Point; }
            private set
            {
                _Point = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Point"));
            }
        }

        private string _Text;
        public string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                _Text = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Text"));
                var strs = Text.Split(',').Select(s => s.Trim()).ToArray();
                if (strs.Length == 2)
                {
                    double x;
                    double y;
                    if (double.TryParse(strs[0], out x) && double.TryParse(strs[1], out y))
                        Point = new Point(x, y);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
