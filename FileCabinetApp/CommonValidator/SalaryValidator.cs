using System;

namespace FileCabinetApp.CommonValidator
{
    /// <summary>
    /// Class SalaryValidator.
    /// </summary>
    public class SalaryValidator : IRecordValidator
    {
        private readonly decimal minSalary;
        private readonly decimal maxSalary;

        /// <summary>
        /// Initializes a new instance of the <see cref="SalaryValidator"/> class.
        /// </summary>
        /// <param name="minSalary">Input parametr min salary.<see cref="decimal"/>.</param>
        /// <param name="maxSalary">Input parametr max salary.<see cref="decimal"/>.</param>
        public SalaryValidator(decimal minSalary, decimal maxSalary)
        {
            this.minSalary = minSalary;
            this.maxSalary = maxSalary;
        }

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

            decimal value = fileCabinetRecord.Salary;
            if (value < this.minSalary || value > this.maxSalary)
            {
                throw new ArgumentException("Salary is incorrect value.");
            }
        }
    }
}
