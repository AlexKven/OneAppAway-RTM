using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    class TestDataSource : OneAppAway._1_1.Data.UIDataSource
    {
        private int _Data;
        public int Data
        {
            get { return _Data; }
            set
            {
                _Data = value;
                OnDataChanged();
            }
        }
    }
}
