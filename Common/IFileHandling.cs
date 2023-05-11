using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface IFileHandling
    {
        [OperationContract]
        ReceivedFileOptions SendData(SendFileOptions file);

    }
}
