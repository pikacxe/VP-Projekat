using InMemoryDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Service
{
    public class XmlHandler
    {

        public List<Load> ReadXmlFile(MemoryStream memoryStream,string filename)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(memoryStream);
            XmlNode root = xmlDocument.DocumentElement;

            XmlNodeList xmlNodeList = root.SelectNodes("row");


            foreach(XmlNode x in xmlNodeList)
            {
                ParseXmlNode(x);
            }

            WriteImportedFile(filename);
            return new List<Load>();

        }

        public List<ParseData> ReadXmlFileMultipleCsv(MemoryStream memoryStream, string filename)
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
