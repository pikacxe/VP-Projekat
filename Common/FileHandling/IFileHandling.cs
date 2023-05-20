using System.ServiceModel;

namespace Common.FileHandling
{
    [ServiceContract]
    public interface IFileHandling
    {
        [OperationContract]
        [FaultContract(typeof(FileHandlingException))]
        ReceivedFileOptions SendData(SendFileOptions file);
    }
}
