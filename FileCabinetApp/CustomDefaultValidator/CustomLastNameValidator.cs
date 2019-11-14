using System;

namespace FileCabinetApp.CustomDefaultValidator
{
    /// <summary>
    /// Class CustomLastNameValidator.
    /// </summary>
    public class CustomLastNameValidator : IRecordValidator
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

            string value = fileCabinetRecord.LastName;
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Last name is null.");
            }

            if (value.Length < 2 || value.Length > 20)
            {
                throw new ArgumentException("Last name is incorrect value.");
            }

            if (WhiteSpace(value))
            {
                throw new ArgumentException("Last name consists of spaces.");
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
