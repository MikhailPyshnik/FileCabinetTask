using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Class implement IEnumerable for work with foreach.
    /// </summary>
    public sealed class EnumerableFilesystemCollection : IEnumerable<FileCabinetRecord>
    {
        private static int maxstringlength;
        private readonly List<FileCabinetRecord> records = new List<FileCabinetRecord>();
        private readonly List<long> recordsPositions;
        private int position;
        private FileStream fileStream;
        private int recordsize;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumerableFilesystemCollection"/> class.
        /// </summary>
        /// <param name="recordsPositions">Input parametr in constructor (list of positions) <see cref="long"/>.</param>
        /// <param name="fileStream">Input parametr in constructor Filestream <see cref="FileStream"/>.</param>
        /// <param name="recordsize">Input parametr in constructor size of record <see cref="int"/>.</param>
        /// <param name="stringlength">Input parametr in constructor max string length <see cref="int"/>.</param>
        public EnumerableFilesystemCollection(List<long> recordsPositions, FileStream fileStream, int recordsize, int stringlength)
        {
            this.recordsPositions = recordsPositions ?? throw new ArgumentNullException(nameof(recordsPositions));
            this.position = 0;
            this.fileStream = fileStream ?? throw new ArgumentNullException(nameof(fileStream));
            this.recordsize = recordsize;
            maxstringlength = stringlength;
            this.GetNext();
        }

        /// <summary>
        /// Method return IEnumerator>.
        /// </summary>
        /// <returns>IEnumerable by lastName <see cref="FileCabinetRecord"/>.</returns>
        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            return new RecordEnumerator(this.records);
        }

        /// <summary>
        /// Method return IEnumerator.
        /// </summary>
        /// <returns>Return <see cref="IEnumerator"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator1();
        }

        private static FileCabinetRecord BytesToFileCabinetRecord(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            var fileCabinetRecord = new FileCabinetRecord();

            using (var memoryStream = new MemoryStream(bytes))
            using (var binaryReader = new BinaryReader(memoryStream))
            {
                Encoding unicode = Encoding.Unicode;
                fileCabinetRecord.Status = binaryReader.ReadInt16();
                fileCabinetRecord.Id = binaryReader.ReadInt32();

                var nameBuffer = binaryReader.ReadBytes(maxstringlength);
                string first = unicode.GetString(nameBuffer, 0, maxstringlength);
                fileCabinetRecord.FirstName = first.Trim('\u0000');

                var lastnameBuffer = binaryReader.ReadBytes(maxstringlength);
                string second = unicode.GetString(lastnameBuffer, 0, maxstringlength);
                fileCabinetRecord.LastName = second.Trim('\u0000');

                int year = binaryReader.ReadInt32();
                int month = binaryReader.ReadInt32();
                int day = binaryReader.ReadInt32();
                DateTime someDate = new DateTime(year, month, day);
                fileCabinetRecord.DateOfBirth = someDate;

                fileCabinetRecord.Sex = binaryReader.ReadChar();
                fileCabinetRecord.Height = binaryReader.ReadInt16();
                fileCabinetRecord.Salary = binaryReader.ReadDecimal();
            }

            return fileCabinetRecord;
        }

        private void GetNext()
        {
            var recordBuffer = new byte[this.recordsize];
            foreach (var item in this.recordsPositions)
            {
                this.fileStream.Position = this.recordsPositions[this.position];
                this.fileStream.Read(recordBuffer, 0, this.recordsize);
                var record = BytesToFileCabinetRecord(recordBuffer);
                this.records.Add(record);
                this.position++;
            }
        }

        private IEnumerator<FileCabinetRecord> GetEnumerator1()
        {
            return this.GetEnumerator();
        }
    }
}
