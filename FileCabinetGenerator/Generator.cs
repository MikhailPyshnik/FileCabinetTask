using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using Bogus;
using FileCabinetApp;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Work with Generator.Generate random data for record(s).
    /// </summary>
    public class Generator
    {
        private readonly uint startId;
        private readonly uint countId;
        private Faker randomDataReneretor = new Faker("en");
        private CultureInfo provider = new CultureInfo("en-US");

        /// <summary>
        /// Initializes a new instance of the <see cref="Generator"/> class.
        /// </summary>
        /// <param name="startId">Input parametr start id.<see cref="uint"/>.</param>
        /// <param name="countId">Input parametr amount of records.<see cref="uint"/>.</param>
        public Generator(uint startId, uint countId)
        {
            this.startId = startId;
            this.countId = countId;
        }

        /// <summary>
        /// Generate data of filecabinetrecod by default rule.
        /// </summary>
        /// <param name="startId">Input parametr start id.<see cref="uint"/>.</param>
        /// <param name="countId">Input parametr amount id.<see cref="uint"/>.</param>
        /// <returns>Rerords by lastName <see cref="FileCabinetRecord"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GenerateRandomDateDefaultValidator(uint startId, uint countId)
        {
            List<FileCabinetRecord> list = new List<FileCabinetRecord>((int)countId);

            var recordValidator = new DefaultValidator();

            for (uint i = startId; i < startId + countId; i++)
            {
                FileCabinetRecord fileCabinetRecord = new FileCabinetRecord();

                fileCabinetRecord.Id = (int)i;
                fileCabinetRecord.FirstName = this.ReadInput("firstname", recordValidator.FirstNameConverter, recordValidator.FirstNameValidator);
                fileCabinetRecord.LastName = this.ReadInput("lastname", recordValidator.LastNameConverter, recordValidator.LastNameValidator);
                fileCabinetRecord.DateOfBirth = this.ReadInput("date", recordValidator.DayOfBirthConverter, recordValidator.DayOfBirthValidator);
                fileCabinetRecord.Sex = this.ReadInput("sex", recordValidator.SexConverter, recordValidator.SexValidator);
                fileCabinetRecord.Height = this.ReadInput("height", recordValidator.HeightConverter, recordValidator.HeightValidator);
                fileCabinetRecord.Salary = this.ReadInput("salary", recordValidator.SalaryConverter, recordValidator.SalaryValidator);

                list.Add(fileCabinetRecord);
            }

            return new ReadOnlyCollection<FileCabinetRecord>(list);
        }

        /// <summary>
        /// Import records to csv file.
        /// </summary>
        /// <param name=" thePathToTheFile">Input parametr start id.<see cref="string"/>.</param>
        public void ImportToCSV(string thePathToTheFile)
        {
            string text = null;
            StreamWriter streamWriterToCsv = null;
            var records = this.GenerateRandomDateDefaultValidator(this.startId, this.countId);
            try
            {
                streamWriterToCsv = new StreamWriter(thePathToTheFile, false, System.Text.Encoding.Default);
                streamWriterToCsv.WriteLine("Id, First Name, Last Name, Date of Birth, Sex, Height, Salary.");
                foreach (var record in records)
                {
                    text = FileCabinetRecordToString(record);
                    streamWriterToCsv.WriteLine(text);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new ArgumentException($"{ex.Message}");
            }
            finally
            {
                if (streamWriterToCsv != null)
                {
                    streamWriterToCsv.Close();
                }
            }
        }

        private static string FileCabinetRecordToString(FileCabinetRecord record)
        {
            var sb = new StringBuilder();
            sb.Append(record.Id + "," + record.FirstName + "," + record.LastName + "," + record.DateOfBirth.ToString("yyyy-MMM-dd", new CultureInfo("en-US")) + "," + record.Sex + "," + record.Height + "," + record.Salary);
            return sb.ToString();
        }

        private T ReadInput<T>(string inputValue, Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = this.GenerateRandomValueFor(inputValue);
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    continue;
                }

                return value;
            }
            while (true);
        }

        private string GenerateRandomValueFor(string inputValue)
        {
            string value = null;
            switch (inputValue)
            {
                case "firstname":
                    value = this.randomDataReneretor.Name.FirstName();
                    break;
                case "lastname":
                    value = this.randomDataReneretor.Name.LastName();
                    break;
                case "date":
                    value = this.randomDataReneretor.Date.Between(new DateTime(1900, 1, 01), new DateTime(2019, 10, 7)).ToString("yyyy-MMM-dd", new CultureInfo("en-US"));
                    break;
                case "sex":
                    value = this.randomDataReneretor.Random.Char('A', 'z').ToString(this.provider);
                    break;
                case "height":
                    value = this.randomDataReneretor.Random.Short().ToString(this.provider);
                    break;
                case "salary":
                    decimal salaryRandom = this.randomDataReneretor.Random.Decimal(0, 20000);
                    decimal salaryCorrect = decimal.Round(salaryRandom, 0);
                    value = salaryCorrect.ToString(this.provider);
                    break;
                default:
                    break;
            }

            return value;
        }
    }
}