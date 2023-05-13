using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace Common
{
    public enum ResultMessageType { Success, Failed, Default }

    [DataContract]
    public class ReceivedFileOptions : IDisposable
    {

        public ReceivedFileOptions(int numOfFiles, ResultMessageType resultMessage)
        {
            ReceivedFiles = new Dictionary<string, MemoryStream>(numOfFiles);
            NumOfFiles = numOfFiles;
            ResultMessage = resultMessage;
        }
        public ReceivedFileOptions():this(0,ResultMessageType.Default)
        {
        
        }

        [DataMember]
        public Dictionary<string, MemoryStream> ReceivedFiles { get; set; }
        [DataMember]
        public int NumOfFiles { get; set; }
        [DataMember]
        public ResultMessageType ResultMessage { get; set; }


        public void Dispose()
        {
            if (ReceivedFiles == null)
            {
                return;
            }
            try
            {
                foreach(var x in ReceivedFiles.Values)
                {
                    x.Dispose();
                    x.Close();
                }
                ReceivedFiles.Clear();

            }
            catch(Exception)
            {
                Console.WriteLine("Error while disposing ReceivedFilesOptions!");
            }


        }

    }
}
