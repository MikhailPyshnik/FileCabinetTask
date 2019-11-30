using System;
using FileCabinetApp.Configuration;

namespace FileCabinetApp
{
    /// <summary>
    /// Class extention ValidatorBuilder.
    /// </summary>
    public static class ValidatorBuilderExtention
    {
        /// <summary>
        /// Extention method CreateDefault.
        /// </summary>
        /// <param name="validator">Input parametr min salary.<see cref="ValidatorBuilder"/>.</param>
        /// <param name="defaultValidatorConfiguration">Input parametr for default value.<see cref="ValidationConfiguration"/>.</param>
        /// <returns>Return <see cref="IRecordValidator"/>.</returns>
        public static IRecordValidator CreateDefault(this ValidatorBuilder validator, ValidationConfiguration defaultValidatorConfiguration)
        {
            if (defaultValidatorConfiguration is null)
            {
                throw new ArgumentNullException(nameof(defaultValidatorConfiguration));
            }

            var recordValidator = new ValidatorBuilder().ValidateFirstName(defaultValidatorConfiguration.FirstName.Min, defaultValidatorConfiguration.FirstName.Max).
                                                   ValidateLastName(defaultValidatorConfiguration.LastName.Min, defaultValidatorConfiguration.LastName.Max).
                                                   ValidateDateOfBirth(
                                                   defaultValidatorConfiguration.DateOfBirth.From,
                                                   defaultValidatorConfiguration.DateOfBirth.To)
                                                   .ValidateGender(defaultValidatorConfiguration.Gender.Male, defaultValidatorConfiguration.Gender.Female).
                                                   ValidateHeigth(defaultValidatorConfiguration.Height.Min, defaultValidatorConfiguration.Height.Max).
                                                   ValidateSalary(defaultValidatorConfiguration.Salary.Min, defaultValidatorConfiguration.Salary.Max).
                                                   Create();
            return recordValidator;
        }

        /// <summary>
        /// Extention method CreateCustom.
        /// </summary>
        /// <param name="validator">Input parametr min salary.<see cref="ValidatorBuilder"/>.</param>
        /// <param name="customValidatorConfiguration">Input parametr for custom value.<see cref="ValidationConfiguration"/>.</param>
        /// <returns>Return <see cref="IRecordValidator"/>.</returns>
        public static IRecordValidator CreateCustom(this ValidatorBuilder validator, ValidationConfiguration customValidatorConfiguration)
        {
            if (customValidatorConfiguration is null)
            {
                throw new ArgumentNullException(nameof(customValidatorConfiguration));
            }

            var recordValidator = new ValidatorBuilder().ValidateFirstName(customValidatorConfiguration.FirstName.Min, customValidatorConfiguration.FirstName.Max).
                                                   ValidateLastName(customValidatorConfiguration.LastName.Min, customValidatorConfiguration.LastName.Max).
                                                   ValidateDateOfBirth(
                                                   customValidatorConfiguration.DateOfBirth.From,
                                                   customValidatorConfiguration.DateOfBirth.To)
                                                   .ValidateGender(customValidatorConfiguration.Gender.Male, customValidatorConfiguration.Gender.Female).
                                                   ValidateHeigth(customValidatorConfiguration.Height.Min, customValidatorConfiguration.Height.Max).
                                                   ValidateSalary(customValidatorConfiguration.Salary.Min, customValidatorConfiguration.Salary.Max).
                                                   Create();
            return recordValidator;
        }
    }
}
