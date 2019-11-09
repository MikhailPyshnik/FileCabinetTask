using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// Read from XML file.
    /// </summary>
    public class FileCabinetRecordXMLReader
    {
        private readonly StreamReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXMLReader"/> class.
        /// </summary>
        /// <param name="reader">Input parametr in constructor <see cref="StreamReader"/>.</param>
        public FileCabinetRecordXMLReader(StreamReader reader)
        {
            this.reader = reader;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <returns>Import rerords by IList.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            List<FileCabinetRecord> listImport = new List<FileCabinetRecord>();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(this.reader);
            var a = xmlDoc.DocumentElement;
            XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("record");
            foreach (XmlNode node in nodeList)
            {
                listImport.Add(StreamRiderFromXML(node));
            }

            return listImport;
        }

        private static FileCabinetRecord StreamRiderFromXML(XmlNode node)
        {
            CultureInfo provider = new CultureInfo("en-US");
            FileCabinetRecord fileCabinetRecord = new FileCabinetRecord();
            fileCabinetRecord.Id = Convert.ToInt32(node.Attributes["id"].InnerText, provider);
            fileCabinetRecord.FirstName = node.SelectSingleNode("name").Attributes["first"].InnerText;
            fileCabinetRecord.LastName = node.SelectSingleNode("name").Attributes["last"].InnerText;
            fileCabinetRecord.DateOfBirth = DateTime.ParseExact(node.SelectSingleNode("dateOfBirth").InnerText, "dd/MM/yyyy", provider);
            fileCabinetRecord.Sex = Convert.ToChar(node.SelectSingleNode("sex").InnerText, provider);
            fileCabinetRecord.Height = Convert.ToInt16(node.SelectSingleNode("height").InnerText, provider);
            fileCabinetRecord.Salary = Convert.ToDecimal(node.SelectSingleNode("salary").InnerText, provider);

            return fileCabinetRecord;
        }
    }
}