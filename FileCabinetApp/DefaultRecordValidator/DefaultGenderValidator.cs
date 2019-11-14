using System;

namespace FileCabinetApp.RecordValidator
{
    /// <summary>
    /// Class DefaultGenderValidator.
    /// </summary>
    public class DefaultGenderValidator : IRecordValidator
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
            if (!'F'.Equals(value) & !'f'.Equals(value) & !'M'.Equals(value) & !'m'.Equals(value))
            {
                throw new ArgumentException("Sex is incorrect value.");
            }
        }
    }
}
