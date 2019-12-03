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
        private static readonly CultureInfo Provider = new CultureInfo("en-US");

        private readonly IRecordValidator validator;

        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        private readonly Dictionary<string, List<FileCabinetRecord>> memoizationDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.OrdinalIgnoreCase);

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
        /// Gets the type storage of filecabinet.
        /// </summary>
        /// <value>The Status of the record.</value>
        public IFileCabinetService FileCabinetProperties { get => new FileCabinetMemoryService(this.validator); }

        /// <summary>
        /// Implementation IFileCabinetService СreateRecod.
        /// </summary>
        /// <param name="fileCabinetRecord">Input parametr record <see cref="FileCabinetRecord"/>.</param>
        /// <returns>Id <see cref="int"/>.</returns>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="fileCabinetRecord"/>, <paramref name="fileCabinetRecord.FirstName"/>,<paramref name="fileCabinetRecord.LastName"/> is null.</exception>
        /// <exception cref="ArgumentException">Throws if <paramref name="fileCabinetRecord.FirstName"/>,<paramref name="fileCabinetRecord.LastName"/>,<paramref name="fileCabinetRecord.DateOfBirth"/>,<paramref name="fileCabinetRecord.Sex"/>,<paramref name="fileCabinetRecord.Height"/>,<paramref name="fileCabinetRecord.Salary"/> is(are) incorrect value.</exception>
        public int CreateRecord(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord is null)
            {
                throw new ArgumentNullException($"{nameof(fileCabinetRecord)} is null!");
            }

            this.validator.ValidateParametrs(fileCabinetRecord);
            if (this.list.Count == 0)
            {
                fileCabinetRecord.Id = 1;
                this.list.Add(fileCabinetRecord);
            }
            else
            {
                fileCabinetRecord.Id = this.list[this.list.Count - 1].Id + 1;
                this.list.Add(fileCabinetRecord);
            }

            this.memoizationDictionary.Clear();
            return fileCabinetRecord.Id;
        }

        /// <summary>
        /// Implementation IFileCabinetService Insert.
        /// </summary>
        /// <param name="fileCabinetRecord">Input parametr record <see cref="FileCabinetRecord"/>.</param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="fileCabinetRecord"/>, <paramref name="fileCabinetRecord.FirstName"/>,<paramref name="fileCabinetRecord.LastName"/> is null.</exception>
        /// <exception cref="ArgumentException">Throws if <paramref name="fileCabinetRecord.FirstName"/>,<paramref name="fileCabinetRecord.LastName"/>,<paramref name="fileCabinetRecord.DateOfBirth"/>,<paramref name="fileCabinetRecord.Sex"/>,<paramref name="fileCabinetRecord.Height"/>,<paramref name="fileCabinetRecord.Salary"/> is(are) incorrect value.</exception>
        /// <returns>Id<see cref="int"/>.</returns>
        public int Insert(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord is null)
            {
                throw new ArgumentNullException($"{nameof(fileCabinetRecord)} is null!");
            }

            if (this.list.Exists(x => x.Id == fileCabinetRecord.Id))
            {
                return 0;
            }

            this.validator.ValidateParametrs(fileCabinetRecord);
            this.list.Add(fileCabinetRecord);
            this.memoizationDictionary.Clear();
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
        public Tuple<int, int> GetStat()
        {
            int countRecordInFile = this.list.Count;
            int countDeleteRecord = 0;
            return new Tuple<int, int>(countRecordInFile, countDeleteRecord);
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

            List<FileCabinetRecord> result = this.FindRecordsByParameters(inputParamentArray, "and");
            this.UpdateRecords(result, inputValueArray);
            this.memoizationDictionary.Clear();
        }

        /// <summary>
        /// Return records by select in MemoryService.
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

            string parametrSearch = string.Join(",", inputParamentArray);

            List<FileCabinetRecord> result = new List<FileCabinetRecord>();

            if (this.memoizationDictionary.ContainsKey(parametrSearch))
            {
                result = this.memoizationDictionary[parametrSearch];
                this.memoizationDictionary.Clear();
            }
            else
            {
                result = this.FindRecordsByParameters(inputParamentArray, logicalOperator);
                this.memoizationDictionary.Add(parametrSearch, result);
            }

            return result;
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

            this.memoizationDictionary.Clear();

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
                    throw new ArgumentException("Not correct value!!!!");
            }

            this.memoizationDictionary.Clear();

            return new ReadOnlyCollection<int>(listTemp);
        }

        /// <summary>
        /// Implementation IFileCabinetService Purge.
        /// </summary>
        /// <returns>Count records for delete <see cref="Tuple"/>.</returns>
        public Tuple<int, int> Purge()
        {
            int countDeletedRecords = 0;
            int countRecordInFile = this.list.Count;
            return new Tuple<int, int>(countDeletedRecords, countRecordInFile);
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

        private void EditRecord(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord is null)
            {
                throw new ArgumentNullException($"{nameof(fileCabinetRecord)} is null!");
            }

            this.validator.ValidateParametrs(fileCabinetRecord);

            FileCabinetRecord res = this.list.Find(item1 => item1.Id == fileCabinetRecord.Id);
            string oldFirstName = res.FirstName;
            string oldLastName = res.LastName;
            string oldDateOfBirth = res.DateOfBirth.ToString("yyyy-MMM-dd", Provider);
            res.FirstName = fileCabinetRecord.FirstName;
            res.LastName = fileCabinetRecord.LastName;
            res.DateOfBirth = fileCabinetRecord.DateOfBirth;
            res.Sex = fileCabinetRecord.Sex;
            res.Height = fileCabinetRecord.Height;
            res.Salary = fileCabinetRecord.Salary;
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

        private List<int> RemoveById(int id)
        {
            List<int> count = new List<int>();
            FileCabinetRecord[] temp = this.list.ToArray();
            foreach (var removeId in temp)
            {
                if (removeId.Id == id)
                {
                    this.list.Remove(removeId);
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
                if (removeFirstName.FirstName.Equals(firstName, StringComparison.InvariantCultureIgnoreCase))
                {
                    this.list.Remove(removeFirstName);
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
                if (removeLastName.LastName.Equals(lastName, StringComparison.InvariantCultureIgnoreCase))
                {
                    this.list.Remove(removeLastName);
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
                    count.Add(removeSalary.Id);
                }
            }

            return count;
        }

        private List<FileCabinetRecord> FindRecordsByParameters(string[] parameter, string logicParametr)
        {
            List<FileCabinetRecord> listTemp = new List<FileCabinetRecord>(this.list);

            if (logicParametr == "and")
            {
                foreach (var item in parameter)
                {
                    string[] split = item.Split("=");
                    listTemp = this.FindRecordsByParameterInListAnd(listTemp, split[0], split[1]);
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

        private List<FileCabinetRecord> FindRecordsByParameterInListAnd(List<FileCabinetRecord> list, string parameter, string value)
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
                    throw new ArgumentException("Not correct value in update!");
            }

            if (listTemp.Count == 0)
            {
                throw new ArgumentException("Don't find records for update by conditional!");
            }

            return listTemp;
        }

        private HashSet<FileCabinetRecord> FindRecordsByParameterInListOr(HashSet<FileCabinetRecord> set, string parameter, string value)
        {
            switch (parameter)
            {
                case "id":
                    int id = int.Parse(value, Provider);
                    set.UnionWith(this.list.FindAll(item1 => item1.Id == id));
                    break;
                case "firstname":
                    string firstName = value;
                    set.UnionWith(this.list.FindAll(item1 => item1.FirstName.Equals(firstName, StringComparison.InvariantCultureIgnoreCase)));
                    break;
                case "lastname":
                    string lastName = value;
                    set.UnionWith(this.list.FindAll(item1 => item1.LastName.Equals(lastName, StringComparison.InvariantCultureIgnoreCase)));
                    break;
                case "dateofbirth":
                    DateTime dateOfBirth = DateTime.Parse(value, Provider);
                    set.UnionWith(this.list.FindAll(item1 => item1.DateOfBirth == dateOfBirth));
                    break;
                case "sex":
                    char sex = char.Parse(value);
                    set.UnionWith(this.list.FindAll(item1 => item1.Sex == sex));
                    break;
                case "height":
                    short height = short.Parse(value, Provider);
                    set.UnionWith(this.list.FindAll(item1 => item1.Height == height));
                    break;
                case "salary":
                    decimal salary = decimal.Parse(value, Provider);
                    set.UnionWith(this.list.FindAll(item1 => item1.Salary == salary));
                    break;
                default:
                    throw new ArgumentException("Not correct value!!!!");
            }

            if (false)
            {
                throw new ArgumentException("Don't find records for update by conditional!");
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
    }
}