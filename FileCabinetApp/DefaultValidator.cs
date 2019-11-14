using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Class of console application.
    /// </summary>
    public class DefaultValidator : IValidatorOfParemetrs, IRecordValidator
    {
        /// <summary>
        /// Implement the method ReadInput.
        /// </summary>
        /// <typeparam name="T">Type of parametr.</typeparam>
        /// <param name="converter">The converter.</param>
        /// <param name="validator">The validator.</param>
        /// <returns>Validate parametr.</returns>
        public T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }

        /// <summary>
        /// Implements the method FirstNameConverter.
        /// </summary>
        /// <param name="value">The converter.</param>
        /// <returns>The correct value.</returns>
        public Tuple<bool, string, string> FirstNameConverter(string value)
        {
            bool result = true;
            if (string.IsNullOrEmpty(value) || value.Trim().Length == 0)
            {
                result = false;
            }

            return new Tuple<bool, string, string>(result, "FirstName - null or empty", value);
        }

        /// <summary>
        /// Implements the method LastNameConverter.
        /// </summary>
        /// <param name="value">The converter.</param>
        /// <returns>The correct value.</returns>
        public Tuple<bool, string, string> LastNameConverter(string value)
        {
            bool result = true;
            if (string.IsNullOrEmpty(value) || value.Trim().Length == 0)
            {
                result = false;
            }

            return new Tuple<bool, string, string>(result, "FirstName - null or empty", value);
        }

        /// <summary>
        /// Implements the method DayOfBirthConverter.
        /// </summary>
        /// <param name="value">The converter.</param>
        /// <returns>The correct value.</returns>
        public Tuple<bool, string, DateTime> DayOfBirthConverter(string value)
        {
            DateTime dateTime;
            bool result = DateTime.TryParse(value, out dateTime);
            return new Tuple<bool, string, DateTime>(result, "DayOfBirth - is incorrect", dateTime);
        }

        /// <summary>
        /// Implements the method SexConverter.
        /// </summary>
        /// <param name="value">The converter.</param>
        /// <returns>The correct value.</returns>
        public Tuple<bool, string, char> SexConverter(string value)
        {
            char sex;
            bool result = char.TryParse(value, out sex);
            return new Tuple<bool, string, char>(result, "Sex - is incorrect", sex);
        }

        /// <summary>
        /// Implements the method HeightConverter.
        /// </summary>
        /// <param name="value">The converter.</param>
        /// <returns>The correct value.</returns>
        public Tuple<bool, string, short> HeightConverter(string value)
        {
            short height;
            bool result = short.TryParse(value, out height);
            return new Tuple<bool, string, short>(result, "Height - is incorrect", height);
        }

        /// <summary>
        /// Implements the method SalaryConverter.
        /// </summary>
        /// <param name="value">The converter.</param>
        /// <returns>The correct value.</returns>
        public Tuple<bool, string, decimal> SalaryConverter(string value)
        {
            decimal salary;
            bool result = decimal.TryParse(value, out salary);
            return new Tuple<bool, string, decimal>(result, "Salary - is incorrect", salary);
        }

        /// <summary>
        /// Validate the method FirstNameValidator.
        /// </summary>
        /// <param name="value">The validator.</param>
        /// <returns>The correct value.</returns>
        public Tuple<bool, string> FirstNameValidator(string value)
        {
            bool result = true;
            string message = null;
            if (string.IsNullOrEmpty(value))
            {
                result = false;
                message = "First name is null";
            }

            if (value.Length < 2 || value.Length > 60)
            {
                result = false;
                message = "First name is incorrect value";
            }

            if (WhiteSpace(value))
            {
                result = false;
                message = "First name consists of spaces.";
            }

            return new Tuple<bool, string>(result, message);
        }

        /// <summary>
        /// Validate the method LastNameValidator.
        /// </summary>
        /// <param name="value">The validator.</param>
        /// <returns>The correct value.</returns>
        public Tuple<bool, string> LastNameValidator(string value)
        {
            bool result = true;
            string message = null;
            if (string.IsNullOrEmpty(value))
            {
                result = false;
                message = "Last  name is null";
            }

            if (value.Length < 2 || value.Length > 60)
            {
                result = false;
                message = "Last name is incorrect value";
            }

            if (WhiteSpace(value))
            {
                result = false;
                message = "Last name consists of spaces";
            }

            return new Tuple<bool, string>(result, message);
        }

        /// <summary>
        /// Validate the method DayOfBirthValidator.
        /// </summary>
        /// <param name="value">The validator.</param>
        /// <returns>The correct value.</returns>
        public Tuple<bool, string> DayOfBirthValidator(DateTime value)
        {
            bool result = true;
            string message = null;
            DateTime date1 = new DateTime(1950, 1, 01);

            if (date1 > value || value > DateTime.Now)
            {
                result = false;
                message = "Date Of Birth is incorrect value";
            }

            return new Tuple<bool, string>(result, message);
        }

        /// <summary>
        /// Validate the method SexValidator.
        /// </summary>
        /// <param name="value">The validator.</param>
        /// <returns>The correct value.</returns>
        public Tuple<bool, string> SexValidator(char value)
        {
            bool result = true;
            string message = null;
            if (!'F'.Equals(value) & !'f'.Equals(value) & !'M'.Equals(value) & !'m'.Equals(value))
            {
                result = false;
                message = "Sex is incorrect value";
            }

            return new Tuple<bool, string>(result, message);
        }

        /// <summary>
        /// Validate the method HeightValidator.
        /// </summary>
        /// <param name="value">The validator.</param>
        /// <returns>The correct value.</returns>
        public Tuple<bool, string> HeightValidator(short value)
        {
            bool result = true;
            string message = null;
            if (value < 60 || value > 230)
            {
                result = false;
                message = "Height is incorrect value";
            }

            return new Tuple<bool, string>(result, message);
        }

        /// <summary>
        /// Validate the method SalaryValidatorsalary.
        /// </summary>
        /// <param name="value">The validator.</param>
        /// <returns>The correct value.</returns>
        public Tuple<bool, string> SalaryValidator(decimal value)
        {
            bool result = true;
            string message = null;
            if (value < 500 || value > 10000)
            {
                result = false;
                message = "Salary is incorrect value";
            }

            return new Tuple<bool, string>(result, message);
        }

        /// <summary>
        /// Implements the method FirstNameConverter.
        /// </summary>
        /// <param name="fileCabinetRecord">Input parametr record <see cref="FileCabinetRecord"/>.</param>
        public void ValidateParametrs(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord == null)
            {
                throw new ArgumentNullException(nameof(fileCabinetRecord));
            }

            this.ValidateFirstName(fileCabinetRecord);
            this.ValidateLastName(fileCabinetRecord);
            this.ValidateDateOfBitrh(fileCabinetRecord);
            this.ValidateSex(fileCabinetRecord);
            this.ValidateHeight(fileCabinetRecord);
            this.ValidateSalary(fileCabinetRecord);
        }

        private static bool WhiteSpace(string value)
        {
            bool result = true;
            for (int i = 0; i < value.Length; i++)
            {
                if (!char.IsWhiteSpace(value[i]))
                {
                    result = false;
                    return result;
                }
            }

            return result;
        }

        private void ValidateFirstName(FileCabinetRecord fileCabinetRecord)
        {
            string value = fileCabinetRecord.FirstName;
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("First name is null.");
            }

            if (value.Length < 2 || value.Length > 60)
            {
                throw new ArgumentException("First name is incorrect value.");
            }

            if (WhiteSpace(value))
            {
                throw new ArgumentException("First name consists of spaces.");
            }
        }

        private void ValidateLastName(FileCabinetRecord fileCabinetRecord)
        {
            string value = fileCabinetRecord.LastName;
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Last  name is null.");
            }

            if (value.Length < 2 || value.Length > 60)
            {
                throw new ArgumentException("Last name is incorrect value.");
            }

            if (WhiteSpace(value))
            {
                throw new ArgumentException("Last name consists of spaces.");
            }
        }

        private void ValidateDateOfBitrh(FileCabinetRecord fileCabinetRecord)
        {
            var value = fileCabinetRecord.DateOfBirth;
            DateTime date1 = new DateTime(1950, 1, 01);

            if (date1 > value || value > DateTime.Now)
            {
                throw new ArgumentException("Date Of Birth is incorrect value.");
            }
        }

        private void ValidateSex(FileCabinetRecord fileCabinetRecord)
        {
            var value = fileCabinetRecord.Sex;
            if (!'F'.Equals(value) & !'f'.Equals(value) & !'M'.Equals(value) & !'m'.Equals(value))
            {
                throw new ArgumentException("Sex is incorrect value.");
            }
        }

        private void ValidateHeight(FileCabinetRecord fileCabinetRecord)
        {
            var value = fileCabinetRecord.Height;
            if (value < 60 || value > 230)
            {
                throw new ArgumentException("Height is incorrect value.");
            }
        }

        private void ValidateSalary(FileCabinetRecord fileCabinetRecord)
        {
            decimal value = fileCabinetRecord.Salary;
            if (value < 500 || value > 10000)
            {
                throw new ArgumentException("Salary is incorrect value.");
            }
        }
    }
}
