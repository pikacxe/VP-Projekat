using Common;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Configuration;
using InMemoryDB;
using System.IO;

namespace Service
{
    public class FileHandlingService : IFileHandling
    {
        [OperationBehavior(AutoDisposeParameters = true)]
        public ReceivedFileOptions SendData(SendFileOptions file)
        {
            
            XmlHandler xmlHandler=new XmlHandler();
            var multipleCsv = ConfigurationManager.AppSettings.Get("multipleCSV");
            ReceivedFileOptions fileOptions = new ReceivedFileOptions();
            if (multipleCsv == "false")
            {
                fileOptions.NumOfFiles = 1;
                GroupedLoads gl = xmlHandler.ReadXmlFile(file.MS,file.FileName);
                fileOptions.ReceivedFiles.Add("result_data.csv", new MemoryStream());
                WriteToStream(fileOptions.ReceivedFiles["result_data.csv"], gl.loads);
            }
            else
            {
                //reuturns list of ParseData
                List<GroupedLoads> parseData=xmlHandler.ReadXmlFileGrouped(file.MS, file.FileName);
                fileOptions.NumOfFiles = parseData.Count();
                foreach (var p in parseData)
                {
                    string fileName = $"result_data_{p.Date.Year}_{p.Date.Month}_{p.Date.Day}.csv";
                    fileOptions.ReceivedFiles.Add(fileName, new MemoryStream());
                    WriteToStream(fileOptions.ReceivedFiles[fileName], p.loads);
                }

            }
            fileOptions.ResultMessage = ResultMessageType.Success;
            return fileOptions;
        }
        private void WriteToStream(MemoryStream memoryStream,List<Load> loads)
        {
            using (StreamWriter streamWriter = new StreamWriter(memoryStream))
            {
                streamWriter.Write(Load.CsvHeader());
                foreach (var l in loads)
                {
                    streamWriter.Write(l.ToCsv());
                }
                streamWriter.Dispose();
                streamWriter.Close();
            }
        }
    }
}
