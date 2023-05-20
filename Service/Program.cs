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
            dBPrinter.CustomEvent += PrintTable;
            dBPrinter.DetectKey();
            using (ServiceHost host = new ServiceHost(typeof(FileHandlingService)))
            {
                host.Open();
                Console.WriteLine("Service started. Press Esc twice to exit...");
                while (Console.ReadKey().Key != ConsoleKey.Escape);
                host.Close();
            }

        }

        private static void PrintTable(object sender, CustomEventArgs<string> args)
        {
            switch (args.Item)
            {
                case "loads":
                    Console.WriteLine(Load.FormatHeader());
                    PrintDictionary(DataBase.Instance.Loads);
                    break;
                case "audits":
                    Console.WriteLine(Audit.FormatHeader());
                    PrintDictionary(DataBase.Instance.Audits);
                    break;
                case "ifiles":
                    Console.WriteLine(ImportedFile.FormatHeader());
                    PrintDictionary(DataBase.Instance.ImportedFiles);
                    break;
            }
        }

        private static void PrintDictionary<T>(IEnumerable<T> list)
        {
            foreach (var kvp in list)
            {
                Console.WriteLine(kvp);
            }
        }

    }
}
