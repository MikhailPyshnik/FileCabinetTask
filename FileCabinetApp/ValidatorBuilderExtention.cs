using System;

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
        /// <returns>Return <see cref="IRecordValidator"/>.</returns>
        public static IRecordValidator CreateDefault(this ValidatorBuilder validator)
        {
           var recordValidator = new ValidatorBuilder().ValidateFirstName(2, 60).
                                                   ValidateLastName(2, 60).
                                                   ValidateDateOfBirth(
                                                   new DateTime(1950, 1, 01),
                                                   DateTime.Now).ValidateGender('M', 'F').
                                                   ValidateHeigth(60, 220).
                                                   ValidateSalary(500, 10000).
                                                   Create();
           return recordValidator;
        }

        /// <summary>
        /// Extention method CreateCustom.
        /// </summary>
        /// <param name="validator">Input parametr min salary.<see cref="ValidatorBuilder"/>.</param>
        /// <returns>Return <see cref="IRecordValidator"/>.</returns>
        public static IRecordValidator CreateCustom(this ValidatorBuilder validator)
        {
            var recordValidator = new ValidatorBuilder().ValidateFirstName(2, 20).
                                                 ValidateLastName(2, 20).
                                                 ValidateDateOfBirth(
                                                 new DateTime(1939, 1, 01),
                                                 DateTime.Now).ValidateGender('m', 'f').
                                                 ValidateHeigth(100, 220).
                                                 ValidateSalary(500, 5000).
                                                 Create();
            return recordValidator;
        }
    }
}
