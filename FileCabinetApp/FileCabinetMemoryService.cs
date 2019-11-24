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
        private readonly IRecordValidator validator;

        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="recordValidator">Input parametr in constructor <see cref="IRecordValidator"/>.</param>
        public FileCabinetMemoryService(IRecordValidator recordValidator)
        {
            this.validator = recordValidator ?? throw new ArgumentNullException(nameof(recordValidator));
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

            this.validator.ValidateParametrs(fileCabinetRecord);
            fileCabinetRecord.Id = this.list[this.list.Count - 1].Id + 1;
            this.list.Add(fileCabinetRecord);
            this.AddRecordToDictionary(fileCabinetRecord);
            return fileCabinetRecord.Id;
        }

        /// <summary>
        /// Implementation IFileCabinetService Insert.
        /// </summary>
        /// <param name="fileCabinetRecord">Input parametr record <see cref="FileCabinetRecord"/>.</param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="fileCabinetRecord"/>, <paramref name="fileCabinetRecord.FirstName"/>,<paramref name="fileCabinetRecord.LastName"/> is null.</exception>
        /// <exception cref="ArgumentException">Throws if <paramref name="fileCabinetRecord.FirstName"/>,<paramref name="fileCabinetRecord.LastName"/>,<paramref name="fileCabinetRecord.DateOfBirth"/>,<paramref name="fileCabinetRecord.Sex"/>,<paramref name="fileCabinetRecord.Height"/>,<paramref name="fileCabinetRecord.Salary"/> is(are) incorrect value.</exception>
        public void Insert(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord == null)
            {
                throw new ArgumentNullException($"{nameof(fileCabinetRecord)} is null!");
            }

            if (this.list.Exists(x => x.Id == fileCabinetRecord.Id))
            {
                this.EditRecord(fileCabinetRecord);
                return;
            }

            this.validator.ValidateParametrs(fileCabinetRecord);
            this.list.Add(fileCabinetRecord);
            this.AddRecordToDictionary(fileCabinetRecord);
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
        public Tuple<int, int> GetStat()
        {
            int countRecordInFile = this.list.Count;
            int countDeleteRecord = 0;
            return new Tuple<int, int>(countRecordInFile, countDeleteRecord);
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

            this.validator.ValidateParametrs(fileCabinetRecord);

            FileCabinetRecord res = this.list.Find(item1 => item1.Id == fileCabinetRecord.Id);
            string oldFirstName = res.FirstName;
            string oldLastName = res.LastName;
            string oldDateOfBirth = res.DateOfBirth.ToString("yyyy-MMM-dd", new CultureInfo("en-US"));
            res.FirstName = fileCabinetRecord.FirstName;
            res.LastName = fileCabinetRecord.LastName;
            res.DateOfBirth = fileCabinetRecord.DateOfBirth;
            res.Sex = fileCabinetRecord.Sex;
            res.Height = fileCabinetRecord.Height;
            res.Salary = fileCabinetRecord.Salary;
            this.ChangeRecordToDictionary(res, oldFirstName, oldLastName, oldDateOfBirth);
        }

        /// <summary>
        /// Implementation IFileCabinetService UpdateRecord.
        /// </summary>
        /// <param name="inputValueArray">Input value array <see cref="string"/>.</param>
        /// <param name="inputParamentArray">Input parametr array <see cref="string"/>.</param>
        public void Update(string[] inputValueArray, string[] inputParamentArray)
        {
            if (inputValueArray == null)
            {
                throw new ArgumentNullException($"{nameof(inputValueArray)} is null!");
            }

            if (inputParamentArray == null)
            {
                throw new ArgumentNullException($"{nameof(inputParamentArray)} is null!");
            }

            List<FileCabinetRecord> result = this.FindRecordsByParameters(inputParamentArray);
            this.UpdateRecords(result, inputValueArray);
        }

        /// <summary>
        /// Implementation IFileCabinetService FindByFirstName.
        /// </summary>
        /// <param name="firstName">Input parametr FirstName <see cref="string"/>.</param>
        /// <returns>IEnumerable by firstName <see cref="FileCabinetRecord"/>.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (firstName == null)
            {
                throw new ArgumentNullException(nameof(firstName));
            }

            List<FileCabinetRecord> resultList = new List<FileCabinetRecord>();
            if (this.firstNameDictionary.ContainsKey(firstName.ToLower(new CultureInfo("en-US"))))
            {
                resultList = this.firstNameDictionary[firstName];
            }

            for (int i = 0; i < resultList.Count; i++)
            {
                yield return resultList[i];
            }
        }

        /// <summary>
        /// Implementation IFileCabinetService FindByLastName.
        /// </summary>
        /// <param name="lastName">Input parametr FirstName <see cref="string"/>.</param>
        /// <returns>IEnumerable by lastName <see cref="FileCabinetRecord"/>.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (lastName == null)
            {
                throw new ArgumentNullException(nameof(lastName));
            }

            List<FileCabinetRecord> resultList = new List<FileCabinetRecord>();
            if (this.lastNameDictionary.ContainsKey(lastName.ToLower(new CultureInfo("en-US"))))
            {
                resultList = this.lastNameDictionary[lastName];
            }

            for (int i = 0; i < resultList.Count; i++)
            {
                yield return resultList[i];
            }
        }

        /// <summary>
        /// Implementation IFileCabinetService FindByDateOfBirth.
        /// </summary>
        /// <param name="dateofbirth">Input parametr FirstName <see cref="string"/>.</param>
        /// <returns>IEnumerable by dateofbirth <see cref="FileCabinetRecord"/>.</returns>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateofbirth)
        {
            if (dateofbirth == null)
            {
                throw new ArgumentNullException(nameof(dateofbirth));
            }

            List<FileCabinetRecord> resultList = new List<FileCabinetRecord>();
            if (this.dateOfBirthDictionary.ContainsKey(dateofbirth.ToLower(new CultureInfo("en-US"))))
            {
                resultList = this.dateOfBirthDictionary[dateofbirth];
            }

            for (int i = 0; i < resultList.Count; i++)
            {
                yield return resultList[i];
            }
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
        /// Implementation IFileCabinetService Restore.
        /// </summary>
        /// <param name="inputValueArray">Input parametr value <see cref="string"/>.</param>
        /// <returns>ReadOnlyCollection deleted id <see cref="int"/>.</returns>
        public ReadOnlyCollection<int> Delete(string[] inputValueArray)
        {
            if (inputValueArray == null)
            {
                throw new ArgumentNullException(nameof(inputValueArray));
            }

            CultureInfo provider = new CultureInfo("en-US");

            List<int> listTemp;

            switch (inputValueArray[0])
            {
                case "id":
                    int id = int.Parse(inputValueArray[1], provider);
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
                    DateTime dateOfBirth = DateTime.Parse(inputValueArray[1], provider);
                    listTemp = this.RemoveByDateOfBirth(dateOfBirth);
                    break;
                case "sex":
                    char sex = char.Parse(inputValueArray[1]);
                    listTemp = this.RemoveByGender(sex);
                    break;
                case "height":
                    short height = short.Parse(inputValueArray[1], provider);
                    listTemp = this.RemoveByHeight(height);
                    break;
                case "salary":
                    decimal salary = decimal.Parse(inputValueArray[1], provider);
                    listTemp = this.RemoveBySalary(salary);
                    break;
                default:
                    throw new ArgumentException("Not correct value!!!!");
            }

            return new ReadOnlyCollection<int>(listTemp);
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

            try
            {
                this.validator.ValidateParametrs(fileCabinetRecord);
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }

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

            foreach (var item in this.list)
            {
                this.AddRecordToDictionary(item);
            }
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

        private List<int> RemoveById(int id)
        {
            List<int> count = new List<int>();
            FileCabinetRecord[] temp = this.list.ToArray();
            foreach (var removeId in temp)
            {
                if (removeId.Id == id)
                {
                    this.list.Remove(removeId);
                    this.RemoveRecordFromDictionary(removeId);
                    count.Add(removeId.Id);
                }
            }

            return count;
        }

        private List<int> RemoveByFirstName(string firstName)
        {
            List<int> count = new List<int>();
            FileCabinetRecord[] temp = this.list.ToArray();
            foreach (var removeFirstName in temp)
            {
                if (removeFirstName.FirstName.ToLower(new CultureInfo("en-US")) == firstName)
                {
                    this.list.Remove(removeFirstName);
                    this.RemoveRecordFromDictionary(removeFirstName);
                    count.Add(removeFirstName.Id);
                }
            }

            return count;
        }

        private List<int> RemoveByLastName(string lastName)
        {
            List<int> count = new List<int>();
            FileCabinetRecord[] temp = this.list.ToArray();
            foreach (var removeLastName in temp)
            {
                if (removeLastName.LastName.ToLower(new CultureInfo("en-US")) == lastName)
                {
                    this.list.Remove(removeLastName);
                    this.RemoveRecordFromDictionary(removeLastName);
                    count.Add(removeLastName.Id);
                }
            }

            return count;
        }

        private List<int> RemoveByDateOfBirth(DateTime dateOfBirth)
        {
            List<int> count = new List<int>();
            FileCabinetRecord[] temp = this.list.ToArray();
            foreach (var removeDate in temp)
            {
                if (removeDate.DateOfBirth == dateOfBirth)
                {
                    this.list.Remove(removeDate);
                    this.RemoveRecordFromDictionary(removeDate);
                    count.Add(removeDate.Id);
                }
            }

            return count;
        }

        private List<int> RemoveByGender(char sex)
        {
            List<int> count = new List<int>();
            FileCabinetRecord[] temp = this.list.ToArray();
            foreach (var removeSex in temp)
            {
                if (removeSex.Sex == sex)
                {
                    this.list.Remove(removeSex);
                    this.RemoveRecordFromDictionary(removeSex);
                    count.Add(removeSex.Id);
                }
            }

            return count;
        }

        private List<int> RemoveByHeight(short height)
        {
            List<int> count = new List<int>();
            FileCabinetRecord[] temp = this.list.ToArray();
            foreach (var removeHeight in temp)
            {
                if (removeHeight.Height == height)
                {
                    this.list.Remove(removeHeight);
                    this.RemoveRecordFromDictionary(removeHeight);
                    count.Add(removeHeight.Id);
                }
            }

            return count;
        }

        private List<int> RemoveBySalary(decimal salary)
        {
            List<int> count = new List<int>();
            FileCabinetRecord[] temp = this.list.ToArray();
            foreach (var removeSalary in temp)
            {
                if (removeSalary.Salary == salary)
                {
                    this.list.Remove(removeSalary);
                    this.RemoveRecordFromDictionary(removeSalary);
                    count.Add(removeSalary.Id);
                }
            }

            return count;
        }

        private List<FileCabinetRecord> FindRecordsByParameters(string[] parameter)
        {
            CultureInfo provider = new CultureInfo("en-US");

            List<FileCabinetRecord> listTemp = new List<FileCabinetRecord>(this.list);

            foreach (var item in parameter)
            {
                string[] split = item.Split("=");
                listTemp = this.FindRecordsByParameterInList(listTemp, split[0], split[1]);
            }

            return listTemp;
        }

        private List<FileCabinetRecord> FindRecordsByParameterInList(List<FileCabinetRecord> list, string parameter, string value)
        {
            CultureInfo provider = new CultureInfo("en-US");

            List<FileCabinetRecord> listTemp = new List<FileCabinetRecord>();

            switch (parameter)
            {
                case "id":
                    int id = int.Parse(value, provider);
                    listTemp = list.FindAll(item1 => item1.Id == id);
                    break;
                case "firstname":
                    string firstName = value;
                    listTemp = list.FindAll(item1 => item1.FirstName.ToLower(provider) == firstName);
                    break;
                case "lastname":
                    string lastName = value;
                    listTemp = list.FindAll(item1 => item1.LastName.ToLower(provider) == lastName);
                    break;
                case "dateofbirth":
                    DateTime dateOfBirth = DateTime.Parse(value, provider);
                    listTemp = list.FindAll(item1 => item1.DateOfBirth == dateOfBirth);
                    break;
                case "sex":
                    char sex = char.Parse(value);
                    listTemp = list.FindAll(item1 => item1.Sex == sex);
                    break;
                case "height":
                    short height = short.Parse(value, provider);
                    listTemp = list.FindAll(item1 => item1.Height == height);
                    break;
                case "salary":
                    decimal salary = decimal.Parse(value, provider);
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

        private void UpdateRecords(List<FileCabinetRecord> listRecords, string[] inputValueArray)
        {
            CultureInfo provider = new CultureInfo("en-US");

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
                            int id = int.Parse(value, provider);
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
                            DateTime dateOfBirth = DateTime.Parse(value, provider);
                            item.DateOfBirth = dateOfBirth;
                            break;
                        case "sex":
                            char sex = char.Parse(value);
                            item.Sex = sex;
                            break;
                        case "height":
                            short height = short.Parse(value, provider);
                            item.Height = height;
                            break;
                        case "salary":
                            decimal salary = decimal.Parse(value, provider);
                            item.Salary = salary;
                            break;
                        default:
                            throw new ArgumentException("Not correct value!!!!");
                    }
                }

                this.EditRecord(item);
            }
        }
    }
}