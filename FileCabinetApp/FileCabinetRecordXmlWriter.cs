using System;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace FileCabinetApp
{
    /// <summary>
    /// Write to XML file.
    /// </summary>
    public class FileCabinetRecordXmlWriter
    {
        private readonly StreamWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        /// <param name="writer">Input parametr in constructor <see cref="StreamWriter"/>.</param>
        public FileCabinetRecordXmlWriter(StreamWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Write records to the xml file.
        /// </summary>
        /// <param name="records">Input parametr array of record <see cref="FileCabinetRecord"/>.</param>
        public void Write(FileCabinetRecord[] records)
        {
            if (records == null)
            {
                // Console.WriteLine("Records is null!");
                // throw new ArgumentNullException("Records is null!");
                throw new ArgumentNullException($"{records} is null.");
            }

            try
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(this.writer))
                {
                    XDocument document = FileCabinetRecordToXml();
                    foreach (var record in records)
                    {
                        var salareToString = record.Salary.ToString(new CultureInfo("en-US"));
                        document.Root.Add(new XElement(
                            "record",
                            new XAttribute("id", $"{record.Id}"),
                            new XElement(
                            "name",
                            new XAttribute("first", $"{record.FirstName}"),
                            new XAttribute("last", $"{record.LastName}")),
                            new XElement("dateOfBirth", $"{record.DateOfBirth.ToString("yyyy-MMM-dd", new CultureInfo("en-US"))}"),
                            new XElement("sex", $"{record.Sex}"),
                            new XElement("height", $"{record.Height}"),
                            new XElement("salary", $"{salareToString}")));
                    }

                    document.Save(this.writer);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static XDocument FileCabinetRecordToXml()
        {
            XDocument dc = new XDocument(new XDeclaration("1.0", "UTF-8", null), new XElement("records"));
            return dc;
        }
    }
}