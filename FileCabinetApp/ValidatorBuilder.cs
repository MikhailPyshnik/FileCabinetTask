using System;
using System.Collections.Generic;
using FileCabinetApp.CommonValidator;

namespace FileCabinetApp
{
    /// <summary>
    /// Class ValidatorBuilder.
    /// </summary>
    public class ValidatorBuilder
    {
        private readonly List<IRecordValidator> validators = new List<IRecordValidator>();

        /// <summary>
        /// Validate first name.
        /// </summary>
        /// <param name="minLenght">Input parametr min salary.<see cref="uint"/>.</param>
        /// <param name="maxLenght">Input parametr max salary.<see cref="uint"/>.</param>
        /// <returns>Return <see cref="ValidatorBuilder"/>.</returns>
        public ValidatorBuilder ValidateFirstName(uint minLenght, uint maxLenght)
        {
            this.validators.Add(new FirstNameValidator(minLenght, maxLenght));
            return this;
        }

        /// <summary>
        /// Validate last name.
        /// </summary>
        /// <param name="minLenght">Input parametr min salary.<see cref="uint"/>.</param>
        /// <param name="maxLenght">Input parametr max salary.<see cref="uint"/>.</param>
        /// <returns>Return <see cref="ValidatorBuilder"/>.</returns>
        public ValidatorBuilder ValidateLastName(uint minLenght, uint maxLenght)
        {
            this.validators.Add(new LastNameValidator(minLenght, maxLenght));
            return this;
        }

        /// <summary>
        /// Validate date of birth.
        /// </summary>
        /// <param name="from">Input parametr start date of birth.<see cref="DateTime"/>.</param>
        /// <param name="to">Input parametr end date of birth.<see cref="DateTime"/>.</param>
        /// <returns>Return <see cref="ValidatorBuilder"/>.</returns>
        public ValidatorBuilder ValidateDateOfBirth(DateTime from, DateTime to)
        {
            this.validators.Add(new DateOfBirthValidator(from, to));
            return this;
        }

        /// <summary>
        /// Validate gender.
        /// </summary>
        /// <param name="maleSex">Input parameter male sex.<see cref="char"/>.</param>
        /// <param name="femaleSex">Input parameter female sex.<see cref="char"/>.</param>
        /// <returns>Return <see cref="ValidatorBuilder"/>.</returns>
        public ValidatorBuilder ValidateGender(char maleSex, char femaleSex)
        {
            this.validators.Add(new GenderValidator(maleSex, femaleSex));
            return this;
        }

        /// <summary>
        /// Validate heigth.
        /// </summary>
        /// <param name="minHeight">Input parametr min height.<see cref="short"/>.</param>
        /// <param name="maxHeight">Input parametr max height.<see cref="short"/>.</param>
        /// <returns>Return <see cref="ValidatorBuilder"/>.</returns>
        public ValidatorBuilder ValidateHeigth(short minHeight, short maxHeight)
        {
            this.validators.Add(new HeightValidator(minHeight, maxHeight));
            return this;
        }

        /// <summary>
        /// Validate salary.
        /// </summary>
        /// <param name="minSalary">Input parametr min salary.<see cref="decimal"/>.</param>
        /// <param name="maxSalary">Input parametr max salary.<see cref="decimal"/>.</param>
        /// <returns>Return <see cref="ValidatorBuilder"/>.</returns>
        public ValidatorBuilder ValidateSalary(decimal minSalary, decimal maxSalary)
        {
            this.validators.Add(new SalaryValidator(minSalary, maxSalary));
            return this;
        }

        /// <summary>
        /// Create new CompositeValidator.
        /// </summary>
        /// <returns>Return <see cref="IRecordValidator"/>.</returns>
        public IRecordValidator Create()
        {
            return new CompositeValidator(this.validators);
        }
    }
}
