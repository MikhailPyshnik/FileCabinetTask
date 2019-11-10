using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace FileCabinetApp
{
    /// <summary>
    /// Work with records. Save and change record(s).
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        public FileCabinetMemoryService()
        {
        }

        /// <summary>
        /// Gets or sets the Validator of the Program.
        /// </summary>
        /// <value>The Validator of the Program.</value>
        public IValidatorOfParemetrs Validator { get; set; }

        /// <summary>
        /// Implementation IFileCabinetService СreateRecod.
        /// </summary>
        /// <param name="fileCabinetRecord">Input parametr record <see cref="FileCabinetRecord"/>.</param>
        /// <returns>Id <see cref="int"/>.</returns>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="fileCabinetRecord"/>, <paramref name="fileCabinetRecord.FirstName"/>,<paramref name="fileCabinetRecord.LastName"/> is null.</exception>
        /// <exception cref="ArgumentException">Throws if <paramref name="fileCabinetRecord.FirstName"/>,<paramref name="fileCabinetRecord.LastName"/>,<paramref name="fileCabinetRecord.DateOfBirth"/>,<paramref name="fileCabinetRecord.Sex"/>,<paramref name="fileCabinetRecord.Height"/>,<paramref name="fileCabinetRecord.Salary"/> is(are) incorrect value.</exception>
        public int CreateRecord(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord == null)
            {
                throw new ArgumentNullException($"{nameof(fileCabinetRecord)} is null!");
            }

            fileCabinetRecord.Id = this.list.Count + 1;
            this.list.Add(fileCabinetRecord);
            this.AddRecordToDictionary(fileCabinetRecord);
            return fileCabinetRecord.Id;
        }

        /// <summary>
        /// Implementation IFileCabinetService GetRecords.
        /// </summary>
        /// <returns>Rerords <see cref="FileCabinetRecord"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return new ReadOnlyCollection<FileCabinetRecord>(this.list);
        }

        /// <summary>
        /// Implementation IFileCabinetService GetStat.
        /// </summary>
        /// <returns>Count records <see cref="int"/>.</returns>
        public int GetStat()
        {
            return this.list.Count;
        }

        /// <summary>
        /// Implementation IFileCabinetService EditRecord.
        /// </summary>
        /// <param name="fileCabinetRecord">Input parametr record <see cref="FileCabinetRecord"/>.</param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="fileCabinetRecord"/>, <paramref name="fileCabinetRecord.FirstName"/>,<paramref name="fileCabinetRecord.LastName"/> is null.</exception>
        /// <exception cref="ArgumentException">Throws if <paramref name="fileCabinetRecord.FirstName"/>,<paramref name="fileCabinetRecord.LastName"/>,<paramref name="fileCabinetRecord.DateOfBirth"/>,<paramref name="fileCabinetRecord.Sex"/>,<paramref name="fileCabinetRecord.Height"/>,<paramref name="fileCabinetRecord.Salary"/> is(are) incorrect value.</exception>
        public void EditRecord(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord == null)
            {
                throw new ArgumentNullException($"{nameof(fileCabinetRecord)} is null!");
            }

            if (fileCabinetRecord.Id > this.GetStat())
            {
                throw new ArgumentException("Input Id is incorrect value.");
            }

            int editId = fileCabinetRecord.Id - 1;
            FileCabinetRecord res = this.list.Find(item1 => item1.Id == fileCabinetRecord.Id);
            string oldFirstName = res.FirstName;
            string oldLastName = res.LastName;
            string oldDateOfBirth = res.DateOfBirth.ToString("yyyy-MMM-dd", new CultureInfo("en-US"));
            FileCabinetRecord item = this.list[editId];
            item.FirstName = fileCabinetRecord.FirstName;
            item.LastName = fileCabinetRecord.LastName;
            item.DateOfBirth = fileCabinetRecord.DateOfBirth;
            item.Sex = fileCabinetRecord.Sex;
            item.Height = fileCabinetRecord.Height;
            item.Salary = fileCabinetRecord.Salary;
            this.ChangeRecordToDictionary(item, oldFirstName, oldLastName, oldDateOfBirth);
        }

        /// <summary>
        /// Implementation IFileCabinetService FindByFirstName.
        /// </summary>
        /// <param name="firstName">Input parametr FirstName <see cref="string"/>.</param>
        /// <returns>Rerords by firstName <see cref="FileCabinetRecord"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            List<FileCabinetRecord> result = new List<FileCabinetRecord>();
            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                result = this.firstNameDictionary[firstName];
            }

            return new ReadOnlyCollection<FileCabinetRecord>(result);
        }

        /// <summary>
        /// Implementation IFileCabinetService FindByLastName.
        /// </summary>
        /// <param name="lastName">Input parametr FirstName <see cref="string"/>.</param>
        /// <returns>Rerords by lastName <see cref="FileCabinetRecord"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            List<FileCabinetRecord> result = new List<FileCabinetRecord>();

            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                result = this.lastNameDictionary[lastName];
            }

            return new ReadOnlyCollection<FileCabinetRecord>(result);
        }

        /// <summary>
        /// Implementation IFileCabinetService FindByDateOfBirth.
        /// </summary>
        /// <param name="dateofbirth">Input parametr FirstName <see cref="string"/>.</param>
        /// <returns>Rerords by dateofbirth <see cref="FileCabinetRecord"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateofbirth)
        {
            List<FileCabinetRecord> result = new List<FileCabinetRecord>();

            if (this.dateOfBirthDictionary.ContainsKey(dateofbirth))
            {
                result = this.dateOfBirthDictionary[dateofbirth];
            }

            return new ReadOnlyCollection<FileCabinetRecord>(result);
        }

        /// <summary>
        /// Implementation IFileCabinetService MakeSnapshot.
        /// </summary>
        /// <returns>Rerords by dateofbirth <see cref="FileCabinetServiceSnapshot"/>.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            FileCabinetServiceSnapshot fileCabinetServiceSnapshot = new FileCabinetServiceSnapshot(this.list.ToArray());

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
        /// Implementation IFileCabinetService Restore.
        /// </summary>
        /// <param name="id">Input parametr id of record <see cref="int"/>.</param>
        public void Remove(int id)
        {
            foreach (var removeId in this.list)
            {
                if (removeId.Id == id)
                {
                    this.list.Remove(removeId);
                    this.RemoveRecordFromDictionary(removeId);
                    break;
                }
            }
        }

        /// <summary>
        /// Implementation IFileCabinetService Purge.
        /// </summary>
        public void Purge()
        {
            Console.WriteLine("In FileCabinetMemoryService purge is not exist.");
        }

        private void AddRecordToDictionary(FileCabinetRecord record)
        {
            this.AddRecordToFirstNameDictionary(record, record.FirstName);
            this.AddRecordToLastNameDictionary(record, record.LastName);
            this.AddRecordToDateOfBirthDictionary(record, record.DateOfBirth);
        }

        private void ChangeRecordToDictionary(FileCabinetRecord item, string oldFirstName, string oldLastName, string oldDateOfBirth)
        {
            this.ChangeRecordInFirstNameDictionary(item, oldFirstName, item.FirstName);
            this.ChangeRecordInLastNameDictionary(item, oldLastName, item.LastName);
            this.ChangeRecordInDateOfBirthDictionary(item, oldDateOfBirth, item.DateOfBirth);
        }

        private void AddRecordToFirstNameDictionary(FileCabinetRecord record, string firstName)
        {
            if (this.firstNameDictionary.ContainsKey(firstName.ToLower(new CultureInfo("en-US"))))
            {
                this.firstNameDictionary[firstName].Add(record);
            }
            else
            {
                List<FileCabinetRecord> listFirstName = this.list.FindAll(item => item.FirstName == firstName);
                this.firstNameDictionary.Add(firstName, listFirstName);
            }
        }

        private void AddRecordToLastNameDictionary(FileCabinetRecord record, string lastName)
        {
            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                this.lastNameDictionary[lastName].Add(record);
            }
            else
            {
                List<FileCabinetRecord> listLastName = this.list.FindAll(item => item.LastName == lastName);
                this.lastNameDictionary.Add(lastName, listLastName);
            }
        }

        private void AddRecordToDateOfBirthDictionary(FileCabinetRecord record, DateTime dateOfBirth)
        {
            string tempDateOfBirh = dateOfBirth.ToString("yyyy-MMM-dd", new CultureInfo("en-US"));
            if (this.dateOfBirthDictionary.ContainsKey(tempDateOfBirh))
            {
                this.dateOfBirthDictionary[tempDateOfBirh].Add(record);
            }
            else
            {
                List<FileCabinetRecord> listdateOfBirth = this.list.FindAll(item => item.DateOfBirth == dateOfBirth);
                this.dateOfBirthDictionary.Add(tempDateOfBirh, listdateOfBirth);
            }
        }

        private void ChangeRecordInFirstNameDictionary(FileCabinetRecord item, string oldFirstName, string firstName)
        {
            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                this.firstNameDictionary[oldFirstName].Remove(item);
                this.firstNameDictionary[firstName].Add(item);
            }
            else
            {
                this.firstNameDictionary[oldFirstName].Remove(item);
                List<FileCabinetRecord> listFirstName = this.list.FindAll(item1 => item1.FirstName == firstName);
                this.firstNameDictionary.Add(firstName, listFirstName);
            }
        }

        private void ChangeRecordInLastNameDictionary(FileCabinetRecord item, string oldLastName, string lastName)
        {
            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                this.lastNameDictionary[oldLastName].Remove(item);
                this.lastNameDictionary[lastName].Add(item);
            }
            else
            {
                this.lastNameDictionary[oldLastName].Remove(item);
                List<FileCabinetRecord> listFirstName = this.list.FindAll(item1 => item1.FirstName == lastName);
                this.lastNameDictionary.Add(lastName, listFirstName);
            }
        }

        private void ChangeRecordInDateOfBirthDictionary(FileCabinetRecord item, string oldDateOfBirth, DateTime dateOfBirth)
        {
            string tempDateOfBirh = dateOfBirth.ToString("yyyy-MMM-dd", new CultureInfo("en-US"));
            if (this.dateOfBirthDictionary.ContainsKey(tempDateOfBirh))
            {
                this.dateOfBirthDictionary[oldDateOfBirth].Remove(item);
                this.dateOfBirthDictionary[tempDateOfBirh].Add(item);
            }
            else
            {
                this.dateOfBirthDictionary[oldDateOfBirth].Remove(item);
                List<FileCabinetRecord> listDateOfBirth = this.list.FindAll(item1 => item.DateOfBirth == dateOfBirth);
                this.dateOfBirthDictionary.Add(tempDateOfBirh, listDateOfBirth);
            }
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
            for (int i = 0; i < this.list.Count; i++)
            {
                if (!validateList.Exists(item => item.Id == this.list[i].Id))
                {
                    validateList.Add(this.list[i]);
                }
            }

            validateList = validateList.OrderBy(item => item.Id).ToList();
            this.list.Clear();
            this.list.AddRange(validateList);
        }

        private void RemoveRecordFromDictionary(FileCabinetRecord record)
        {
            this.RemoveItemByFirstName(record);
            this.RemoveItemLastName(record);
            this.RemoveItemDateOfBirth(record);
        }

        private void RemoveItemByFirstName(FileCabinetRecord record)
        {
            string firstName = record.FirstName;
            if (this.firstNameDictionary.ContainsKey(firstName.ToLower(new CultureInfo("en-US"))))
            {
                this.firstNameDictionary[firstName].Remove(record);
            }
        }

        private void RemoveItemLastName(FileCabinetRecord record)
        {
            string lastName = record.LastName;
            if (this.firstNameDictionary.ContainsKey(lastName.ToLower(new CultureInfo("en-US"))))
            {
                this.firstNameDictionary[lastName].Remove(record);
            }
        }

        private void RemoveItemDateOfBirth(FileCabinetRecord record)
        {
            string tempDateOfBirh = record.DateOfBirth.ToString("yyyy-MMM-dd", new CultureInfo("en-US"));
            if (this.firstNameDictionary.ContainsKey(tempDateOfBirh.ToLower(new CultureInfo("en-US"))))
            {
                this.firstNameDictionary[tempDateOfBirh].Remove(record);
            }
        }
    }
}