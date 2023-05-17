using Common;
using InMemoryDB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Service
{
    public class XmlHandler:CustomEventSource<GroupedLoads>
    {
        public void ReadXmlFile(MemoryStream memoryStream, string filename)
        {
            XmlDocument xmlDocument = new XmlDocument();
            using(MemoryStream ms=new MemoryStream(memoryStream.ToArray()))
            {
                xmlDocument.Load(ms);
                XmlNode root = xmlDocument.DocumentElement;
                XmlNodeList xmlNodeList = root.SelectNodes("row");
                List<GroupedLoads> groupedLoadsList = new List<GroupedLoads>();
                DateTime date=new DateTime();
                int listindex = -1;

                foreach (XmlNode x in xmlNodeList)
                {
                    string datum = x.SelectSingleNode("TIME_STAMP").InnerText;

                    DateTime currentdate = DateTime.ParseExact(datum, "yyyy-MM-dd HH:mm", null);
                    if (date.Date!=currentdate.Date)
                    {

                        groupedLoadsList.Add(new GroupedLoads(currentdate));
                        listindex++;
                     
                        date=currentdate;
                    }
                    groupedLoadsList[listindex].loads.Add(ParseXmlNode(x));
                }

                // WriteImportedFile(filename);
                ms.Dispose();
                ms.Close();
                RaiseCustomEvent(groupedLoadsList);
            }
        }
        private void WriteImportedFile(string filename)
        {
            throw new NotImplementedException();
        }

        private Load ParseXmlNode(XmlNode x)
        {
           string datum= x.SelectSingleNode("TIME_STAMP").InnerText;
           //Check date if valid
            DateTime date = DateTime.ParseExact(datum,"yyyy-MM-dd HH:mm",null);
            float ForecastValue;
            if (float.TryParse(x.SelectSingleNode("FORECAST_VALUE").InnerText, NumberStyles.Float, CultureInfo.InvariantCulture, out ForecastValue)==false)
            {
                Audit audit = new Audit( date, MessageType.Error, "Invalid Forecast Value");

            }
            float MeasuredValue;
            if( float.TryParse(x.SelectSingleNode("MEASURED_VALUE").InnerText,NumberStyles.Float, CultureInfo.InvariantCulture, out MeasuredValue)==false)
            {
                Audit audit = new Audit(date, MessageType.Error, "Invalid Measured Value");
            }

            return new Load(date, ForecastValue, MeasuredValue);
            
        }



    }
}
