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
        // /// <returns>Id <see cref="int"/>.</returns>
        void Insert(FileCabinetRecord fileCabinetRecord);

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
        /// Edit record by id.
        /// </summary>
        /// <param name="fileCabinetRecord">Input parametr record <see cref="FileCabinetRecord"/>.</param>
        void EditRecord(FileCabinetRecord fileCabinetRecord);

        /// <summary>
        /// Return records by first name.
        /// </summary>
        /// <param name="firstName">Input parametr FirstName <see cref="string"/>.</param>
        /// <returns>IEnumerable by firstName <see cref="FileCabinetRecord"/>.</returns>
        IEnumerable<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>
        /// Return records by last name.
        /// </summary>
        /// <param name="lastName">Input parametr LastName <see cref="string"/>.</param>
        /// <returns>IEnumerable by lastName <see cref="FileCabinetRecord"/>.</returns>
        IEnumerable<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>
        /// Return records by date of birth.
        /// </summary>
        /// <param name="dateofbirth">Input parametr DataOfBitrh <see cref="string"/>.</param>
        /// <returns>IEnumerable by dateofbirth <see cref="FileCabinetRecord"/>.</returns>
        IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateofbirth);

        /// <summary>
        /// Return make snapshot.
        /// </summary>
        /// <returns>Rerords by dateofbirth <see cref=" FileCabinetServiceSnapshot"/>.</returns>
        FileCabinetServiceSnapshot MakeSnapshot();

        /// <summary>
        /// Return make snapshot for restore.
        /// </summary>
        /// <param name="fileCabinetServiceSnapshot">Input parametr fileCabinetServiceSnapshot <see cref="FileCabinetServiceSnapshot"/>.</param>
        void Restore(FileCabinetServiceSnapshot fileCabinetServiceSnapshot);

        /// <summary>
        /// Delete record by id.
        /// </summary>
        /// <param name="id">Input parametr id of record <see cref="int"/>.</param>
        void Remove(int id);

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
        void Purge();
    }
}
