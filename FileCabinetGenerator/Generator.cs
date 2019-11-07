using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Bogus;
using FileCabinetApp;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Work with Generator.Generate random data for record(s).
    /// </summary>
    public class Generator
    {
        private Faker randomDataReneretor = new Faker("en");
        private CultureInfo provider = new CultureInfo("en-US");

        /// <summary>
        /// Generate data of filecabinetrecod by default rule.
        /// </summary>
        /// <param name="startId">Input parametr start id.<see cref="uint"/>.</param>
        /// <param name="countId">Input parametr amount id.<see cref="uint"/>.</param>
        /// <returns>Rerords by lastName <see cref="FileCabinetRecord"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GenerateDefaultValidator(uint startId, uint countId)
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

        private T ReadInput<T>(string inputValue, Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = this.Cause(inputValue);
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

        private string Cause(string inputValue)
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
                    value = this.randomDataReneretor.Random.Decimal(-100, 20000).ToString(this.provider);
                    break;
                default:
                    break;
            }

            return value;
        }
    }
}