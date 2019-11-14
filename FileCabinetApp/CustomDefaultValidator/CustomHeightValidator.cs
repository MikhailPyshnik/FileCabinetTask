using System;

namespace FileCabinetApp.CustomDefaultValidator
{
    /// <summary>
    /// Class CustomHeightValidator.
    /// </summary>
    public class CustomHeightValidator : IRecordValidator
    {
        /// <summary>
        /// Implements the method ValidateParameters.
        /// </summary>
        /// <param name="fileCabinetRecord">Input parameter record <see cref="FileCabinetRecord"/>.</param>
        public void ValidateParametrs(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord == null)
            {
                throw new ArgumentNullException($"{nameof(fileCabinetRecord)} is null.");
            }

            var value = fileCabinetRecord.Height;
            if (value < 100 || value > 230)
            {
                throw new ArgumentException("Height is incorrect value.");
            }
        }
    }
}
