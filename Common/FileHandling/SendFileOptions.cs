using System;
using System.Runtime.Serialization;
using System.IO;

namespace Common.FileHandling
{
    [DataContract]
    public class SendFileOptions : IDisposable
    {
        
        public SendFileOptions(MemoryStream ms,string fileName)
        {
            MS=ms;
            FileName = fileName;
        }

        [DataMember]
        public MemoryStream MS { get; set; }

        [DataMember]
        public string FileName { get; set; }

        public void Dispose()
        {
            if(MS == null)
            {
                return;
            }
            try
            {
                MS.Dispose();
                MS.Close();
                MS = null;
            }
            catch (Exception)
            {
                Console.WriteLine("Error while disposing SendFileOptions! ");
            }
        }
    }
}
