using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class CustomEventSource<T>
    {
        public event CustomEventHandler CustomEvent;
        public delegate void CustomEventHandler(object sender, CustomEventArgs<T> args);

        public void RaiseCustomEvent(List<T> test)
        {
            
            // Create a new instance of the custom event arguments class
            CustomEventArgs<T> args = new CustomEventArgs<T>(test);

            // Invoke the event, passing in the sender (this) and the custom event arguments
            CustomEvent?.Invoke(this, args);
        }
    }
}
