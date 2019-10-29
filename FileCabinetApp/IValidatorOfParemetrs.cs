using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    ///  Exposes an validateParametrs, which supports validate values ​​by criterion.
    /// </summary>
    public interface IValidatorOfParemetrs
    {
        /// <summary>
        /// Defines a rule for checking values.
        /// </summary>
        /// <typeparam name="T">Type of parametr.</typeparam>
        /// <param name="converter">The converter.</param>
        /// <param name="validator">The validator.</param>
        /// <returns>Validate parametr.</returns>
        T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator);

        /// <summary>
        /// Convert the first name to string.
        /// </summary>
        /// <param name="value">The converter.</param>
        /// <returns>The correct value.</returns>
        Tuple<bool, string, string> FirstNameConverter(string value);

        /// <summary>
        /// Convert the last name to string.
        /// </summary>
        /// <param name="value">The converter.</param>
        /// <returns>The correct value.</returns>
        Tuple<bool, string, string> LastNameConverter(string value);

        /// <summary>
        /// Convert the day of birth to string.
        /// </summary>
        /// <param name="value">The converter.</param>
        /// <returns>The correct value.</returns>
        Tuple<bool, string, DateTime> DayOfBirthConverter(string value);

        /// <summary>
        /// Convert the sex to string.
        /// </summary>
        /// <param name="value">The converter.</param>
        /// <returns>The correct value.</returns>
        Tuple<bool, string, char> SexConverter(string value);

        /// <summary>
        /// Convert the height to string.
        /// </summary>
        /// <param name="value">The converter.</param>
        /// <returns>The correct value.</returns>
        Tuple<bool, string, short> HeightConverter(string value);

        /// <summary>
        /// Convert the salary to string.
        /// </summary>
        /// <param name="value">The converter.</param>
        /// <returns>The correct value.</returns>
        Tuple<bool, string, decimal> SalaryConverter(string value);

        /// <summary>
        /// Validate the first name.
        /// </summary>
        /// <param name="value">The validator.</param>
        /// <returns>The correct value.</returns>
        Tuple<bool, string> FirstNameValidator(string value);

        /// <summary>
        /// Validate the last name.
        /// </summary>
        /// <param name="value">The validator.</param>
        /// <returns>The correct value.</returns>
        Tuple<bool, string> LastNameValidator(string value);

        /// <summary>
        /// Validate the day of birth.
        /// </summary>
        /// <param name="value">The validator.</param>
        /// <returns>The correct value.</returns>
        Tuple<bool, string> DayOfBirthValidator(DateTime value);

        /// <summary>
        /// Validate the sex.
        /// </summary>
        /// <param name="value">The validator.</param>
        /// <returns>The correct value.</returns>
        Tuple<bool, string> SexValidator(char value);

        /// <summary>
        /// Validate the height.
        /// </summary>
        /// <param name="value">The validator.</param>
        /// <returns>The correct value.</returns>
        Tuple<bool, string> HeightValidator(short value);

        /// <summary>
        /// Validate the salary.
        /// </summary>
        /// <param name="value">The validator.</param>
        /// <returns>The correct value.</returns>
        Tuple<bool, string> SalaryValidator(decimal value);
    }
}
