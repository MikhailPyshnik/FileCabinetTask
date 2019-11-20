using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Class implement IRecorditerator for work with FileCabinetFilesystemService.
    /// </summary>
    public class FileSystemIterator : IRecorditerator
    {
        private static int maxstringlength;
        private readonly List<long> recordsPositions;
        private int position;
        private FileStream fileStream;
        private int recordsize;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemIterator"/> class.
        /// </summary>
        /// <param name="recordsPositions">Input parametr in constructor (list of positions) <see cref="long"/>.</param>
        /// <param name="fileStream">Input parametr in constructor Filestream <see cref="FileStream"/>.</param>
        /// <param name="recordsize">Input parametr in constructor size of record <see cref="int"/>.</param>
        /// <param name="stringlength">Input parametr in constructor max string length <see cref="int"/>.</param>
        public FileSystemIterator(List<long> recordsPositions, FileStream fileStream, int recordsize, int stringlength)
        {
            this.recordsPositions = recordsPositions ?? throw new ArgumentNullException(nameof(recordsPositions));
            this.position = -1;
            this.fileStream = fileStream ?? throw new ArgumentNullException(nameof(fileStream));
            this.recordsize = recordsize;
            maxstringlength = stringlength;
        }

        /// <summary>
        /// Return records.
        /// </summary>
        /// <returns>Rerord by condition<see cref="FileCabinetRecord"/>.</returns>
        public FileCabinetRecord GetNext()
        {
            var recordBuffer = new byte[this.recordsize];
            this.fileStream.Position = this.recordsPositions[this.position];
            this.fileStream.Read(recordBuffer, 0, this.recordsize);
            var record = BytesToFileCabinetRecord(recordBuffer);
            return record;
        }

        /// <summary>
        /// Get next record.
        /// </summary>
        /// <returns>Rerord if is exist <see cref="bool"/>.</returns>
        public bool HasMore()
        {
            if (++this.position == this.recordsPositions.Count)
            {
                return false;
            }
            else
            {
                return true;
            }
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
    }
}
