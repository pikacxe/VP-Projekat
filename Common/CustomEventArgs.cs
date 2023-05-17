using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class CustomEventArgs<T>:EventArgs
    {
        public List<T> Items { get; set; }

        public CustomEventArgs(List<T> test)
        {
            Items=test;
        }
    }
}
