using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class ReceivedFileOptions : IDisposable
    {

        public ReceivedFileOptions(int numOfFiles, string resultMessage)
        {
            ReceivedFiles = new Dictionary<string, MemoryStream>(numOfFiles);
            NumOfFiles = numOfFiles;
            ResultMessage = resultMessage;
        }

        [DataMember]
        public Dictionary<string, MemoryStream> ReceivedFiles { get; set; }
        [DataMember]
        public int NumOfFiles { get; set; }

        public string ResultMessage { get; set; }


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
                Console.WriteLine("Greska prilikom disposovanja ReceivedFilesOptions!");
            }


        }

    }
}
