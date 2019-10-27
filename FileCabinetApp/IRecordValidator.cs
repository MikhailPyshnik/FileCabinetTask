using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    ///  Exposes an validateParametrs, which supports validate values ​​by criterion.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        ///  Defines a rule for checking values.
        /// </summary>
        /// <param name="fileCabinetRecord">Input parametr record <see cref="FileCabinetRecord"/>.</param>
        void ValidateParametrs(FileCabinetRecord fileCabinetRecord);
    }
}
