using System;

namespace FileCabinetApp.RecordValidator
{
    /// <summary>
    /// Class DefaultDateOfBirthValidator.
    /// </summary>
    public class DefaultDateOfBirthValidator : IRecordValidator
    {
        /// <summary>
        /// Implements the method ValidateParametrs.
        /// </summary>
        /// <param name="fileCabinetRecord">Input parameter record <see cref="FileCabinetRecord"/>.</param>
        public void ValidateParametrs(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord == null)
            {
                throw new ArgumentNullException($"{nameof(fileCabinetRecord)} is null.");
            }

            var value = fileCabinetRecord.DateOfBirth;
            DateTime tempDate = new DateTime(1950, 1, 01);

            if (tempDate > value || value > DateTime.Now)
            {
                throw new ArgumentException("Date Of Birth is incorrect value.");
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
