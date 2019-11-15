using System;
using System.Collections;
using System.Collections.Generic;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Contains methods Print.
    /// </summary>
    public interface IRecordPrinter
    {
        /// <summary>
        /// Method Print.
        /// </summary>
        /// <param name="records">Input parametr record <see cref="IEnumerable"/>.</param>
        // /// <returns>Id <see cref="int"/>.</returns>
        void Print(IEnumerable<FileCabinetRecord> records);
    }
}
