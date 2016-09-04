using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneAppAway._1_1
{
    public class EventArgs<T> : EventArgs
    {
        public EventArgs() { }
        public EventArgs(T parameter)
        {
            Parameter = parameter;
        }
        public T Parameter { get; set; }
    }
}
