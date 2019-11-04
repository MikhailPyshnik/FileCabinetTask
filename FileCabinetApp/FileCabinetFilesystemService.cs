using System;
using System.Collections.ObjectModel;
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
        private const int FirstNameLength = 120;
        private const int MaxNameLength = 120;
        private const int LastNameLength = 120;
        private const int RecordSize = sizeof(short) + sizeof(int) + FirstNameLength + LastNameLength + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(char) + sizeof(short) + sizeof(decimal);
        private static int recordId = 1;
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

            this.fileStream.Seek(0, SeekOrigin.End);
            var b1 = FileCabinetRecordToBytes(fileCabinetRecord);
            this.fileStream.Write(b1, 0, b1.Length);
            this.fileStream.Flush();
            return fileCabinetRecord.Id;
        }

        /// <summary>
        /// Implementation IFileCabinetService EditRecord.
        /// </summary>
        /// <param name="fileCabinetRecord">Input parametr record <see cref="FileCabinetRecord"/>.</param>
        public void EditRecord(FileCabinetRecord fileCabinetRecord)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implementation IFileCabinetService FindByDateOfBirth.
        /// </summary>
        /// <param name="dateofbirth">Input parametr FirstName <see cref="string"/>.</param>
        /// <returns>Rerords by dateofbirth <see cref="FileCabinetRecord"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateofbirth)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implementation IFileCabinetService FindByFirstName.
        /// </summary>
        /// <param name="firstName">Input parametr FirstName <see cref="string"/>.</param>
        /// <returns>Rerords by firstName <see cref="FileCabinetRecord"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implementation IFileCabinetService FindByLastName.
        /// </summary>
        /// <param name="lastName">Input parametr FirstName <see cref="string"/>.</param>
        /// <returns>Rerords by lastName <see cref="FileCabinetRecord"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implementation IFileCabinetService GetRecords.
        /// </summary>
        /// <returns>Rerords <see cref="FileCabinetRecord"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implementation IFileCabinetService GetStat.
        /// </summary>
        /// <returns>Count records <see cref="int"/>.</returns>
        public int GetStat()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implementation IFileCabinetService FindByDateOfBirth.
        /// </summary>
        /// <returns>Rerords by dateofbirth <see cref="FileCabinetServiceSnapshot"/>.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
        }

        private static byte[] FileCabinetRecordToBytes(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord == null)
            {
                throw new ArgumentNullException(nameof(fileCabinetRecord));
            }

            fileCabinetRecord.Id = recordId;
            var bytes = new byte[RecordSize];
            using (var memoryStream = new MemoryStream(bytes))
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                short status = 0;
                binaryWriter.Write(status);
                binaryWriter.Write(fileCabinetRecord.Id);
                Encoding unicode = Encoding.Unicode;
                var firstnameBytes = unicode.GetBytes(fileCabinetRecord.FirstName);
                var nameBuffer = new byte[MaxNameLength];
                var lastnameBytes = unicode.GetBytes(fileCabinetRecord.LastName);
                var lastnameBuffer = new byte[MaxNameLength];

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

            ++recordId;
            return bytes;
        }
    }
}
