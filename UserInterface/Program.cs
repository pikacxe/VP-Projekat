using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.ServiceModel;
using System.IO;
using System.Configuration;
using System.Xml;

namespace UserInterface
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var xmlPath = ConfigurationManager.AppSettings["xmlPath"];
            while (true)
            {
                if (File.Exists(xmlPath))
                {
                    Console.WriteLine($"Cekam kreiranje ili modifikovanja fajla: {xmlPath}");
                    while (!File.Exists(xmlPath)) ;
                }
                FileInfo fi = new FileInfo(xmlPath);
                DateTime fileCreated = fi.CreationTime;
                FileStream f = File.Open(xmlPath, FileMode.Open);
            }

        }

    }
}
