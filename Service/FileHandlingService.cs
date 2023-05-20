using Common.CustomEvents;
using Common.FileHandling;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Configuration;
using InMemoryDB;
using System.IO;
using System;

namespace Service
{
    public class FileHandlingService : IFileHandling
    {
        ReceivedFileOptions fileOptions = new ReceivedFileOptions();
        XmlHandler xmlHandler = new XmlHandler();
       
        public FileHandlingService()
        {
            var multipleCsv = ConfigurationManager.AppSettings.Get("multipleCSV");
            if (multipleCsv == "false")
            {
                xmlHandler.CustomEvent += GenerateSingleCsv;
            }
            else
            {
                xmlHandler.CustomEvent += GenerateMultipleCsv;
            }
        }

        [OperationBehavior(AutoDisposeParameters = true)]

        public ReceivedFileOptions SendData(SendFileOptions file)
        {
            if (file == null || file.MS == null)
            {
                throw new FaultException<FileHandlingException>(new FileHandlingException("[ERROR] Sent invalid file"));
            }
            try
            {
                xmlHandler.ReadXmlFile(file.MS, file.FileName);
                fileOptions.ResultMessage = ResultMessageType.Success;
            }
            catch(Exception e)
            {
                Console.WriteLine($"[ERROR] {e.Message}");
            }

            return fileOptions;
        }

        public void GenerateSingleCsv(object sender, CustomEventArgs<List<GroupedLoads>> args)
        {
            try
            {
                fileOptions.NumOfFiles = 1;
                List<Load> loads = new List<Load>();
                foreach (var x in args.Item)
                {
                    loads.AddRange(x.loads);
                }
                fileOptions.ReceivedFiles.Add("result_data.csv", new MemoryStream());

                WriteToStream(fileOptions.ReceivedFiles["result_data.csv"], loads);
            }
            catch (Exception e)
            {
                fileOptions.ResultMessage = ResultMessageType.Failed;
                Console.WriteLine(e.Message);
            }
        }
        public void GenerateMultipleCsv(object sender, CustomEventArgs<List<GroupedLoads>> args)
        {
            try
            {
                fileOptions.NumOfFiles = args.Item.Count();
                foreach (var p in args.Item)
                {
                    string fileName = $"result_data_{p.Date.Year}_{p.Date.Month}_{p.Date.Day}.csv";
                    fileOptions.ReceivedFiles.Add(fileName, new MemoryStream());
                    WriteToStream(fileOptions.ReceivedFiles[fileName], p.loads);
                }
            }
            catch (Exception e)
            {
                fileOptions.ResultMessage = ResultMessageType.Failed;
                Console.WriteLine(e.Message);
            }
        }
        private void WriteToStream(MemoryStream memoryStream, List<Load> loads)
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
