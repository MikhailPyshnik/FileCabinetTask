using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Work with file format db.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int MaxStringLength = 120;
        private const int RecordSize = 278;
        private FileStream fileStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="fileStream">Input parametr in constructor <see cref="FileStream"/>.</param>
        public FileCabinetFilesystemService(FileStream fileStream)
        {
            this.fileStream = fileStream;
        }

        /// <summary>
        /// Gets or sets the Validator of the Program.
        /// </summary>
        /// <value>The Validator of the Program.</value>
        public IValidatorOfParemetrs Validator { get; set; }

        /// <summary>
        /// Implementation IFileCabinetService GetStat.
        /// </summary>
        /// <returns>Count records <see cref="int"/>.</returns>
        public int GetStat()
        {
            long length = new FileInfo(this.fileStream.Name).Length;
            int countRecordInFile = (int)length / 278;
            return countRecordInFile;
        }

        /// <summary>
        /// Implementation IFileCabinetService СreateRecod.
        /// </summary>
        /// <param name="fileCabinetRecord">Input parametr record <see cref="FileCabinetRecord"/>.</param>
        /// <returns>Id <see cref="int"/>.</returns>
        public int CreateRecord(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord == null)
            {
                throw new ArgumentNullException($"{nameof(fileCabinetRecord)} is null!");
            }

            fileCabinetRecord.Id = this.GetStat() + 1;
            var recordBuffer = new byte[RecordSize];
            this.fileStream.Seek(0, SeekOrigin.End);
            var b1 = FileCabinetRecordToBytes(fileCabinetRecord);
            this.fileStream.Write(b1, 0, b1.Length);
            this.fileStream.Flush();

            return fileCabinetRecord.Id;
        }

        /// <summary>
        /// Implementation IFileCabinetService GetRecords.
        /// </summary>
        /// <returns>Rerords <see cref="FileCabinetRecord"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            List<FileCabinetRecord> listRecord = new List<FileCabinetRecord>();
            var recordBuffer = new byte[RecordSize];
            int counteRecordInFile = this.GetStat();
            this.fileStream.Seek(0, SeekOrigin.Begin);
            for (int i = 1; i <= counteRecordInFile; i++)
            {
                this.fileStream.Read(recordBuffer, 0, RecordSize);
                var record = BytesToFileCabinetRecord(recordBuffer);
                listRecord.Add(record);
            }

            return new ReadOnlyCollection<FileCabinetRecord>(listRecord);
        }

        /// <summary>
        /// Implementation IFileCabinetService EditRecord.
        /// </summary>
        /// <param name="fileCabinetRecord">Input parametr record <see cref="FileCabinetRecord"/>.</param>
        public void EditRecord(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord == null)
            {
                throw new ArgumentNullException($"{nameof(fileCabinetRecord)} is null!");
            }

            if (fileCabinetRecord.Id > this.GetStat())
            {
                throw new ArgumentException("Input Id is incorrect value.Id less than count of record in file.");
            }

            int editIdReord = fileCabinetRecord.Id;
            var recordBuffer = new byte[RecordSize];
            int counteRecordInFile = this.GetStat();
            this.fileStream.Seek(0, SeekOrigin.Begin);
            for (int i = 0; i < counteRecordInFile; i++)
            {
                this.fileStream.Read(recordBuffer, 0, RecordSize);
                var bytesToRecord = BytesToFileCabinetRecord(recordBuffer);
                if (bytesToRecord.Id == editIdReord)
                {
                    this.fileStream.Seek(i * 278, SeekOrigin.Begin);
                    bytesToRecord.FirstName = fileCabinetRecord.FirstName;
                    bytesToRecord.LastName = fileCabinetRecord.LastName;
                    bytesToRecord.DateOfBirth = fileCabinetRecord.DateOfBirth;
                    bytesToRecord.Sex = fileCabinetRecord.Sex;
                    bytesToRecord.Height = fileCabinetRecord.Height;
                    bytesToRecord.Salary = fileCabinetRecord.Salary;
                    this.fileStream.Seek(0, SeekOrigin.Current);
                    var recordToBytes = FileCabinetRecordToBytes(bytesToRecord);
                    this.fileStream.Write(recordToBytes, 0, recordToBytes.Length);
                    this.fileStream.Flush();
                    return;
                }
            }
        }

        /// <summary>
        /// Implementation IFileCabinetService FindByFirstName.
        /// </summary>
        /// <param name="firstName">Input parametr FirstName <see cref="string"/>.</param>
        /// <returns>Rerords by firstName <see cref="FileCabinetRecord"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            List<FileCabinetRecord> firstNameList = new List<FileCabinetRecord>();
            ReadOnlyCollection<FileCabinetRecord> list = this.GetRecords();
            foreach (var item in list)
            {
                if (item.FirstName.ToLower(new CultureInfo("en-US")) == firstName.ToLower(new CultureInfo("en-US")))
                {
                    firstNameList.Add(item);
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(firstNameList);
        }

        /// <summary>
        /// Implementation IFileCabinetService FindByLastName.
        /// </summary>
        /// <param name="lastName">Input parametr FirstName <see cref="string"/>.</param>
        /// <returns>Rerords by lastName <see cref="FileCabinetRecord"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            List<FileCabinetRecord> lastNameList = new List<FileCabinetRecord>();
            ReadOnlyCollection<FileCabinetRecord> list = this.GetRecords();
            foreach (var item in list)
            {
                if (item.LastName.ToLower(new CultureInfo("en-US")) == lastName.ToLower(new CultureInfo("en-US")))
                {
                    lastNameList.Add(item);
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(lastNameList);
        }

        /// <summary>
        /// Implementation IFileCabinetService FindByDateOfBirth.
        /// </summary>
        /// <param name="dateofbirth">Input parametr FirstName <see cref="string"/>.</param>
        /// <returns>Rerords by dateofbirth <see cref="FileCabinetRecord"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateofbirth)
        {
            List<FileCabinetRecord> dayOfBirthList = new List<FileCabinetRecord>();
            ReadOnlyCollection<FileCabinetRecord> list = this.GetRecords();
            foreach (var item in list)
            {
                string temp = item.DateOfBirth.ToString("yyyy-MMM-dd", new CultureInfo("en-US"));
                if (temp.ToLower(new CultureInfo("en-US")) == dateofbirth.ToLower(new CultureInfo("en-US")))
                {
                    dayOfBirthList.Add(item);
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(dayOfBirthList);
        }

        /// <summary>
        /// Implementation IFileCabinetService FindByDateOfBirth.
        /// </summary>
        /// <returns>Rerords by dateofbirth <see cref="FileCabinetServiceSnapshot"/>.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implementation IFileCabinetService Restore.
        /// </summary>
        /// <param name="fileCabinetServiceSnapshot">Input parametr fileCabinetServiceSnapshot <see cref="FileCabinetServiceSnapshot"/>.</param>
        public void Restore(FileCabinetServiceSnapshot fileCabinetServiceSnapshot)
        {
            throw new NotImplementedException();
        }

        private static byte[] FileCabinetRecordToBytes(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord == null)
            {
                throw new ArgumentNullException(nameof(fileCabinetRecord));
            }

            var bytes = new byte[RecordSize];
            using (var memoryStream = new MemoryStream(bytes))
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                binaryWriter.Write(fileCabinetRecord.Status);
                binaryWriter.Write(fileCabinetRecord.Id);
                Encoding unicode = Encoding.Unicode;
                var firstnameBytes = unicode.GetBytes(fileCabinetRecord.FirstName);
                var nameBuffer = new byte[MaxStringLength];
                var lastnameBytes = unicode.GetBytes(fileCabinetRecord.LastName);
                var lastnameBuffer = new byte[MaxStringLength];

                int nameLength = firstnameBytes.Length;
                int lastnameLength = lastnameBytes.Length;

                Array.Copy(firstnameBytes, 0, nameBuffer, 0, nameLength);
                Array.Copy(lastnameBytes, 0, lastnameBuffer, 0, lastnameLength);

                binaryWriter.Write(nameBuffer);
                binaryWriter.Write(lastnameBuffer);
                int year = fileCabinetRecord.DateOfBirth.Year;
                int mounth = fileCabinetRecord.DateOfBirth.Month;
                int day = fileCabinetRecord.DateOfBirth.Day;
                binaryWriter.Write(year);
                binaryWriter.Write(mounth);
                binaryWriter.Write(day);
                binaryWriter.Write(fileCabinetRecord.Sex);
                binaryWriter.Write(fileCabinetRecord.Height);
                binaryWriter.Write(fileCabinetRecord.Salary);
            }

            return bytes;
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

                var nameBuffer = binaryReader.ReadBytes(MaxStringLength);
                string first = unicode.GetString(nameBuffer, 0, MaxStringLength);
                fileCabinetRecord.FirstName = first.Trim('\u0000');

                var lastnameBuffer = binaryReader.ReadBytes(MaxStringLength);
                string second = unicode.GetString(lastnameBuffer, 0, MaxStringLength);
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
