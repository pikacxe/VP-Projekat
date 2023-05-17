using System;
using System.ServiceModel;

namespace Service
{
    internal class Program
    {
        static void Main(string[] args)
        {
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
