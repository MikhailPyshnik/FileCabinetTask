using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Work with records. Save and change record(s).DefaultService.
    /// </summary>
    public class FileCabinetDefaultService : FileCabinetService
    {
        /// <summary>
        /// Сreate new record FileCabinetRecord.
        /// </summary>
        /// <param name="fileCabinetRecord">Input parametr record <see cref="FileCabinetRecord"/>.</param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="fileCabinetRecord"/>, <paramref name="fileCabinetRecord.FirstName"/>,<paramref name="fileCabinetRecord.LastName"/> is null.</exception>
        /// <exception cref="ArgumentException">Throws if <paramref name="fileCabinetRecord.FirstName"/>,<paramref name="fileCabinetRecord.LastName"/>,<paramref name="fileCabinetRecord.DateOfBirth"/>,<paramref name="fileCabinetRecord.Sex"/>,<paramref name="fileCabinetRecord.Height"/>,<paramref name="fileCabinetRecord.Salary"/> is(are) incorrect value.</exception>
        public override void ValidateExtention(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord == null)
            {
                throw new ArgumentNullException($"{nameof(fileCabinetRecord)} is null!");
            }

            if (fileCabinetRecord.FirstName == null)
            {
                throw new ArgumentNullException($"First name {nameof(fileCabinetRecord.FirstName)} is null.");
            }

            if (fileCabinetRecord.FirstName.Length < 2 || fileCabinetRecord.FirstName.Length > 60)
            {
                throw new ArgumentException("First name is incorrect value.");
            }

            if (FileCabinetDefaultService.WhiteSpace(fileCabinetRecord.FirstName))
            {
                throw new ArgumentException("First name consists of spaces.");
            }

            if (fileCabinetRecord.LastName == null)
            {
                throw new ArgumentNullException($"Last name {nameof(fileCabinetRecord.LastName)} is null.");
            }

            if (fileCabinetRecord.LastName.Length < 2 || fileCabinetRecord.LastName.Length > 60)
            {
                throw new ArgumentException("Last name is incorrect value.");
            }

            if (FileCabinetDefaultService.WhiteSpace(fileCabinetRecord.LastName))
            {
                throw new ArgumentException("Last name consists of spaces.");
            }

            DateTime date1 = new DateTime(1950, 1, 01);

            if (date1 > fileCabinetRecord.DateOfBirth || fileCabinetRecord.DateOfBirth > DateTime.Now)
            {
                throw new ArgumentException("Date Of Birth is incorrect value.");
            }

            if (!'F'.Equals(fileCabinetRecord.Sex) & !'f'.Equals(fileCabinetRecord.Sex) & !'M'.Equals(fileCabinetRecord.Sex) & !'m'.Equals(fileCabinetRecord.Sex))
            {
                throw new ArgumentException("Sex is incorrect value.");
            }

            if (fileCabinetRecord.Height < 60 || fileCabinetRecord.Height > 230)
            {
                throw new ArgumentException("Height is incorrect value.");
            }

            if (fileCabinetRecord.Salary < 500 || fileCabinetRecord.Salary > 10000)
            {
                throw new ArgumentException("Salary is incorrect value.");
            }
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
    }
}
