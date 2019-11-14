using System;

namespace FileCabinetApp.CustomDefaultValidator
{
    /// <summary>
    /// Class CustomLastNameValidator.
    /// </summary>
    public class CustomSalaryValidator : IRecordValidator
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

            decimal value = fileCabinetRecord.Salary;
            if (value < 500 || value > 5000)
            {
                throw new ArgumentException("Salary is incorrect value.");
            }
        }
    }
}
