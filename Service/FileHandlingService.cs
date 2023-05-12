using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class FileHandlingService : IFileHandling
    {
        [OperationBehavior(AutoDisposeParameters = true)]
        public ReceivedFileOptions SendData(SendFileOptions file)
        {
            return new ReceivedFileOptions(0,ResultMessageType.Success);
        }
    }
}
