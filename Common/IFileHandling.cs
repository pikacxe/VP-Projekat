using System.ServiceModel;

namespace Common
{
    [ServiceContract]
    public interface IFileHandling
    {
        [OperationContract]
        [FaultContract(typeof(FileHandlingException))]
        ReceivedFileOptions SendData(SendFileOptions file);
    }
}
