using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Service
{
    public class XmlHandler
    {

        public GroupedLoads ReadXmlFile(MemoryStream memoryStream, string filename)
        {
            XmlDocument xmlDocument = new XmlDocument();
            using (MemoryStream ms = new MemoryStream(memoryStream.ToArray()))
            {

                xmlDocument.Load(ms);
                XmlNode root = xmlDocument.DocumentElement;

                XmlNodeList xmlNodeList = root.SelectNodes("row");


                foreach (XmlNode x in xmlNodeList)
                {
                    Console.WriteLine(x.SelectSingleNode("TIME_STAMP").InnerText);
                    // ParseXmlNode(x);
                }

                // WriteImportedFile(filename);
                ms.Dispose();
                ms.Close();
                return new GroupedLoads(DateTime.Now);

            }
        }
        public List<GroupedLoads> ReadXmlFileGrouped(MemoryStream memoryStream, string filename)
        {
            throw new NotImplementedException();
        }
        private void WriteImportedFile(string filename)
        {
            throw new NotImplementedException();
        }

        private void ParseXmlNode(XmlNode x)
        {
            throw new NotImplementedException();
        }
    }
}
