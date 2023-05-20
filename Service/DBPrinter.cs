using Common.CustomEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class DBPrinter : CustomEventSource<string>
    {

        public void DetectKey()
        {
            ConsoleKeyInfo keyInfo;
            Task.Run(() =>
            {
                while (true)
                {
                    keyInfo = Console.ReadKey();
                    if (keyInfo.Key == ConsoleKey.F4)
                    {
                        RaiseCustomEvent("audits");
                    }
                    if (keyInfo.Key == ConsoleKey.F5)
                    {
                        RaiseCustomEvent("loads");
                    }
                    if (keyInfo.Key == ConsoleKey.F6)
                    {
                        RaiseCustomEvent("ifiles");
                    }
                }
            });
        }
    }
}
