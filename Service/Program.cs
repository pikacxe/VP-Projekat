using System;
using System.ServiceModel;

namespace Service
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //XmlHandler xmlHandler = new XmlHandler();
            //xmlHandler.ReadXmlFile("LOAD_DATA");
            using (ServiceHost host = new ServiceHost(typeof(FileHandlingService)))
            {
                host.Open();
                Console.WriteLine("Service started");
                Console.ReadKey();
                host.Close();
            }

        }
    }
}
