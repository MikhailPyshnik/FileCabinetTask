using System;

namespace FileCabinetApp.RecordValidator
{
    /// <summary>
    /// Class DefaultHeightValidator.
    /// </summary>
    public class DefaultHeightValidator : IRecordValidator
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

            var value = fileCabinetRecord.Height;
            if (value < 60 || value > 230)
            {
                throw new ArgumentException("Height is incorrect value.");
            }
        }
    }
}
