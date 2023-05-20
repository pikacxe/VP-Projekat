using System.Runtime.Serialization;

namespace Common.FileHandling
{
    [DataContract]
    public class FileHandlingException : System.Exception
    {
        private string message;

        [DataMember]
        public string Message { get => message; set => message = value; }

        public FileHandlingException(string message)
        {
            this.message = message;
        }
    }
}
