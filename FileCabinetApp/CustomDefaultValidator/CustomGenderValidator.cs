using System;

namespace FileCabinetApp.CustomDefaultValidator
{
    /// <summary>
    /// Class CustomGenderValidator.
    /// </summary>
    public class CustomGenderValidator : IRecordValidator
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

            var value = fileCabinetRecord.Sex;
            if (!'F'.Equals(value) && !'M'.Equals(value))
            {
                throw new ArgumentException("Sex is incorrect value.");
            }
        }
    }
}
