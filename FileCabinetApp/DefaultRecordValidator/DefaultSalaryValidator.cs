using System;

namespace FileCabinetApp.RecordValidator
{
    /// <summary>
    /// Class DefaultSalaryValidator.
    /// </summary>
    public class DefaultSalaryValidator : IRecordValidator
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
            if (value < 500 || value > 10000)
            {
                throw new ArgumentException("Salary is incorrect value.");
            }
        }
    }
}
