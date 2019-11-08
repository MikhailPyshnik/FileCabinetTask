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
        /// Set validate parametrs for FileCabinetRecord.
        /// </summary>
        /// <param name="streamWriter">Input parametr record <see cref="StreamWriter"/>.</param>
        public void SaveToXml(StreamWriter streamWriter)
        {
            FileCabinetRecordXmlWriter fileCabinetRecordXmlWriter = new FileCabinetRecordXmlWriter(streamWriter);
            fileCabinetRecordXmlWriter.Write(this.MakeSnapshot().records);
        }

        /// <summary>
        /// Set validate parametrs for FileCabinetRecord.
        /// </summary>
        /// <param name="reader">Input parametr record <see cref="StreamReader"/>.</param>
        public void LoadFromCsv(StreamReader reader)
        {
            FileCabinetRecordCSVReader fileCabinetRecordCSVReader = new FileCabinetRecordCSVReader(reader);
            var il = fileCabinetRecordCSVReader.ReadAll();
            List<FileCabinetRecord> listImport = new List<FileCabinetRecord>(il);
            var readonl = new ReadOnlyCollection<FileCabinetRecord>(listImport);
            this.Records = readonl;
        }
    }
}