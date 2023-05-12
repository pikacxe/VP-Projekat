using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;

namespace Common
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
