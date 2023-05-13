using System.ServiceModel;

namespace Common
{
    [ServiceContract]
    public interface IFileHandling
    {
        [OperationContract]
        ReceivedFileOptions SendData(SendFileOptions file);

    }
}
