using System;
using System.IO;
using System.Configuration;
using System.ServiceModel;
using Common.FileHandling;
using UserInterface.FileInUseChecker;
using System.Threading;

namespace UserInterface
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Load config options
            string filePath = ConfigurationManager.AppSettings.Get("xmlPath");


            // Get file name from path
            string fileName = Path.GetFileName(filePath);

            // Create a new FileSystemWatcher object and set its properties
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = Path.GetDirectoryName(filePath);
            watcher.Filter = fileName;
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime;

            // Add an event handler for the Changed event
            watcher.Changed += OnFileChanged;
            watcher.Created += OnFileChanged;

            // Start watching for changes
            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Proccess started and waiting for changes! Press Esc key to exit...");

            // Main loop 
            while (Console.ReadKey(intercept: true).Key != ConsoleKey.Escape) ;

            // Properly dispose of event listener
            watcher.Changed -= OnFileChanged;
            watcher.Created -= OnFileChanged;
            watcher.Dispose();
        }

        private static void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"Detected file '{e.Name}' change, proccessing...");

            // Check if file is still in use
            while (FileInUse(e.FullPath))
            {
                Thread.Sleep(1000);
            };

            // Define needed parameters
            SendFileOptions sfo = null;
            ReceivedFileOptions rfo = null;
            ChannelFactory<IFileHandling> cf = null;
            IFileHandling fileHandler;

            try
            {
                // Create new ChannelFactory and load a preset configuration
                cf = new ChannelFactory<IFileHandling>("FileHandlingService");
                fileHandler = cf.CreateChannel();

                // Prepare file content for sending
                sfo = new SendFileOptions(new MemoryStream(), e.Name);

                // Write file content to memory stream
                GetMemoryStream(sfo.MS, e.FullPath);
                Console.WriteLine("[Info] Sending data to service for proccessing!");

                // Send file content to service
                rfo = fileHandler.SendData(sfo);

                // Dispose of send file options as it is no longer needed
                sfo.Dispose();

                Console.WriteLine("[Info] Received results!");
                HandleResult(rfo);

                // Properly dispose of received files
                rfo.Dispose();

            }
            catch (FaultException<FileHandlingException> ex)
            {
                WriteErrorMessageToConsole(ex.Detail.Message);
            }
            catch (Exception ex)
            {
                WriteErrorMessageToConsole(ex.Message);
            }
            finally
            {
                if(rfo != null)
                {
                    rfo.Dispose();
                }
                if(sfo != null)
                {
                    sfo.Dispose();
                }
                if(cf != null)
                {
                    cf.Close();
                }
            }
            Console.WriteLine("Waiting for changes. Press Esc to exit...");
        }


        private static void WriteErrorMessageToConsole(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Error] {message}");
            Console.ResetColor();
        }

        private static bool FileInUse(string filePath)
        {
            // Helper class for FileInUse check
            IFileInUseChecker fileInUseCheck = new FileInUseCommonChecker();

            // File has changed, check if it is still in use
            if (fileInUseCheck.IsFileInUse(filePath))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[WARN] File '{filePath}' is currently in use by another proccess!");
                Console.ResetColor();
                return true;
            }
            return false;
        }

        private static void HandleResult(ReceivedFileOptions rfo)
        {
            if (rfo.ResultMessage == ResultMessageType.Success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[Success] Received {rfo.NumOfFiles} files. Initialized file saving sequence...");

                // Where to save received files
                string receivePath = ConfigurationManager.AppSettings.Get("savePath");

                foreach (var file in rfo.ReceivedFiles)
                {
                    string fullPath = Path.Combine(receivePath, file.Key);
                    using (FileStream fileStream = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        // Write data from received MemoryStream to local file
                        using (MemoryStream receivedStream = new MemoryStream(file.Value.ToArray()))
                        {
                            receivedStream.CopyTo(fileStream);
                            Console.WriteLine($"\t[SAVED] '{fullPath}'");
                        }
                        // Properly dispose of stream
                        fileStream.Dispose();
                        fileStream.Close();
                    }
                }

                Console.WriteLine($"[Success] File saving sequence completed! Saved {rfo.NumOfFiles} to '{receivePath}'");
                Console.ResetColor();
            }
            else if (rfo.ResultMessage == ResultMessageType.Failed)
            {
                WriteErrorMessageToConsole(rfo.Message);
            }
        }

        private static void GetMemoryStream(MemoryStream ms, string filePath)
        {
            // Open file for reading
            using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                // Copy file content to MemoryStream
                fs.CopyTo(ms);

                // Properly dispose of stream
                fs.Dispose();
                fs.Close();
            }
        }
    }
}
