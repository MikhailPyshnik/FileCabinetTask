using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// Implementation pattern Memento.
    /// </summary>
    public class FileCabinetServiceSnapshot
    {
        private readonly FileCabinetRecord[] records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">Input parametr in constructor <see cref="FileCabinetRecord"/>.</param>
        public FileCabinetServiceSnapshot(FileCabinetRecord[] records)
        {
            this.records = records;
        }

        /// <summary>
        /// Gets the records.
        /// </summary>
        /// <value>The ReadOnlyCollection records.</value>
        public ReadOnlyCollection<FileCabinetRecord> Records { get; private set; }

        /// <summary>
        /// Set snapshot of FileCabinetServiceSnapshot.
        /// </summary>
        /// <returns>Value <see cref="FileCabinetServiceSnapshot"/>.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            FileCabinetServiceSnapshot fileCabinetServiceSnapshot = new FileCabinetServiceSnapshot(this.records);
            return fileCabinetServiceSnapshot;
        }

        /// <summary>
        /// Save the records to csv.
        /// </summary>
        /// <param name="streamWriter">Input parametr record <see cref="StreamWriter"/>.</param>
        public void SaveToCsv(StreamWriter streamWriter)
        {
            FileCabinetRecordCsvWriter fileCabinetRecordCsvWriter = new FileCabinetRecordCsvWriter(streamWriter);
            fileCabinetRecordCsvWriter.Write(this.MakeSnapshot().records);
        }

        /// <summary>
        /// Save the records to XML.
        /// </summary>
        /// <param name="streamWriter">Input parametr record <see cref="StreamWriter"/>.</param>
        public void SaveToXml(StreamWriter streamWriter)
        {
            FileCabinetRecordXmlWriter fileCabinetRecordXmlWriter = new FileCabinetRecordXmlWriter(streamWriter);
            fileCabinetRecordXmlWriter.Write(this.MakeSnapshot().records);
        }

        /// <summary>
        /// Read records from CSV.
        /// </summary>
        /// <param name="reader">Input parametr record <see cref="StreamReader"/>.</param>
        public void LoadFromCsv(StreamReader reader)
        {
            FileCabinetRecordCSVReader fileCabinetRecordCSVReader = new FileCabinetRecordCSVReader(reader);
            var importedRecords = fileCabinetRecordCSVReader.ReadAll();
            List<FileCabinetRecord> listImport = new List<FileCabinetRecord>(importedRecords);
            var importedRecordsCollection = new ReadOnlyCollection<FileCabinetRecord>(listImport);
            this.Records = importedRecordsCollection;
        }

        /// <summary>
        /// Read records from XML.
        /// </summary>
        /// <param name="reader">Input parametr record <see cref="StreamReader"/>.</param>
        public void LoadFromXML(StreamReader reader)
        {
            FileCabinetRecordXMLReader fileCabinetRecordXMLReader = new FileCabinetRecordXMLReader(reader);
            var importedRecords = fileCabinetRecordXMLReader.ReadAll();
            List<FileCabinetRecord> listImport = new List<FileCabinetRecord>(importedRecords);
            var importedRecordsCollection = new ReadOnlyCollection<FileCabinetRecord>(listImport);
            this.Records = importedRecordsCollection;
        }
    }
}