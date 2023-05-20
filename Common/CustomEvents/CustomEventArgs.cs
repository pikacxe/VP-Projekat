using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.CustomEvents
{
    public class CustomEventArgs<T>:EventArgs
    {
        public T Item { get; set; }

        public CustomEventArgs(T item)
        {
            Item=item;
        }
    }
}
