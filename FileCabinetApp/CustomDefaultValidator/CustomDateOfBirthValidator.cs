using System;

namespace FileCabinetApp.CustomDefaultValidator
{
    /// <summary>
    /// Class CustomDateOfBirthValidator.
    /// </summary>
    public class CustomDateOfBirthValidator : IRecordValidator
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
            DateTime tempDate = new DateTime(1939, 1, 09);

            if (tempDate > value || value > DateTime.Now)
            {
                throw new ArgumentException("Date Of Birth is incorrect value.");
            }
        }
    }
}
