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
            // Check number of files from App.config
            var multipleCsv = ConfigurationManager.AppSettings.Get("multipleCSV");
            if (multipleCsv == "false")
            {
                // Event for single file
                xmlHandler.CustomEvent += GenerateSingleCsv;
            }
            else
            {
                //Event for multiple files
                xmlHandler.CustomEvent += GenerateMultipleCsv;
            }
        }

        [OperationBehavior(AutoDisposeParameters = true)]


        /// Main entry method. Accepts file from client's side.
        /// Proccess it and generate result file options
        public ReceivedFileOptions SendData(SendFileOptions file)
        {
            if (file == null || file.MS == null)
            {
                throw new FaultException<FileHandlingException>(new FileHandlingException("[ERROR] Sent invalid file"));
            }
            try
            {
                // Variable for result message
                string message = string.Empty;

                // Parse provided file
                xmlHandler.ReadXmlFile(file.MS, file.FileName, out message);

                // Set result message
                fileOptions.ResultMessage = ResultMessageType.Success;
                fileOptions.Message = message;
            }
            catch(Exception e)
            {
                fileOptions.ResultMessage = ResultMessageType.Failed;
                fileOptions.Message = "Internal service error";
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
                // Add elements from all GroupLoads lists in one list
                foreach (var x in args.Item)
                {
                    loads.AddRange(x.loads);
                }
                
                fileOptions.ReceivedFiles.Add("result_data.csv", new MemoryStream());
                // Write all loads in single file
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
                    // Make file for every date
                    string fileName = $"result_data_{p.Date.Year}_{p.Date.Month}_{p.Date.Day}.csv";
                    fileOptions.ReceivedFiles.Add(fileName, new MemoryStream());
                    // Write loads from that date
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
                // Write elements from list to file
                foreach (var l in loads)
                {
                    streamWriter.Write(l.ToCsv());
                }
                // Properly dispose of streamWriter
                streamWriter.Dispose();
                streamWriter.Close();
            }
        }

       

    }
}
