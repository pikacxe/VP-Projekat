using System;
using System.IO;
using System.Configuration;
using System.ServiceModel;
using Common;
using UserInterface.FileInUseChecker;

namespace UserInterface
{
    internal class Program
    {
        static bool changed = false;
        static void Main(string[] args)
        {
            // Load config options
            var filePath = ConfigurationManager.AppSettings.Get("xmlPath");
            var receivePath = ConfigurationManager.AppSettings.Get("savePath");

            // Create a new FileSystemWatcher object and set its properties
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = Path.GetDirectoryName(filePath);
            watcher.Filter = Path.GetFileName(filePath);
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime;

            // Add an event handler for the Changed event
            watcher.Changed += OnFileChanged;

            // Start watching for changes
            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Proccess started and waiting for changes! Press Esc key to exit...");

            // Create new ChannelFactory and load a preset configuration
            ChannelFactory<IFileHandling> cf = new ChannelFactory<IFileHandling>("FileHandlingService");
            IFileHandling fileHandler = cf.CreateChannel();

            IFileInUseChecker fileInUseCheck = new FileInUseCommonChecker();

            // Main loop 
            while (true)
            {
                if (Console.KeyAvailable && Console.ReadKey().Key == ConsoleKey.Escape)
                {
                    break;
                }

                // Wait for file changes
                if (!changed)
                {
                    continue;
                }

                // File has changed, check if it is still in use
                if (fileInUseCheck.IsFileInUse(filePath))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[WARN] File is currently in ise by another proccess!");
                    Console.ResetColor();
                    continue;
                }

                // Changes have been completed
                changed = false;


                using (MemoryStream ms = new MemoryStream())
                {
                    // Write file content to memory stream
                    GetMemoryStream(ms, filePath);

                    // Prepare file content for sending
                    using (SendFileOptions sfo = new SendFileOptions(ms, filePath))
                    {
                        Console.WriteLine("[Info] Sending data to service for proccessing!");

                        // Send file content to service
                        using (ReceivedFileOptions rfo = fileHandler.SendData(sfo))
                        {
                            // Properly dispose of stream
                            sfo.Dispose();
                            ms.Dispose();
                            ms.Close();

                            Console.WriteLine("[Info] Received results!");

                            if (rfo.ResultMessage == ResultMessageType.Success)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"[Success] Received {rfo.NumOfFiles} files. Initialized file saving sequence...");

                                foreach (var file in rfo.ReceivedFiles)
                                {
                                    string fullPath = Path.Combine(receivePath, file.Key);
                                    using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
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
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("[Error] Failed to proccess provided data. Please check your data file!");
                                Console.ResetColor();
                            }
                            // Properly dispose of received files
                            rfo.Dispose();
                        }

                    }
                }
                Console.WriteLine("Waiting for changes. Press Esc to exit...");
            }

            // Properly dispose of event listener
            watcher.Changed -= OnFileChanged;
            watcher.Dispose();

        }

        private static void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("Detected file change, proccessing...");
            changed = true;
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
