using System;

namespace FileCabinetApp.CommonValidator
{
    /// <summary>
    /// Class DateOfBirthValidator.
    /// </summary>
    public class DateOfBirthValidator : IRecordValidator
    {
        private readonly DateTime from;
        private readonly DateTime to;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateOfBirthValidator"/> class.
        /// </summary>
        /// <param name="from">Input parametr start date of birth.<see cref="DateTime"/>.</param>
        /// <param name="to">Input parametr end date of birth.<see cref="DateTime"/>.</param>
        public DateOfBirthValidator(DateTime from, DateTime to)
        {
            this.from = from;
            this.to = to;
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

            var value = fileCabinetRecord.DateOfBirth;

            if (this.from > value || value > this.to)
            {
                throw new ArgumentException("Date Of Birth is incorrect value.");
            }
        }
    }
}
