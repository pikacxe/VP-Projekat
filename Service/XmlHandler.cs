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
        // method is responsible for reading and processing an XML file
        public void ReadXmlFile(MemoryStream memoryStream, string filename)
        {
            XmlDocument xmlDocument = new XmlDocument();
            using (MemoryStream ms = new MemoryStream(memoryStream.ToArray()))
            {
                xmlDocument.Load(ms);
                Console.WriteLine($"[INFO] Received '{filename}'");
                XmlNode root = xmlDocument.DocumentElement;
                // If the root element is not null, it means the XML document is valid.
                if (root != null)
                {
                    XmlNodeList xmlNodeList = root.SelectNodes("row");
                    // If the xmlNodeList has at least one node, it means the XML document contains valid data.
                    if (xmlNodeList.Count > 0)
                    {
                        if (ParseXmlNodeList(xmlNodeList))
                        {
                            WriteImportedFile(filename);
                        }

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
                // If the root element is null, it means the XML document is invalid
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
        // method is responsible for adding information about the imported file to the database
        private void WriteImportedFile(string filename)
        {
            DataBase.Instance.AddImportedFile(new ImportedFile(filename));
            Console.WriteLine($"[INFO] Imported file '{filename}' proccessed and saved to database.");
        }

        // method is responsible for parsing a list of XML nodes and processing them
        private bool ParseXmlNodeList(XmlNodeList xmlNodeList)
        {
            List<GroupedLoads> groupedLoadsList = new List<GroupedLoads>();
            DateTime date = new DateTime();
            int listindex = -1;
            int count = 0;

            foreach (XmlNode x in xmlNodeList)
            {
                Load load = ParseXmlNode(x);
                // If the load object is not valid skip it
                if (load.TimeStamp == new DateTime() || load.MeasuredValue == -1 || load.ForecastValue == -1)
                {
                    continue;
                }
                //If the date of the current load object is different from the date variable, it means that a new group should be created.
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
            return groupedLoadsList.Count > 0;
        }

        //this method parses an XML node and creates a Load object by extracting values from specific child nodes.
        //It performs parsing for the "TIME_STAMP", "FORECAST_VALUE", and "MEASURED_VALUE" nodes, assigning the parsed values to the corresponding properties of the Load object.
        private Load ParseXmlNode(XmlNode x)
        {
            Load load = new Load();
            DateTime date;
            //Check if date is valid
            if (TryParseNodeValueDate(x.SelectSingleNode("TIME_STAMP"), out date))
            {
                load.TimeStamp = date;
            }

            float forecast;
            //Check if forecast is valid
            if (TryParseNodeValue(x.SelectSingleNode("FORECAST_VALUE"), out forecast))
            {
                load.ForecastValue = forecast;

            }

            float measured;
            //Check if measured is valid
            if (TryParseNodeValue(x.SelectSingleNode("MEASURED_VALUE"), out measured))
            {
                load.MeasuredValue = measured;

            }

            return load;
        }

        //method attempts to parse a date value from an XML node
        private bool TryParseNodeValueDate(XmlNode xmlNode, out DateTime Storage)
        {
            if (xmlNode != null)
            {
                string stringValue = xmlNode.InnerText.Trim();
                // It checks if the resulting string is not empty
                if (stringValue != string.Empty)
                {
                    //If there is a value, it tries to parse that string into a DateTime format using the DateTime.ParseExact method
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
            // If the xmlNode is null, it creates an Audit object for a missing node error and adds it to the database
            else
            {
                Audit audit = new Audit(DateTime.Now, MessageType.Error, "XmlNode is missing.");
                DataBase.Instance.AddAudit(audit);
            }
            // Finally, it assigns a new instance of the DateTime object to the Storage parameter, and the method returns false
            Storage = new DateTime();
            return false;
        }

        private bool TryParseNodeValue(XmlNode xmlNode, out float Storage)
        {
            if (xmlNode != null)
            {
                string stringValue = xmlNode.InnerText.Trim();
                // It checks if the resulting string is not empty, indicating that the node has a value
                if (stringValue != string.Empty)
                {
                    //If there is a value, it tries to parse that string into a float using the float.TryParse method
                    bool ret = float.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out Storage);
                    //If the parsing fails, meaning the string value is not a valid floating-point number,
                    // it creates an Audit object to record the error and adds it to the database
                    if (ret == false)
                    {
                        Audit audit = new Audit(DateTime.Now, MessageType.Warning, $"Invalid value in XmlNode '{xmlNode.Name}'.");
                        DataBase.Instance.AddAudit(audit);
                        return false;
                    }
                    return true;

                }
                //If the string value of the node is empty, it creates an Audit object for an empty node information and adds it to the database
                else
                {
                    Audit audit = new Audit(DateTime.Now, MessageType.Info, $"XmlNode '{xmlNode.Name}' is empty.");
                    DataBase.Instance.AddAudit(audit);
                }
            }
            //If the xmlNode is null, it creates an Audit object for a missing node error and adds it to the database
            else
            {
                Audit audit = new Audit(DateTime.Now, MessageType.Error, "XmlNode is missing.");
                DataBase.Instance.AddAudit(audit);
            }
            //Finally, if the parsing was unsuccessful, it assigns the value -1 to the Storage parameter, and the method returns false
            Storage = -1;
            return false;
        }
    }
}
