using InMemoryDB;
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

                GroupedLoads groupedLoads = new GroupedLoads(DateTime.Now);
                XmlNodeList xmlNodeList = root.SelectNodes("row");
                int index = 0;

                foreach (XmlNode x in xmlNodeList)
                {
                    groupedLoads.loads.Add(ParseXmlNode(x,index));
                    index++;
                    // ParseXmlNode(x);
                }

                // WriteImportedFile(filename);
                ms.Dispose();
                ms.Close();
                return groupedLoads;

            }
        }
        public List<GroupedLoads> ReadXmlFileGrouped(MemoryStream memoryStream, string filename)
        {
            XmlDocument xmlDocument = new XmlDocument();
            using(MemoryStream ms=new MemoryStream(memoryStream.ToArray()))
            {
                xmlDocument.Load(ms);
                XmlNode root = xmlDocument.DocumentElement;
                XmlNodeList xmlNodeList = root.SelectNodes("row");
                List<GroupedLoads> groupedLoadsList = new List<GroupedLoads>();
                int index = 0;
                DateTime date=new DateTime();
                int listindex = -1;

                foreach (XmlNode x in xmlNodeList)
                {
                    string datum = x.SelectSingleNode("TIME_STAMP").InnerText;

                    DateTime currentdate = DateTime.ParseExact(datum, "yyyy-MM-dd HH:mm", null);
                    if (date.Date!=currentdate)
                    {
                        groupedLoadsList.Add(new GroupedLoads(currentdate));
                        listindex++;
                      
                    }
                    groupedLoadsList[listindex].loads.Add(ParseXmlNode(x, index));
                    index++;
                }

                // WriteImportedFile(filename);
                ms.Dispose();
                ms.Close();
                return groupedLoadsList;


            }
        }
        private void WriteImportedFile(string filename)
        {
            throw new NotImplementedException();
        }

        private Load ParseXmlNode(XmlNode x,int index)
        {
           string datum= x.SelectSingleNode("TIME_STAMP").InnerText;
           
            DateTime date = DateTime.ParseExact(datum,"yyyy-MM-dd HH:mm",null);
            float ForecastValue;

            if(float.TryParse(x.SelectSingleNode("FORECAST_VALUE").InnerText,out ForecastValue)==false)
            {
                Audit audit = new Audit(index, date, MessageType.Error, "Invalid Forecast Value");

            }
            float MeasuredValue;
            if( float.TryParse(x.SelectSingleNode("MEASURED_VALUE").InnerText,out MeasuredValue)==false)
            {
                Audit audit = new Audit(index, date, MessageType.Error, "Invalid Measured Value");
            }

            return new Load(index, date, ForecastValue, MeasuredValue);
            
        }



    }
}
