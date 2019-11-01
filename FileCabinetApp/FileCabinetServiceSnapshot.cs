using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
    }
}