using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Work with file format db.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int MAXSTRINGLENGTH = 120;
        private const int SETTHIRDBITTRUE = 4;
        private const int RECORDSIZE = 278;
        private readonly IRecordValidator validator;

        private readonly Dictionary<string, List<long>> firstNameDictionary = new Dictionary<string, List<long>>(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, List<long>> lastNameDictionary = new Dictionary<string, List<long>>(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, List<long>> dateOfBirthDictionary = new Dictionary<string, List<long>>(StringComparer.OrdinalIgnoreCase);

        private FileStream fileStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="fileStream">Input parametr in constructor <see cref="FileStream"/>.</param>
        /// <param name="recordValidator">Input parametr in constructor <see cref="IRecordValidator"/>.</param>
        public FileCabinetFilesystemService(FileStream fileStream, IRecordValidator recordValidator)
        {
            this.fileStream = fileStream ?? throw new ArgumentNullException(nameof(fileStream));
            this.validator = recordValidator ?? throw new ArgumentNullException(nameof(recordValidator));
            this.GetPositionOfRecordsList();
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
        public Tuple<int, int> GetStat()
        {
            int countRecordInFile = this.GetCountAllRecordssFromFile();
            int countDeleteRecord = this.GetDdeletedRecordList().Count;
            return new Tuple<int, int>(countRecordInFile, countDeleteRecord);
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

            this.validator.ValidateParametrs(fileCabinetRecord);
            fileCabinetRecord.Id = this.GetMaxIdInNotDeletedRecordsFromFile() + 1;
            this.fileStream.Seek(0, SeekOrigin.End);
            long seek = this.fileStream.Position;
            var b1 = FileCabinetRecordToBytes(fileCabinetRecord);
            this.fileStream.Write(b1, 0, b1.Length);
            this.fileStream.Flush();

            this.AddRecordToDictionary(fileCabinetRecord, seek);

            return fileCabinetRecord.Id;
        }

        /// <summary>
        /// Implementation IFileCabinetService GetRecords.
        /// </summary>
        /// <returns>Rerords <see cref="FileCabinetRecord"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            List<FileCabinetRecord> listRecord = new List<FileCabinetRecord>();
            var recordBuffer = new byte[RECORDSIZE];
            int counteRecordInFile = this.GetCountAllRecordssFromFile();
            this.fileStream.Seek(0, SeekOrigin.Begin);
            for (int i = 1; i <= counteRecordInFile; i++)
            {
                this.fileStream.Read(recordBuffer, 0, RECORDSIZE);
                if (recordBuffer[0] == SETTHIRDBITTRUE)
                {
                    continue;
                }
                else
                {
                    var record = BytesToFileCabinetRecord(recordBuffer);
                    listRecord.Add(record);
                }
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

            this.validator.ValidateParametrs(fileCabinetRecord);
            int editIdReord = fileCabinetRecord.Id;
            var recordBuffer = new byte[RECORDSIZE];
            int counteRecordInFile = this.GetCountAllRecordssFromFile();
            this.fileStream.Seek(0, SeekOrigin.Begin);
            for (int i = 0; i < counteRecordInFile; i++)
            {
                this.fileStream.Read(recordBuffer, 0, RECORDSIZE);
                if (recordBuffer[0] == SETTHIRDBITTRUE)
                {
                    continue;
                }
                else
                {
                    var bytesToRecord = BytesToFileCabinetRecord(recordBuffer);
                    if (bytesToRecord.Id == editIdReord)
                    {
                        string oldFirstName = bytesToRecord.FirstName;
                        string oldLastName = bytesToRecord.LastName;
                        string oldDateOfBirth = bytesToRecord.DateOfBirth.ToString("yyyy-MMM-dd", new CultureInfo("en-US"));

                        this.fileStream.Seek(i * 278, SeekOrigin.Begin);
                        long seek = this.fileStream.Position;
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
                        this.ChangeRecordToDictionary(seek, bytesToRecord, oldFirstName, oldLastName, oldDateOfBirth);
                        return;
                    }
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
            var recordBuffer = new byte[RECORDSIZE];
            var result = this.firstNameDictionary[firstName];

            foreach (var item in result)
            {
                this.fileStream.Position = item;
                this.fileStream.Read(recordBuffer, 0, RECORDSIZE);
                var record = BytesToFileCabinetRecord(recordBuffer);
                firstNameList.Add(record);
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
            var recordBuffer = new byte[RECORDSIZE];
            var result = this.lastNameDictionary[lastName];

            foreach (var item in result)
            {
                this.fileStream.Position = item;
                this.fileStream.Read(recordBuffer, 0, RECORDSIZE);
                var record = BytesToFileCabinetRecord(recordBuffer);
                lastNameList.Add(record);
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
            List<FileCabinetRecord> dateofbirthList = new List<FileCabinetRecord>();
            var recordBuffer = new byte[RECORDSIZE];
            var result = this.dateOfBirthDictionary[dateofbirth];

            foreach (var item in result)
            {
                this.fileStream.Position = item;
                this.fileStream.Read(recordBuffer, 0, RECORDSIZE);
                var record = BytesToFileCabinetRecord(recordBuffer);
                dateofbirthList.Add(record);
            }

            return new ReadOnlyCollection<FileCabinetRecord>(dateofbirthList);
        }

        /// <summary>
        /// Implementation IFileCabinetService MakeSnapshot.
        /// </summary>
        /// <returns>Rerords by dateofbirth <see cref="FileCabinetServiceSnapshot"/>.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            var records = this.GetRecords();
            List<FileCabinetRecord> importedFromDbToList = new List<FileCabinetRecord>(records);
            FileCabinetRecord[] array = importedFromDbToList.ToArray();
            FileCabinetServiceSnapshot fileCabinetServiceSnapshot = new FileCabinetServiceSnapshot(array);
            return fileCabinetServiceSnapshot;
        }

        /// <summary>
        /// Implementation IFileCabinetService Restore.
        /// </summary>
        /// <param name="fileCabinetServiceSnapshot">Input parametr fileCabinetServiceSnapshot <see cref="FileCabinetServiceSnapshot"/>.</param>
        public void Restore(FileCabinetServiceSnapshot fileCabinetServiceSnapshot)
        {
            if (fileCabinetServiceSnapshot == null)
            {
                throw new ArgumentNullException($"{nameof(fileCabinetServiceSnapshot)} is null");
            }

            var importRecords = fileCabinetServiceSnapshot.Records;

            List<FileCabinetRecord> listImport = new List<FileCabinetRecord>(importRecords);

            List<FileCabinetRecord> validatedList = this.GetValidFileCabinetRecords(listImport);

            int countImportRecord = validatedList.Count;

            this.UpdateListAfterImport(validatedList);

            Console.WriteLine($"{countImportRecord} records were add to FileCabinetMemoryService");
        }

        /// <summary>
        /// Implementation IFileCabinetService Remove.
        /// </summary>
        /// <param name="id">Input parametr id of record <see cref="int"/>.</param>
        public void Remove(int id)
        {
            var a = this.GetAllRecordsFromFile();
            List<FileCabinetRecord> recordToList = new List<FileCabinetRecord>(a);
            int curent = 0;
            int removeIdRecord = id;
            var recordBuffer = new byte[RECORDSIZE];
            int counteRecordInFile = this.GetCountAllRecordssFromFile();
            this.fileStream.Seek(0, SeekOrigin.Begin);
            foreach (var revoveId in recordToList)
            {
                this.fileStream.Read(recordBuffer, 0, RECORDSIZE);
                if (revoveId.Id == id)
                {
                    this.fileStream.Seek(curent * 278, SeekOrigin.Begin);
                    this.fileStream.Seek(0, SeekOrigin.Current);
                    long seek = this.fileStream.Position;
                    var removeRecord = recordBuffer;
                    removeRecord[0] = 4;
                    this.fileStream.Write(removeRecord, 0, removeRecord.Length);
                    this.fileStream.Flush();
                    this.RemoveRecordFromDictionary(revoveId, seek);

                    break;
                }

                ++curent;
            }
        }

        /// <summary>
        /// Implementation IFileCabinetService Purge.
        /// </summary>
        // /// <param name="id">Input parametr id of record <see cref="int"/>.</param>
        public void Purge()
        {
            int countDeletedRecords = this.GetDdeletedRecordList().Count;
            int countRecordsInFile = this.GetCountAllRecordssFromFile();

            this.WriteRecordsToFile(this.GetNotDeletedRecordsList());

            Console.WriteLine($"Data file processing is completed: {countDeletedRecords} of {countRecordsInFile} records were purged.");
        }

        private static byte[] FileCabinetRecordToBytes(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord == null)
            {
                throw new ArgumentNullException(nameof(fileCabinetRecord));
            }

            var bytes = new byte[RECORDSIZE];
            using (var memoryStream = new MemoryStream(bytes))
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                binaryWriter.Write(fileCabinetRecord.Status);
                binaryWriter.Write(fileCabinetRecord.Id);
                Encoding unicode = Encoding.Unicode;
                var firstnameBytes = unicode.GetBytes(fileCabinetRecord.FirstName);
                var nameBuffer = new byte[MAXSTRINGLENGTH];
                var lastnameBytes = unicode.GetBytes(fileCabinetRecord.LastName);
                var lastnameBuffer = new byte[MAXSTRINGLENGTH];

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

                var nameBuffer = binaryReader.ReadBytes(MAXSTRINGLENGTH);
                string first = unicode.GetString(nameBuffer, 0, MAXSTRINGLENGTH);
                fileCabinetRecord.FirstName = first.Trim('\u0000');

                var lastnameBuffer = binaryReader.ReadBytes(MAXSTRINGLENGTH);
                string second = unicode.GetString(lastnameBuffer, 0, MAXSTRINGLENGTH);
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

        private bool ValidateInput<T>(string inputValue, Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            T value;
            bool result = false;

            var input = inputValue;
            var conversionResult = converter(input);

            if (!conversionResult.Item1)
            {
                return result;
            }

            value = conversionResult.Item3;

            var validationResult = validator(value);
            if (!validationResult.Item1)
            {
                return result;
            }

            result = true;
            return result;
        }

        private bool ValidateImportParametr(FileCabinetRecord fileCabinetRecord)
        {
            CultureInfo provider = new CultureInfo("en-US");

            bool isFirstNameValid = this.ValidateInput(fileCabinetRecord.FirstName, this.Validator.FirstNameConverter, this.Validator.FirstNameValidator);
            bool isLastNameValid = this.ValidateInput(fileCabinetRecord.LastName, this.Validator.LastNameConverter, this.Validator.LastNameValidator);
            bool isDateOfBirthValid = this.ValidateInput(fileCabinetRecord.DateOfBirth.ToString("dd/MM/yyyy", new CultureInfo("en-US")), this.Validator.DayOfBirthConverter, this.Validator.DayOfBirthValidator);
            bool isSexValid = this.ValidateInput(fileCabinetRecord.Sex.ToString(provider), this.Validator.SexConverter, this.Validator.SexValidator);
            bool isHeigthValid = this.ValidateInput(fileCabinetRecord.Height.ToString(provider), this.Validator.HeightConverter, this.Validator.HeightValidator);
            bool isSalaryValid = this.ValidateInput(fileCabinetRecord.Salary.ToString(provider), this.Validator.SalaryConverter, this.Validator.SalaryValidator);

            return isFirstNameValid && isLastNameValid && isDateOfBirthValid && isSexValid && isHeigthValid && isSalaryValid;
        }

        private List<FileCabinetRecord> GetValidFileCabinetRecords(List<FileCabinetRecord> listImport)
        {
            List<FileCabinetRecord> validatedList = new List<FileCabinetRecord>();

            foreach (var item in listImport)
            {
                if (this.ValidateImportParametr(item))
                {
                    validatedList.Add(item);
                }
                else
                {
                    Console.WriteLine($"{item.Id} records has incorrect value.This record was skipped.");
                }
            }

            return validatedList;
        }

        private void UpdateListAfterImport(List<FileCabinetRecord> validateList)
        {
            List<FileCabinetRecord> listRecord = new List<FileCabinetRecord>();
            List<FileCabinetRecord> deletedRecordList = this.GetDdeletedRecordList();
            List<FileCabinetRecord> validatedRecordsWithIdFromDeletedRecordList = new List<FileCabinetRecord>();
            var recordBuffer = new byte[RECORDSIZE];
            int counteRecordInFile = this.GetCountAllRecordssFromFile();
            this.fileStream.Seek(0, SeekOrigin.Begin);
            for (int i = 1; i <= counteRecordInFile; i++)
            {
                this.fileStream.Read(recordBuffer, 0, RECORDSIZE);
                var record = BytesToFileCabinetRecord(recordBuffer);
                listRecord.Add(record);
            }

            // Add records from notDeletedRecordsList to validateList by id.
            foreach (var it in listRecord)
            {
                if (it.Status == 4)
                {
                    validateList.Add(it);
                }

                if (!validateList.Exists(item => item.Id == it.Id))
                {
                    validateList.Add(it);
                }
            }

            validateList = validateList.OrderBy(item => item.Id).ToList();

            // Get records by id form validateList if id exist in deletedRecordList.
            foreach (var item in validateList)
            {
                if (deletedRecordList.Exists(item1 => item1.Id == item.Id))
                {
                    validatedRecordsWithIdFromDeletedRecordList.Add(item);
                }
            }

            // Remove records by id form validateList if id exist in deletedRecordList.
            foreach (var item in deletedRecordList)
            {
                if (validateList.Exists(item1 => item1.Id == item.Id))
                {
                    validateList.RemoveAll(item1 => item1.Id == item.Id);
                }
            }

            listRecord.Clear();
            listRecord.AddRange(validateList);

            listRecord.AddRange(validatedRecordsWithIdFromDeletedRecordList);

            this.fileStream.SetLength(0);
            this.fileStream.Flush();

            // Write in cabinet-records.db all records.
            foreach (var item in listRecord)
            {
                this.fileStream.Seek(0, SeekOrigin.End);
                var b1 = FileCabinetRecordToBytes(item);
                this.fileStream.Write(b1, 0, b1.Length);
                this.fileStream.Flush();
            }
        }

        private ReadOnlyCollection<FileCabinetRecord> GetAllRecordsFromFile()
        {
            List<FileCabinetRecord> listRecord = new List<FileCabinetRecord>();
            var recordBuffer = new byte[RECORDSIZE];
            int counteRecordInFile = this.GetCountAllRecordssFromFile();
            this.fileStream.Seek(0, SeekOrigin.Begin);
            for (int i = 1; i <= counteRecordInFile; i++)
            {
                this.fileStream.Read(recordBuffer, 0, RECORDSIZE);
                var record = BytesToFileCabinetRecord(recordBuffer);
                listRecord.Add(record);
            }

            return new ReadOnlyCollection<FileCabinetRecord>(listRecord);
        }

        private List<FileCabinetRecord> GetNotDeletedRecordsList()
        {
            List<FileCabinetRecord> notDeletedRecordsList = new List<FileCabinetRecord>();
            var recordBuffer = new byte[RECORDSIZE];
            int counteRecordInFile = this.GetCountAllRecordssFromFile();
            this.fileStream.Seek(0, SeekOrigin.Begin);
            for (int i = 1; i <= counteRecordInFile; i++)
            {
                this.fileStream.Read(recordBuffer, 0, RECORDSIZE);
                if (recordBuffer[0] != SETTHIRDBITTRUE)
                {
                    var record = BytesToFileCabinetRecord(recordBuffer);
                    notDeletedRecordsList.Add(record);
                }
            }

            return notDeletedRecordsList;
        }

        private List<FileCabinetRecord> GetDdeletedRecordList()
        {
            List<FileCabinetRecord> deletedRecordList = new List<FileCabinetRecord>();

            var recordBuffer = new byte[RECORDSIZE];
            int counteRecordInFile = this.GetCountAllRecordssFromFile();
            this.fileStream.Seek(0, SeekOrigin.Begin);
            for (int i = 1; i <= counteRecordInFile; i++)
            {
                this.fileStream.Read(recordBuffer, 0, RECORDSIZE);
                if (recordBuffer[0] == SETTHIRDBITTRUE)
                {
                    var record = BytesToFileCabinetRecord(recordBuffer);
                    deletedRecordList.Add(record);
                }
            }

            return deletedRecordList;
        }

        private List<FileCabinetRecord> GetValidRecordListWithNotDeletedRecordsList(List<FileCabinetRecord> list)
        {
            List<FileCabinetRecord> deletedRecordList = new List<FileCabinetRecord>();

            var recordBuffer = new byte[RECORDSIZE];
            int counteRecordInFile = this.GetCountAllRecordssFromFile();
            this.fileStream.Seek(0, SeekOrigin.Begin);
            for (int i = 1; i <= counteRecordInFile; i++)
            {
                this.fileStream.Read(recordBuffer, 0, RECORDSIZE);
                if (recordBuffer[0] == SETTHIRDBITTRUE)
                {
                    var record = BytesToFileCabinetRecord(recordBuffer);
                    deletedRecordList.Add(record);
                }
            }

            return deletedRecordList;
        }

        private void WriteRecordsToFile(List<FileCabinetRecord> list)
        {
            this.fileStream.SetLength(0);
            this.fileStream.Flush();

            foreach (var item in list)
            {
                this.fileStream.Seek(0, SeekOrigin.End);
                var b1 = FileCabinetRecordToBytes(item);
                this.fileStream.Write(b1, 0, b1.Length);
                this.fileStream.Flush();
            }
        }

        private int GetCountAllRecordssFromFile()
        {
            long length = new FileInfo(this.fileStream.Name).Length;
            int countRecordInFile = (int)length / 278;
            return countRecordInFile;
        }

        private int GetMaxIdInNotDeletedRecordsFromFile()
        {
            int maxIdInFile = 0;

            List<FileCabinetRecord> notDeletedRecordsList = this.GetNotDeletedRecordsList();
            foreach (var item in notDeletedRecordsList)
            {
                if (item.Id > maxIdInFile)
                {
                    maxIdInFile = item.Id;
                }
            }

            return maxIdInFile;
        }

        private void AddRecordToDictionary(FileCabinetRecord record, long position)
        {
            this.AddRecordToFirstNameDictionary(record.FirstName, position);
            this.AddRecordToLastNameDictionary(record.LastName, position);
            this.AddRecordToDateOfBirthDictionary(record, position);
        }

        private void AddRecordToFirstNameDictionary(string firstName, long position)
        {
            if (this.firstNameDictionary.ContainsKey(firstName.ToLower(new CultureInfo("en-US"))))
            {
                this.firstNameDictionary[firstName].Add(position);
            }
            else
            {
                List<long> listpositions = new List<long>();
                listpositions.Add(position);
                this.firstNameDictionary.Add(firstName, listpositions);
            }
        }

        private void AddRecordToLastNameDictionary(string lastName, long position)
        {
            if (this.lastNameDictionary.ContainsKey(lastName.ToLower(new CultureInfo("en-US"))))
            {
                this.firstNameDictionary[lastName].Add(position);
            }
            else
            {
                List<long> listpositions = new List<long>();
                listpositions.Add(position);
                this.lastNameDictionary.Add(lastName, listpositions);
            }
        }

        private void AddRecordToDateOfBirthDictionary(FileCabinetRecord record, long position)
        {
            string tempDateOfBirh = record.DateOfBirth.ToString("yyyy-MMM-dd", new CultureInfo("en-US"));
            if (this.dateOfBirthDictionary.ContainsKey(tempDateOfBirh))
            {
                this.dateOfBirthDictionary[tempDateOfBirh].Add(position);
            }
            else
            {
                List<long> listpositions = new List<long>();
                listpositions.Add(position);
                this.dateOfBirthDictionary.Add(tempDateOfBirh, listpositions);
            }
        }

        private void ChangeRecordToDictionary(long position, FileCabinetRecord item, string oldFirstName, string oldLastName, string oldDateOfBirth) // FileCabinetRecord item
        {
            this.ChangeRecordInFirstNameDictionary(position, oldFirstName, item.FirstName);
            this.ChangeRecordInLastNameDictionary(position, oldLastName, item.LastName);
            this.ChangeRecordInDateOfBirthDictionary(position, oldDateOfBirth, item.DateOfBirth);
        }

        private void ChangeRecordInFirstNameDictionary(long position, string oldFirstName, string firstName)
        {
            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                this.firstNameDictionary[oldFirstName].Remove(position);
                this.firstNameDictionary[firstName].Add(position);
            }
            else
            {
                this.firstNameDictionary[oldFirstName].Remove(position);

                List<long> listpositions = new List<long>();
                listpositions.Add(position);
                this.dateOfBirthDictionary.Add(firstName, listpositions);
            }
        }

        private void ChangeRecordInLastNameDictionary(long position, string oldLastName, string lastName)
        {
            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                this.lastNameDictionary[oldLastName].Remove(position);
                this.lastNameDictionary[lastName].Add(position);
            }
            else
            {
                this.lastNameDictionary[oldLastName].Remove(position);
                List<long> listpositions = new List<long>();
                listpositions.Add(position);
                this.lastNameDictionary.Add(lastName, listpositions);
            }
        }

        private void ChangeRecordInDateOfBirthDictionary(long position, string oldDateOfBirth, DateTime dateOfBirth)
        {
            string tempDateOfBirh = dateOfBirth.ToString("yyyy-MMM-dd", new CultureInfo("en-US"));
            if (this.dateOfBirthDictionary.ContainsKey(tempDateOfBirh))
            {
                this.dateOfBirthDictionary[oldDateOfBirth].Remove(position);
                this.dateOfBirthDictionary[tempDateOfBirh].Add(position);
            }
            else
            {
                this.dateOfBirthDictionary[oldDateOfBirth].Remove(position);
                List<long> listpositions = new List<long>();
                listpositions.Add(position);
                this.dateOfBirthDictionary.Add(tempDateOfBirh, listpositions);
            }
        }

        private void RemoveRecordFromDictionary(FileCabinetRecord record, long position)
        {
            this.RemoveItemByFirstName(record, position);
            this.RemoveItemLastName(record, position);
            this.RemoveItemDateOfBirth(record, position);
        }

        private void RemoveItemByFirstName(FileCabinetRecord record, long position)
        {
            string firstName = record.FirstName;
            if (this.firstNameDictionary.ContainsKey(firstName.ToLower(new CultureInfo("en-US"))))
            {
                this.firstNameDictionary[firstName].Remove(position);
            }
        }

        private void RemoveItemLastName(FileCabinetRecord record, long position)
        {
            string lastName = record.LastName;
            if (this.firstNameDictionary.ContainsKey(lastName.ToLower(new CultureInfo("en-US"))))
            {
                this.firstNameDictionary[lastName].Remove(position);
            }
        }

        private void RemoveItemDateOfBirth(FileCabinetRecord record, long position)
        {
            string tempDateOfBirh = record.DateOfBirth.ToString("yyyy-MMM-dd", new CultureInfo("en-US"));
            if (this.firstNameDictionary.ContainsKey(tempDateOfBirh.ToLower(new CultureInfo("en-US"))))
            {
                this.firstNameDictionary[tempDateOfBirh].Remove(position);
            }
        }

        private void GetPositionOfRecordsList()
        {
            var recordBuffer = new byte[RECORDSIZE];
            int counteRecordInFile = this.GetCountAllRecordssFromFile();
            this.fileStream.Seek(0, SeekOrigin.Begin);
            for (int i = 1; i <= counteRecordInFile; i++)
            {
                if (recordBuffer[0] == SETTHIRDBITTRUE)
                {
                    continue;
                }
                else
                {
                    long seek = this.fileStream.Position;
                    this.fileStream.Read(recordBuffer, 0, RECORDSIZE);
                    var record = BytesToFileCabinetRecord(recordBuffer);
                    this.AddRecordToDictionary(record, seek);
                }
            }
        }
    }
}
