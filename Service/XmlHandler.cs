using Common.FileHandling;
using Common.CustomEvents;
using InMemoryDB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.ServiceModel;
using System.Xml;


namespace Service
{
    public class XmlHandler : CustomEventSource<List<GroupedLoads>>
    {
        public void ReadXmlFile(MemoryStream memoryStream, string filename)
        {
            XmlDocument xmlDocument=new XmlDocument();
            using (MemoryStream ms = new MemoryStream(memoryStream.ToArray()))
            {
                xmlDocument.Load(ms);
                Console.WriteLine($"[INFO] Received '{filename}'");
                XmlNode root = xmlDocument.DocumentElement;
                if (root != null)
                {
                    XmlNodeList xmlNodeList = root.SelectNodes("row");
                    if (xmlNodeList.Count > 0)
                    {
                        ParseXmlNodeList(xmlNodeList);
                        WriteImportedFile(filename);
                    }
                    else
                    {
                        string message = $"'{filename}' xml document is empty. Or does not cotains 'row' tags.";
                        Audit audit = new Audit(DateTime.Now, MessageType.Error, message);
                        DataBase.Instance.AddAudit(audit);
                        ms.Dispose();
                        ms.Close();
                        throw new FaultException<FileHandlingException>(new FileHandlingException(message));
                    }
                }
                else
                {
                    string message = $"Invalid xml document '{filename}'";
                    Audit audit = new Audit(DateTime.Now, MessageType.Error, message);
                    DataBase.Instance.AddAudit(audit);
                    ms.Dispose();
                    ms.Close();
                    throw new FaultException<FileHandlingException>(new FileHandlingException(message));
                }
                ms.Dispose();
                ms.Close();
            }
        }
        private void WriteImportedFile(string filename)
        {
            DataBase.Instance.AddImportedFile(new ImportedFile(filename));
            Console.WriteLine($"[INFO] Imported file '{filename}' proccessed and saved to database.");
        }

        private void ParseXmlNodeList(XmlNodeList xmlNodeList)
        {
            List<GroupedLoads> groupedLoadsList = new List<GroupedLoads>();
            DateTime date = new DateTime();
            int listindex = -1;
            int count = 0;

            foreach (XmlNode x in xmlNodeList)
            {
                Load load = ParseXmlNode(x);
                if (load == new Load())
                {
                    continue;
                }
                if (date.Date != load.TimeStamp.Date)
                {
                    groupedLoadsList.Add(new GroupedLoads(load.TimeStamp.Date));
                    date = load.TimeStamp.Date;
                    listindex++;
                }
                groupedLoadsList[listindex].loads.Add(load);
                DataBase.Instance.AddLoad(load);
                count++;
            }
            Console.WriteLine($"[INFO] {count} loads proccessed and saved to database");
            RaiseCustomEvent(groupedLoadsList);
        }

        private Load ParseXmlNode(XmlNode x)
        {
            Load load = new Load();
            //Check date if valid
            DateTime date;

            if (TryParseNodeValueDate(x.SelectSingleNode("TIME_STAMP"), out date))
            {
                load.TimeStamp = date;
            }

            float forecast;
            if (TryParseNodeValue(x.SelectSingleNode("FORECAST_VALUE"), out forecast))
            {
                load.ForecastValue = forecast;

            }

            float measured;
            if (TryParseNodeValue(x.SelectSingleNode("MEASURED_VALUE"), out measured))
            {
                load.MeasuredValue = measured;

            }

            return load;
        }
        private bool TryParseNodeValueDate(XmlNode xmlNode, out DateTime Storage)
        {
            if (xmlNode != null)
            {
                string stringValue = xmlNode.InnerText.Trim();
                if (stringValue != string.Empty)
                {
                    try
                    {
                        Storage = DateTime.ParseExact(stringValue, "yyyy-MM-dd HH:mm", null);
                        return true;
                    }
                    catch (FormatException)
                    {
                        Audit audit = new Audit(DateTime.Now, MessageType.Warning, $"Invalid format in XmlNode '{xmlNode.Name}'.");
                        DataBase.Instance.AddAudit(audit);
                    }
                }
                else
                {
                    Audit audit = new Audit(DateTime.Now, MessageType.Info, $"XmlNode '{xmlNode.Name}' is empty.");
                    DataBase.Instance.AddAudit(audit);
                }
            }
            else
            {
                Audit audit = new Audit(DateTime.Now, MessageType.Error, "XmlNode is missing.");
                DataBase.Instance.AddAudit(audit);
            }
            Storage = new DateTime();
            return false;
        }

        private bool TryParseNodeValue(XmlNode xmlNode, out float Storage)
        {
            if (xmlNode != null)
            {
                string stringValue = xmlNode.InnerText.Trim();
                if (stringValue != string.Empty)
                {
                    bool ret = float.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out Storage);
                    if (ret == false)
                    {
                        Audit audit = new Audit(DateTime.Now, MessageType.Warning, $"Invalid value in XmlNode '{xmlNode.Name}'.");
                        DataBase.Instance.AddAudit(audit);
                        return false;
                    }
                    return true;

                }
                else
                {
                    Audit audit = new Audit(DateTime.Now, MessageType.Info, $"XmlNode '{xmlNode.Name}' is empty.");
                    DataBase.Instance.AddAudit(audit);
                }
            }
            else
            {
                Audit audit = new Audit(DateTime.Now, MessageType.Error, "XmlNode is missing.");
                DataBase.Instance.AddAudit(audit);
            }
            Storage = -1;
            return false;
        }
    }
}
