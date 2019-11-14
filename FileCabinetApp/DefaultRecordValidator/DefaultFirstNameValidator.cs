using System;

namespace FileCabinetApp.RecordValidator
{
    /// <summary>
    /// Class DefaultFirstNameValidator.
    /// </summary>
    public class DefaultFirstNameValidator : IRecordValidator
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
