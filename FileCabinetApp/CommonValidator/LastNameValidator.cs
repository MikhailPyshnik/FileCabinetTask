using System;

namespace FileCabinetApp.CommonValidator
{
    /// <summary>
    /// Class LastNameValidator.
    /// </summary>
    public class LastNameValidator : IRecordValidator
    {
        private readonly uint minLenght;
        private readonly uint maxLenght;

        /// <summary>
        /// Initializes a new instance of the <see cref="LastNameValidator"/> class.
        /// </summary>
        /// <param name="minLenght">Input parametr min salary.<see cref="uint"/>.</param>
        /// <param name="maxLenght">Input parametr max salary.<see cref="uint"/>.</param>
        public LastNameValidator(uint minLenght, uint maxLenght)
        {
            this.minLenght = minLenght;
            this.maxLenght = maxLenght;
        }

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

            if (value.Length < this.minLenght || value.Length > this.maxLenght)
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
