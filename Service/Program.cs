using Common.CustomEvents;
using InMemoryDB;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Service
{
    internal class Program
    {
        static void Main(string[] args)
        {

            DBPrinter dBPrinter = new DBPrinter();
            // Event for printing tables 
            dBPrinter.CustomEvent += PrintTable;
            // Detecting from keybord key which table to write
            dBPrinter.DetectKey();

            
            using (ServiceHost host = new ServiceHost(typeof(FileHandlingService)))
            {
                host.Open();
                Console.WriteLine("Service started. Press Esc twice to exit...");
                while (Console.ReadKey(intercept:true).Key != ConsoleKey.Escape);
                host.Close();
            }

        }

        private static void PrintTable(object sender, CustomEventArgs<string> args)
        {
            switch (args.Item)
            {
                case "loads":
                    Console.WriteLine(Load.FormatHeader());
                    PrintList(DataBase.Instance.Loads);
                    break;
                case "audits":
                    Console.WriteLine(Audit.FormatHeader());
                    PrintList(DataBase.Instance.Audits);
                    break;
                case "ifiles":
                    Console.WriteLine(ImportedFile.FormatHeader());
                    PrintList(DataBase.Instance.ImportedFiles);
                    break;
            }
        }

        private static void PrintList<T>(IEnumerable<T> list)
        {
            foreach (var x in list)
            {
                Console.WriteLine(x);
            }
        }

    }
}
