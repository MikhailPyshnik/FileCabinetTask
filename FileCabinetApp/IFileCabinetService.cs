using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FileCabinetApp
{
    /// <summary>
    ///  Exposes an validateParametrs, which supports validate values ​​by criterion.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// Gets or sets the Validator of the Program.
        /// </summary>
        /// <value>The Validator of the Program.</value>
        IValidatorOfParemetrs Validator { get; set; }

        /// <summary>
        /// Сreate new record FileCabinetRecord.
        /// </summary>
        /// <param name="fileCabinetRecord">Input parametr record <see cref="FileCabinetRecord"/>.</param>
        /// <returns>Id <see cref="int"/>.</returns>
        int CreateRecord(FileCabinetRecord fileCabinetRecord);

        /// <summary>
        /// Inseert new record FileCabinetRecord.
        /// </summary>
        /// <param name="fileCabinetRecord">Input parametr record <see cref="FileCabinetRecord"/>.</param>
        /// <returns>Id <see cref="int"/>.</returns>
        int Insert(FileCabinetRecord fileCabinetRecord);

        /// <summary>
        /// Get all record FileCabinetRecord.
        /// </summary>
        /// <returns>Rerords <see cref="FileCabinetRecord"/>.</returns>
        ReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>
        /// Get count of record and delete records FileCabinetRecord.
        /// </summary>
        /// <returns>Count records <see cref="Tuple"/>.</returns>
        Tuple<int, int> GetStat();

        /// <summary>
        /// Return records by select.
        /// </summary>
        /// <param name="inputParamentArray">Input parametr array <see cref="string"/>.</param>
        /// <param name="logicalOperator">Input parametr for conditional <see cref="string"/>.</param>
        /// <returns>IEnumerable by firstName <see cref="FileCabinetRecord"/>.</returns>
        IEnumerable<FileCabinetRecord> SelectByCondition(string[] inputParamentArray, string logicalOperator);

        /// <summary>
        /// Return make snapshot.
        /// </summary>
        /// <returns>Rerords by dateofbirth <see cref=" FileCabinetServiceSnapshot"/>.</returns>
        FileCabinetServiceSnapshot MakeSnapshot();

        /// <summary>
        /// Return make snapshot for restore.
        /// </summary>
        /// <param name="fileCabinetServiceSnapshot">Input parametr fileCabinetServiceSnapshot <see cref="FileCabinetServiceSnapshot"/>.</param>
        /// <returns>Counts add import record(s) <see cref="int"/>.</returns>
        int Restore(FileCabinetServiceSnapshot fileCabinetServiceSnapshot);

        /// <summary>
        /// Delete record by predicate.
        /// </summary>
        /// <param name="inputValueArray">Input parametr array <see cref="string"/>.</param>
        /// <returns>ReadOnlyCollection deleted id <see cref="int"/>.</returns>
        ReadOnlyCollection<int> Delete(string[] inputValueArray);

        /// <summary>
        /// Update record by.
        /// </summary>
        /// <param name="inputValueArray">Input value array <see cref="string"/>.</param>
        /// <param name="inputParamentArray">Input parametr array <see cref="string"/>.</param>
        void Update(string[] inputValueArray, string[] inputParamentArray);

        /// <summary>
        /// Delete record by bit in file.
        /// </summary>
        /// <returns>Count records for delete <see cref="Tuple"/>.</returns>
        Tuple<int, int> Purge();
    }
}
