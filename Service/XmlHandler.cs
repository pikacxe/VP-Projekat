using Common.FileHandling;
using Common.CustomEvents;
using InMemoryDB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;


namespace Service
{
    public class XmlHandler : CustomEventSource<List<GroupedLoads>>
    {
        // method is responsible for reading and processing an XML file
        public void ReadXmlFile(MemoryStream memoryStream, string filename, out string message)
        {
            XmlDocument xmlDocument = new XmlDocument();
            message = string.Empty;
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
                        if (ParseXmlNodeList(xmlNodeList, out message))
                        {
                            WriteImportedFile(filename);
                        }
                    }
                    else
                    {
                        message = $"'{filename}' xml document is empty or does not contain 'row' tags.";
                        Audit audit = new Audit(DateTime.Now, MessageType.Error, message);
                        DataBase.Instance.AddAudit(audit);
                    }
                }
                // If the root element is null, it means the XML document is invalid
                else
                {
                    message = $"Invalid xml document '{filename}'";
                    Audit audit = new Audit(DateTime.Now, MessageType.Error, message);
                    DataBase.Instance.AddAudit(audit);
                }
                ms.Dispose();
                ms.Close();
            }
        }
        // method is responsible for adding information about the imported file to the database
        private void WriteImportedFile(string filename)
        {
            DataBase.Instance.AddImportedFile(new ImportedFile(filename));
            Console.WriteLine($"[INFO] Imported file '{filename}' processed and saved to database.");
        }

        // method is responsible for parsing a list of XML nodes and processing them
        private bool ParseXmlNodeList(XmlNodeList xmlNodeList, out string message)
        {
            List<GroupedLoads> groupedLoadsList = new List<GroupedLoads>();
            DateTime date = new DateTime();
            int listindex = -1;
            int count = 0;
            int cntFailed = 0;
            message = string.Empty;

            foreach (XmlNode x in xmlNodeList)
            {
                count++;
                Load load = ParseXmlNode(x, count);

                // If the load object is not valid skip it
                if (load.TimeStamp == new DateTime() || load.MeasuredValue == -1 || load.ForecastValue == -1)
                {
                    cntFailed++;
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
            }
            message = $"[INFO] Processed '{count}' Load(s)" + (cntFailed > 0 ? $", out of which '{cntFailed}' Load(s) failed." : ".");
            Console.WriteLine(message);
            RaiseCustomEvent(groupedLoadsList);
            return groupedLoadsList.Count > 0;
        }

        //this method parses an XML node and creates a Load object by extracting values from specific child nodes.
        //It performs parsing for the "TIME_STAMP", "FORECAST_VALUE", and "MEASURED_VALUE" nodes, assigning the parsed values to the corresponding properties of the Load object.
        private Load ParseXmlNode(XmlNode x, int index)
        {
            string errorMessage = string.Empty;
            Load load = new Load();
            DateTime date;
            //Check if date is valid
            string temp = string.Empty;
            if ((temp = TryParseNodeValueDate(x.SelectSingleNode("TIME_STAMP"), out date)) == string.Empty)
            {
                load.TimeStamp = date;
            }
            else
            {
                errorMessage += temp;
                temp = string.Empty;
            }

            float forecast;
            //Check if forecast is valid
            if ((temp = TryParseNodeValue(x.SelectSingleNode("FORECAST_VALUE"), out forecast)) == string.Empty)
            {
                load.ForecastValue = forecast;

            }
            else
            {
                errorMessage += temp;
                temp = string.Empty;
            }

            float measured;
            //Check if measured is valid
            if ((temp = TryParseNodeValue(x.SelectSingleNode("MEASURED_VALUE"), out measured)) == string.Empty)
            {
                load.MeasuredValue = measured;

            }
            else
            {
                errorMessage += temp;
                temp = string.Empty;
            }
            if(errorMessage != string.Empty)
            {
                Audit audit =new Audit(DateTime.Now, MessageType.Error, $"[Load {index}] - {errorMessage}");
                DataBase.Instance.AddAudit(audit);
            }

            return load;
        }

        //method attempts to parse a date value from an XML node
        private string TryParseNodeValueDate(XmlNode xmlNode, out DateTime Storage)
        {
            string errorMessage = string.Empty;
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
                        return errorMessage;
                    }
                    catch (FormatException)
                    {
                        errorMessage += $"Invalid format in XmlNode '{xmlNode.Name}'. ";
                    }
                }
                else
                {
                    errorMessage += $"XmlNode '{xmlNode.Name}' is empty. ";
                }
            }
            // If the xmlNode is null add apropriate message
            else
            {
                errorMessage += $"XmlNode 'TIME_STAMP' is missing. "; 
            }
            // Finally, it assigns a new instance of the DateTime object to the Storage parameter, and the method returns false
            Storage = new DateTime();
            return errorMessage;
        }

        private string TryParseNodeValue(XmlNode xmlNode, out float Storage)
        {
            string errorMessage = string.Empty;
            Storage = -1;
            if (xmlNode != null)
            {
                string stringValue = xmlNode.InnerText.Trim();
                // It checks if the resulting string is not empty, indicating that the node has a value
                if (stringValue != string.Empty)
                {
                    //If there is a value, it tries to parse that string into a float using the float.TryParse method
                    bool ret = float.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out Storage);
                    //If the parsing fails, meaning the string value is not a valid floating-point number,
                    // record the error
                    if (ret == false)
                    {
                        errorMessage += $"Invalid value in XmlNode '{xmlNode.Name}'. ";
                    }
                }
                //If the string value of the node is empty add apropriate message
                else
                {
                    errorMessage +=$"XmlNode '{xmlNode.Name}' is empty. ";
                }
            }
            //If the xmlNode is null add apropriate message
            else
            {
                errorMessage += $"XmlNode 'FORECAST_VALUE' or 'MEASURED_VALUE' is missing. ";
            }
            return errorMessage;
        }
    }
}
