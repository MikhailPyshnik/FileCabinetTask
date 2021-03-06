﻿using System;
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
        private static readonly CultureInfo Provider = new CultureInfo("en-US");
        private readonly IRecordValidator validator;

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
        /// Gets the type storage of filecabinet.
        /// </summary>
        /// <value>The Status of the record.</value>
        public IFileCabinetService FileCabinetProperties { get => new FileCabinetFilesystemService(this.fileStream, this.validator); }

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
            if (fileCabinetRecord is null)
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

            return fileCabinetRecord.Id;
        }

        /// <summary>
        /// Implementation IFileCabinetService СreateRecod.
        /// </summary>
        /// <param name="fileCabinetRecord">Input parametr record <see cref="FileCabinetRecord"/>.</param>
        /// <returns>Id<see cref="int"/>.</returns>
        public int Insert(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord is null)
            {
                throw new ArgumentNullException($"{nameof(fileCabinetRecord)} is null!");
            }

            this.validator.ValidateParametrs(fileCabinetRecord);
            int editIdReord = fileCabinetRecord.Id;
            var recordBuffer = new byte[RECORDSIZE];
            int counteRecordInFile = this.GetCountAllRecordssFromFile();
            if (counteRecordInFile == 0)
            {
                this.CreateRecord(fileCabinetRecord);
            }
            else
            {
                List<FileCabinetRecord> listTempForInsert = new List<FileCabinetRecord>(this.GetNotDeletedRecordsList());
                this.fileStream.Seek(0, SeekOrigin.Begin);
                for (int i = 0; i < counteRecordInFile; i++)
                {
                    this.fileStream.Read(recordBuffer, 0, RECORDSIZE);
                    if (recordBuffer[0] != SETTHIRDBITTRUE)
                    {
                        var bytesToRecord = BytesToFileCabinetRecord(recordBuffer);
                        var tempRecord = listTempForInsert.Find(item1 => item1.Id == bytesToRecord.Id);

                        if (listTempForInsert.Exists(x => x.Id == fileCabinetRecord.Id))
                        {
                            return 0;
                        }
                        else
                        {
                            this.fileStream.Seek(0, SeekOrigin.End);
                            long seek1 = this.fileStream.Position;
                            var b2 = FileCabinetRecordToBytes(fileCabinetRecord);
                            this.fileStream.Write(b2, 0, b2.Length);
                            this.fileStream.Flush();
                            break;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            return editIdReord;
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
        /// Implementation IFileCabinetService UpdateRecord.
        /// </summary>
        /// <param name="inputValueArray">Input value array <see cref="string"/>.</param>
        /// <param name="inputParamentArray">Input parametr array <see cref="string"/>.</param>
        public void Update(string[] inputValueArray, string[] inputParamentArray)
        {
            if (inputValueArray is null)
            {
                throw new ArgumentNullException($"{nameof(inputValueArray)} is null!");
            }

            if (inputParamentArray is null)
            {
                throw new ArgumentNullException($"{nameof(inputParamentArray)} is null!");
            }

            List<FileCabinetRecord> result = this.FindRecordsByParameters(inputParamentArray);
            this.UpdateRecords(result, inputValueArray);
        }

        /// <summary>
        /// Return records by select in FileSystemService.
        /// </summary>
        /// <param name="inputParamentArray">Input parametr array <see cref="string"/>.</param>
        /// <param name="logicalOperator">Input parametr for conditional <see cref="string"/>.</param>
        /// <returns>IEnumerable by firstName <see cref="FileCabinetRecord"/>.</returns>
        public IEnumerable<FileCabinetRecord> SelectByCondition(string[] inputParamentArray, string logicalOperator)
        {
            if (inputParamentArray is null)
            {
                throw new ArgumentNullException($"{nameof(inputParamentArray)} is null!");
            }

            if (logicalOperator is null)
            {
                throw new ArgumentNullException($"{nameof(logicalOperator)} is null!");
            }

            List<FileCabinetRecord> result = this.FindRecordsByParameters(inputParamentArray, logicalOperator);
            return result;
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
        /// <returns>Counts add import record(s) <see cref="int"/>.</returns>
        public int Restore(FileCabinetServiceSnapshot fileCabinetServiceSnapshot)
        {
            if (fileCabinetServiceSnapshot is null)
            {
                throw new ArgumentNullException($"{nameof(fileCabinetServiceSnapshot)} is null");
            }

            var importRecords = fileCabinetServiceSnapshot.Records;

            List<FileCabinetRecord> listImport = new List<FileCabinetRecord>(importRecords);

            List<FileCabinetRecord> validatedList = this.GetValidFileCabinetRecords(listImport);

            int countImportRecord = validatedList.Count;

            this.UpdateListAfterImport(validatedList);

            return countImportRecord;
        }

        /// <summary>
        /// Implementation IFileCabinetService Restore.
        /// </summary>
        /// <param name="inputValueArray">Input parametr value <see cref="string"/>.</param>
        /// <returns>ReadOnlyCollection deleted id <see cref="int"/>.</returns>
        public ReadOnlyCollection<int> Delete(string[] inputValueArray)
        {
            if (inputValueArray is null)
            {
                throw new ArgumentNullException(nameof(inputValueArray));
            }

            List<int> listTemp;

            switch (inputValueArray[0])
            {
                case "id":
                    int id = int.Parse(inputValueArray[1], Provider);
                    listTemp = this.RemoveById(id);
                    break;
                case "firstname":
                    string firstName = inputValueArray[1];
                    listTemp = this.RemoveByFirstName(firstName);
                    break;
                case "lastname":
                    string lastName = inputValueArray[1];
                    listTemp = this.RemoveByLastName(lastName);
                    break;
                case "dateofbirth":
                    DateTime dateOfBirth = DateTime.Parse(inputValueArray[1], Provider);
                    listTemp = this.RemoveByDateOfBirth(dateOfBirth);
                    break;
                case "sex":
                    char sex = char.Parse(inputValueArray[1]);
                    listTemp = this.RemoveByGender(sex);
                    break;
                case "height":
                    short height = short.Parse(inputValueArray[1], Provider);
                    listTemp = this.RemoveByHeight(height);
                    break;
                case "salary":
                    decimal salary = decimal.Parse(inputValueArray[1], Provider);
                    listTemp = this.RemoveBySalary(salary);
                    break;
                default:
                    throw new ArgumentException("Not correct value in remove(delete) !!!!");
            }

            return new ReadOnlyCollection<int>(listTemp);
        }

        /// <summary>
        /// Implementation IFileCabinetService Purge.
        /// </summary>
        /// <returns>Count records for delete <see cref="Tuple"/>.</returns>
        public Tuple<int, int> Purge()
        {
            int countDeletedRecords = this.GetDdeletedRecordList().Count;
            int countRecordsInFile = this.GetCountAllRecordssFromFile();

            this.WriteRecordsToFile(this.GetNotDeletedRecordsList());

            return new Tuple<int, int>(countDeletedRecords, countRecordsInFile);
        }

        private static byte[] FileCabinetRecordToBytes(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord is null)
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
            if (bytes is null)
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
            bool isFirstNameValid = this.ValidateInput(fileCabinetRecord.FirstName, this.Validator.FirstNameConverter, this.Validator.FirstNameValidator);
            bool isLastNameValid = this.ValidateInput(fileCabinetRecord.LastName, this.Validator.LastNameConverter, this.Validator.LastNameValidator);
            bool isDateOfBirthValid = this.ValidateInput(fileCabinetRecord.DateOfBirth.ToString("dd/MM/yyyy", Provider), this.Validator.DayOfBirthConverter, this.Validator.DayOfBirthValidator);
            bool isSexValid = this.ValidateInput(fileCabinetRecord.Sex.ToString(Provider), this.Validator.SexConverter, this.Validator.SexValidator);
            bool isHeigthValid = this.ValidateInput(fileCabinetRecord.Height.ToString(Provider), this.Validator.HeightConverter, this.Validator.HeightValidator);
            bool isSalaryValid = this.ValidateInput(fileCabinetRecord.Salary.ToString(Provider), this.Validator.SalaryConverter, this.Validator.SalaryValidator);

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
                    Console.WriteLine($"#{item.Id} records has incorrect value.This record was skipped.");
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

            this.GetPositionOfRecordsList();
        }

        private List<FileCabinetRecord> GetAllRecordsFromFile()
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

            return new List<FileCabinetRecord>(listRecord);
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

        private void GetPositionOfRecordsList()
        {
            var recordBuffer = new byte[RECORDSIZE];
            int counteRecordInFile = this.GetCountAllRecordssFromFile();
            this.fileStream.Seek(0, SeekOrigin.Begin);
            for (int i = 1; i <= counteRecordInFile; i++)
            {
                long seek = this.fileStream.Position;
                this.fileStream.Read(recordBuffer, 0, RECORDSIZE);
                if (recordBuffer[0] != SETTHIRDBITTRUE)
                {
                    var record = BytesToFileCabinetRecord(recordBuffer);
                }
            }
        }

        private List<int> RemoveById(int id)
        {
            List<int> count = new List<int>();
            var a = this.GetAllRecordsFromFile();
            List<FileCabinetRecord> recordToList = new List<FileCabinetRecord>(a);
            int curent = 0;
            var recordBuffer = new byte[RECORDSIZE];
            int counteRecordInFile = this.GetCountAllRecordssFromFile();
            this.fileStream.Seek(0, SeekOrigin.Begin);
            foreach (var remove in recordToList)
            {
                this.fileStream.Read(recordBuffer, 0, RECORDSIZE);
                if (remove.Id == id)
                {
                    this.fileStream.Seek(curent * 278, SeekOrigin.Begin);
                    this.fileStream.Seek(0, SeekOrigin.Current);
                    long seek = this.fileStream.Position;
                    var removeRecord = recordBuffer;
                    removeRecord[0] = 4;
                    this.fileStream.Write(removeRecord, 0, removeRecord.Length);
                    this.fileStream.Flush();
                    count.Add(remove.Id);
                }

                ++curent;
            }

            return count;
        }

        private List<int> RemoveByFirstName(string firstName)
        {
            List<int> count = new List<int>();
            var a = this.GetAllRecordsFromFile();
            List<FileCabinetRecord> recordToList = new List<FileCabinetRecord>(a);
            int curent = 0;
            var recordBuffer = new byte[RECORDSIZE];
            int counteRecordInFile = this.GetCountAllRecordssFromFile();
            this.fileStream.Seek(0, SeekOrigin.Begin);
            foreach (var remove in recordToList)
            {
                this.fileStream.Read(recordBuffer, 0, RECORDSIZE);
                if (remove.FirstName.ToLower(Provider) == firstName)
                {
                    this.fileStream.Seek(curent * 278, SeekOrigin.Begin);
                    this.fileStream.Seek(0, SeekOrigin.Current);
                    long seek = this.fileStream.Position;
                    var removeRecord = recordBuffer;
                    removeRecord[0] = 4;
                    this.fileStream.Write(removeRecord, 0, removeRecord.Length);
                    this.fileStream.Flush();
                    count.Add(remove.Id);
                }

                ++curent;
            }

            return count;
        }

        private List<int> RemoveByLastName(string lastName)
        {
            List<int> count = new List<int>();
            var a = this.GetAllRecordsFromFile();
            List<FileCabinetRecord> recordToList = new List<FileCabinetRecord>(a);
            int curent = 0;
            var recordBuffer = new byte[RECORDSIZE];
            int counteRecordInFile = this.GetCountAllRecordssFromFile();
            this.fileStream.Seek(0, SeekOrigin.Begin);
            foreach (var remove in recordToList)
            {
                this.fileStream.Read(recordBuffer, 0, RECORDSIZE);
                if (remove.LastName.ToLower(Provider) == lastName)
                {
                    this.fileStream.Seek(curent * 278, SeekOrigin.Begin);
                    this.fileStream.Seek(0, SeekOrigin.Current);
                    long seek = this.fileStream.Position;
                    var removeRecord = recordBuffer;
                    removeRecord[0] = 4;
                    this.fileStream.Write(removeRecord, 0, removeRecord.Length);
                    this.fileStream.Flush();
                    count.Add(remove.Id);
                }

                ++curent;
            }

            return count;
        }

        private List<int> RemoveByDateOfBirth(DateTime dateOfBirth)
        {
            List<int> count = new List<int>();
            var a = this.GetAllRecordsFromFile();
            List<FileCabinetRecord> recordToList = new List<FileCabinetRecord>(a);
            int curent = 0;
            var recordBuffer = new byte[RECORDSIZE];
            int counteRecordInFile = this.GetCountAllRecordssFromFile();
            this.fileStream.Seek(0, SeekOrigin.Begin);
            foreach (var remove in recordToList)
            {
                this.fileStream.Read(recordBuffer, 0, RECORDSIZE);
                if (remove.DateOfBirth == dateOfBirth)
                {
                    this.fileStream.Seek(curent * 278, SeekOrigin.Begin);
                    this.fileStream.Seek(0, SeekOrigin.Current);
                    long seek = this.fileStream.Position;
                    var removeRecord = recordBuffer;
                    removeRecord[0] = 4;
                    this.fileStream.Write(removeRecord, 0, removeRecord.Length);
                    this.fileStream.Flush();
                    count.Add(remove.Id);
                }

                ++curent;
            }

            return count;
        }

        private List<int> RemoveByGender(char sex)
        {
            List<int> count = new List<int>();
            var a = this.GetAllRecordsFromFile();
            List<FileCabinetRecord> recordToList = new List<FileCabinetRecord>(a);
            int curent = 0;
            var recordBuffer = new byte[RECORDSIZE];
            int counteRecordInFile = this.GetCountAllRecordssFromFile();
            this.fileStream.Seek(0, SeekOrigin.Begin);
            foreach (var remove in recordToList)
            {
                this.fileStream.Read(recordBuffer, 0, RECORDSIZE);
                if (remove.Sex == sex)
                {
                    this.fileStream.Seek(curent * 278, SeekOrigin.Begin);
                    this.fileStream.Seek(0, SeekOrigin.Current);
                    long seek = this.fileStream.Position;
                    var removeRecord = recordBuffer;
                    removeRecord[0] = 4;
                    this.fileStream.Write(removeRecord, 0, removeRecord.Length);
                    this.fileStream.Flush();
                    count.Add(remove.Id);
                }

                ++curent;
            }

            return count;
        }

        private List<int> RemoveByHeight(short height)
        {
            List<int> count = new List<int>();
            var a = this.GetAllRecordsFromFile();
            List<FileCabinetRecord> recordToList = new List<FileCabinetRecord>(a);
            int curent = 0;
            var recordBuffer = new byte[RECORDSIZE];
            int counteRecordInFile = this.GetCountAllRecordssFromFile();
            this.fileStream.Seek(0, SeekOrigin.Begin);
            foreach (var remove in recordToList)
            {
                this.fileStream.Read(recordBuffer, 0, RECORDSIZE);
                if (remove.Height == height)
                {
                    this.fileStream.Seek(curent * 278, SeekOrigin.Begin);
                    this.fileStream.Seek(0, SeekOrigin.Current);
                    long seek = this.fileStream.Position;
                    var removeRecord = recordBuffer;
                    removeRecord[0] = 4;
                    this.fileStream.Write(removeRecord, 0, removeRecord.Length);
                    this.fileStream.Flush();
                    count.Add(remove.Id);
                }

                ++curent;
            }

            return count;
        }

        private List<int> RemoveBySalary(decimal salary)
        {
            List<int> count = new List<int>();
            var a = this.GetAllRecordsFromFile();
            List<FileCabinetRecord> recordToList = new List<FileCabinetRecord>(a);
            int curent = 0;
            var recordBuffer = new byte[RECORDSIZE];
            int counteRecordInFile = this.GetCountAllRecordssFromFile();
            this.fileStream.Seek(0, SeekOrigin.Begin);
            foreach (var remove in recordToList)
            {
                this.fileStream.Read(recordBuffer, 0, RECORDSIZE);
                if (remove.Salary == salary)
                {
                    this.fileStream.Seek(curent * 278, SeekOrigin.Begin);
                    this.fileStream.Seek(0, SeekOrigin.Current);
                    long seek = this.fileStream.Position;
                    var removeRecord = recordBuffer;
                    removeRecord[0] = 4;
                    this.fileStream.Write(removeRecord, 0, removeRecord.Length);
                    this.fileStream.Flush();
                    count.Add(remove.Id);
                }

                ++curent;
            }

            return count;
        }

        private List<FileCabinetRecord> FindRecordsByParameters(string[] parameter)
        {
            List<FileCabinetRecord> listTemp = new List<FileCabinetRecord>(this.GetNotDeletedRecordsList());

            foreach (var item in parameter)
            {
                string[] split = item.Split("=");
                listTemp = this.FindRecordsByParameterInList(listTemp, split[0], split[1]);
            }

            return listTemp;
        }

        private List<FileCabinetRecord> FindRecordsByParameterInList(List<FileCabinetRecord> list, string parameter, string value)
        {
            List<FileCabinetRecord> listTemp = new List<FileCabinetRecord>();

            switch (parameter)
            {
                case "id":
                    int id = int.Parse(value, Provider);
                    listTemp = list.FindAll(item1 => item1.Id == id);
                    break;
                case "firstname":
                    string firstName = value;
                    listTemp = list.FindAll(item1 => item1.FirstName.Equals(firstName, StringComparison.InvariantCultureIgnoreCase));
                    break;
                case "lastname":
                    string lastName = value;
                    listTemp = list.FindAll(item1 => item1.LastName.Equals(lastName, StringComparison.InvariantCultureIgnoreCase));
                    break;
                case "dateofbirth":
                    DateTime dateOfBirth = DateTime.Parse(value, Provider);
                    listTemp = list.FindAll(item1 => item1.DateOfBirth == dateOfBirth);
                    break;
                case "sex":
                    char sex = char.Parse(value);
                    listTemp = list.FindAll(item1 => item1.Sex == sex);
                    break;
                case "height":
                    short height = short.Parse(value, Provider);
                    listTemp = list.FindAll(item1 => item1.Height == height);
                    break;
                case "salary":
                    decimal salary = decimal.Parse(value, Provider);
                    listTemp = list.FindAll(item1 => item1.Salary == salary);
                    break;
                default:
                    throw new ArgumentException("Not correct value!!!!");
            }

            if (listTemp.Count == 0)
            {
                throw new ArgumentException("Don't find records for update by conditional!");
            }

            return listTemp;
        }

        private List<FileCabinetRecord> FindRecordsByParameters(string[] parameter, string logicParametr)
        {
            List<FileCabinetRecord> listTemp = new List<FileCabinetRecord>(this.GetNotDeletedRecordsList());

            if (logicParametr == "and")
            {
                foreach (var item in parameter)
                {
                    string[] split = item.Split("=");
                    listTemp = this.FindRecordsByParameterInList(listTemp, split[0], split[1]);
                }
            }

            if (logicParametr == "or")
            {
                HashSet<FileCabinetRecord> set = new HashSet<FileCabinetRecord>();
                foreach (var item in parameter)
                {
                    string[] split = item.Split("=");
                    set = this.FindRecordsByParameterInListOr(set, split[0], split[1]);
                }

                listTemp = new List<FileCabinetRecord>(set);
            }

            return listTemp;
        }

        private HashSet<FileCabinetRecord> FindRecordsByParameterInListOr(HashSet<FileCabinetRecord> set, string parameter, string value)
        {
            List<FileCabinetRecord> listTemp = new List<FileCabinetRecord>(this.GetNotDeletedRecordsList());

            switch (parameter)
            {
                case "id":
                    int id = int.Parse(value, Provider);
                    set.UnionWith(listTemp.FindAll(item1 => item1.Id == id));
                    break;
                case "firstname":
                    string firstName = value;
                    set.UnionWith(listTemp.FindAll(item1 => item1.FirstName.Equals(firstName, StringComparison.InvariantCultureIgnoreCase)));
                    break;
                case "lastname":
                    string lastName = value;
                    set.UnionWith(listTemp.FindAll(item1 => item1.LastName.Equals(lastName, StringComparison.InvariantCultureIgnoreCase)));
                    break;
                case "dateofbirth":
                    DateTime dateOfBirth = DateTime.Parse(value, Provider);
                    set.UnionWith(listTemp.FindAll(item1 => item1.DateOfBirth == dateOfBirth));
                    break;
                case "sex":
                    char sex = char.Parse(value);
                    set.UnionWith(listTemp.FindAll(item1 => char.ToLowerInvariant(item1.Sex) == sex));
                    break;
                case "height":
                    short height = short.Parse(value, Provider);
                    set.UnionWith(listTemp.FindAll(item1 => item1.Height == height));
                    break;
                case "salary":
                    decimal salary = decimal.Parse(value, Provider);
                    set.UnionWith(listTemp.FindAll(item1 => item1.Salary == salary));
                    break;
                default:
                    throw new ArgumentException("Not correct value!!!!");
            }

            return set;
        }

        private void UpdateRecords(List<FileCabinetRecord> listRecords, string[] inputValueArray)
        {
            foreach (var record in listRecords)
            {
                FileCabinetRecord item = new FileCabinetRecord();
                item.Id = record.Id;
                item.FirstName = record.FirstName;
                item.LastName = record.LastName;
                item.DateOfBirth = record.DateOfBirth;
                item.Sex = record.Sex;
                item.Height = record.Height;
                item.Salary = record.Salary;

                foreach (var parametr in inputValueArray)
                {
                    string[] split = parametr.Split("=");
                    string parameter = split[0];
                    string value = split[1];
                    switch (parameter)
                    {
                        case "id":
                            int id = int.Parse(value, Provider);
                            item.Id = id;
                            break;
                        case "firstname":
                            string firstName = value;
                            item.FirstName = firstName;
                            break;
                        case "lastname":
                            string lastName = value;
                            item.LastName = lastName;
                            break;
                        case "dateofbirth":
                            DateTime dateOfBirth = DateTime.Parse(value, Provider);
                            item.DateOfBirth = dateOfBirth;
                            break;
                        case "sex":
                            char sex = char.Parse(value);
                            item.Sex = sex;
                            break;
                        case "height":
                            short height = short.Parse(value, Provider);
                            item.Height = height;
                            break;
                        case "salary":
                            decimal salary = decimal.Parse(value, Provider);
                            item.Salary = salary;
                            break;
                        default:
                            throw new ArgumentException("Not correct value!!!!");
                    }
                }

                this.EditRecord(item);
            }
        }

        private void EditRecord(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord is null)
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
                        string oldDateOfBirth = bytesToRecord.DateOfBirth.ToString("yyyy-MMM-dd", Provider);

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
                        return;
                    }
                }
            }
        }
    }
}
