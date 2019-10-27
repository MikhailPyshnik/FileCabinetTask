using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Work with records. Save and change record(s).
    /// </summary>
    public class FileCabinetCustomService : FileCabinetService
    {
        /// <summary>
        /// Сreate custom implementation of validator.
        /// </summary>
        /// <returns>Id <see cref="IRecordValidator"/>.</returns>
        public override IRecordValidator CreateValidator()
        {
            return new CustomValidator();
        }
    }
}
