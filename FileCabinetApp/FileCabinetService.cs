﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Work with records. Save and change record(s).
    /// </summary>
    public abstract class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Сreate new record FileCabinetRecord.
        /// </summary>
        /// <returns>Id <see cref="IRecordValidator"/>.</returns>
        public abstract IRecordValidator CreateValidator();

        /// <summary>
        /// Сreate new record FileCabinetRecord.
        /// </summary>
        /// <param name="fileCabinetRecord">Input parametr record <see cref="FileCabinetRecord"/>.</param>
        /// <returns>Id <see cref="int"/>.</returns>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="fileCabinetRecord"/>, <paramref name="fileCabinetRecord.FirstName"/>,<paramref name="fileCabinetRecord.LastName"/> is null.</exception>
        /// <exception cref="ArgumentException">Throws if <paramref name="fileCabinetRecord.FirstName"/>,<paramref name="fileCabinetRecord.LastName"/>,<paramref name="fileCabinetRecord.DateOfBirth"/>,<paramref name="fileCabinetRecord.Sex"/>,<paramref name="fileCabinetRecord.Height"/>,<paramref name="fileCabinetRecord.Salary"/> is(are) incorrect value.</exception>
        public int CreateRecord(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord == null)
            {
                throw new ArgumentNullException($"{nameof(fileCabinetRecord)} is null!");
            }

            this.CreateValidator().ValidateParametrs(fileCabinetRecord);

            fileCabinetRecord.Id = this.list.Count + 1;
            this.list.Add(fileCabinetRecord);
            this.AddRecordToDictionary(fileCabinetRecord);
            return fileCabinetRecord.Id;
        }

        /// <summary>
        /// Get all record FileCabinetRecord.
        /// </summary>
        /// <returns>Rerords <see cref="FileCabinetRecord"/>.</returns>
        public FileCabinetRecord[] GetRecords()
        {
            FileCabinetRecord[] copyFileCabinetRecord = new FileCabinetRecord[this.list.Count];
            this.list.CopyTo(copyFileCabinetRecord);
            return copyFileCabinetRecord;
        }

        /// <summary>
        /// Get count of record FileCabinetRecord.
        /// </summary>
        /// <returns>Count records <see cref="int"/>.</returns>
        public int GetStat()
        {
            return this.list.Count;
        }

        /// <summary>
        /// Edit record by id.
        /// </summary>
        /// <param name="fileCabinetRecord">Input parametr record <see cref="FileCabinetRecord"/>.</param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="fileCabinetRecord"/>, <paramref name="fileCabinetRecord.FirstName"/>,<paramref name="fileCabinetRecord.LastName"/> is null.</exception>
        /// <exception cref="ArgumentException">Throws if <paramref name="fileCabinetRecord.FirstName"/>,<paramref name="fileCabinetRecord.LastName"/>,<paramref name="fileCabinetRecord.DateOfBirth"/>,<paramref name="fileCabinetRecord.Sex"/>,<paramref name="fileCabinetRecord.Height"/>,<paramref name="fileCabinetRecord.Salary"/> is(are) incorrect value.</exception>
        public void EditRecord(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord == null)
            {
                throw new ArgumentNullException($"{nameof(fileCabinetRecord)} is null!");
            }

            if (fileCabinetRecord.Id > this.GetStat())
            {
                throw new ArgumentException("Input Id is incorrect value.");
            }

            int editId = fileCabinetRecord.Id - 1;
            FileCabinetRecord res = this.list.Find(item1 => item1.Id == fileCabinetRecord.Id);
            string oldFirstName = res.FirstName;
            string oldLastName = res.LastName;
            string oldDateOfBirth = res.DateOfBirth.ToString("yyyy-MMM-dd", new CultureInfo("en-US"));
            FileCabinetRecord item = this.list[editId];
            item.FirstName = fileCabinetRecord.FirstName;
            item.LastName = fileCabinetRecord.LastName;
            item.DateOfBirth = fileCabinetRecord.DateOfBirth;
            item.Sex = fileCabinetRecord.Sex;
            item.Height = fileCabinetRecord.Height;
            item.Salary = fileCabinetRecord.Salary;
            this.ChangeRecordToDictionary(item, oldFirstName, oldLastName, oldDateOfBirth);
        }

        /// <summary>
        /// Return records by first name.
        /// </summary>
        /// <param name="firstName">Input parametr FirstName <see cref="string"/>.</param>
        /// <returns>Rerords by firstName <see cref="FileCabinetRecord"/>.</returns>
        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            List<FileCabinetRecord> result = new List<FileCabinetRecord>();

            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                result = this.firstNameDictionary[firstName];
            }

            return result.ToArray();
        }

        /// <summary>
        /// Return records by last name.
        /// </summary>
        /// <param name="lastName">Input parametr FirstName <see cref="string"/>.</param>
        /// <returns>Rerords by lastName <see cref="FileCabinetRecord"/>.</returns>
        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            List<FileCabinetRecord> result = new List<FileCabinetRecord>();

            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                result = this.lastNameDictionary[lastName];
            }

            return result.ToArray();
        }

        /// <summary>
        /// Return records by date of birth.
        /// </summary>
        /// <param name="dateofbirth">Input parametr FirstName <see cref="string"/>.</param>
        /// <returns>Rerords by dateofbirth <see cref="FileCabinetRecord"/>.</returns>
        public FileCabinetRecord[] FindByDateOfBirth(string dateofbirth)
        {
            List<FileCabinetRecord> result = new List<FileCabinetRecord>();

            if (this.dateOfBirthDictionary.ContainsKey(dateofbirth))
            {
                result = this.dateOfBirthDictionary[dateofbirth];
            }

            return result.ToArray();
        }

        private void AddRecordToDictionary(FileCabinetRecord record)
        {
            this.AddRecordToFirstNameDictionary(record, record.FirstName);
            this.AddRecordToLastNameDictionary(record, record.LastName);
            this.AddRecordToDateOfBirthDictionary(record, record.DateOfBirth);
        }

        private void ChangeRecordToDictionary(FileCabinetRecord item, string oldFirstName,  string oldLastName,  string oldDateOfBirth)
        {
            this.ChangeRecordInFirstNameDictionary(item, oldFirstName, item.FirstName);
            this.ChangeRecordInLastNameDictionary(item, oldLastName, item.LastName);
            this.ChangeRecordInDateOfBirthDictionary(item, oldDateOfBirth, item.DateOfBirth);
        }

        private void AddRecordToFirstNameDictionary(FileCabinetRecord record, string firstName)
        {
            if (this.firstNameDictionary.ContainsKey(firstName.ToLower(new CultureInfo("en-US"))))
            {
                this.firstNameDictionary[firstName].Add(record);
            }
            else
            {
                List<FileCabinetRecord> listFirstName = this.list.FindAll(item => item.FirstName == firstName);
                this.firstNameDictionary.Add(firstName, listFirstName);
            }
        }

        private void AddRecordToLastNameDictionary(FileCabinetRecord record, string lastName)
        {
            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                this.lastNameDictionary[lastName].Add(record);
            }
            else
            {
                List<FileCabinetRecord> listLastName = this.list.FindAll(item => item.LastName == lastName);
                this.lastNameDictionary.Add(lastName, listLastName);
            }
        }

        private void AddRecordToDateOfBirthDictionary(FileCabinetRecord record, DateTime dateOfBirth)
        {
            string tempDateOfBirh = dateOfBirth.ToString("yyyy-MMM-dd", new CultureInfo("en-US"));
            if (this.dateOfBirthDictionary.ContainsKey(tempDateOfBirh))
            {
                this.dateOfBirthDictionary[tempDateOfBirh].Add(record);
            }
            else
            {
                List<FileCabinetRecord> listdateOfBirth = this.list.FindAll(item => item.DateOfBirth == dateOfBirth);
                this.dateOfBirthDictionary.Add(tempDateOfBirh, listdateOfBirth);
            }
        }

        private void ChangeRecordInFirstNameDictionary(FileCabinetRecord item, string oldFirstName, string firstName)
        {
            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                this.firstNameDictionary[oldFirstName].Remove(item);
                this.firstNameDictionary[firstName].Add(item);
            }
            else
            {
                this.firstNameDictionary[oldFirstName].Remove(item);
                List<FileCabinetRecord> listFirstName = this.list.FindAll(item1 => item1.FirstName == firstName);
                this.firstNameDictionary.Add(firstName, listFirstName);
            }
        }

        private void ChangeRecordInLastNameDictionary(FileCabinetRecord item, string oldLastName, string lastName)
        {
            if (this.lastNameDictionary.ContainsKey(lastName))
            {
                this.lastNameDictionary[oldLastName].Remove(item);
                this.lastNameDictionary[lastName].Add(item);
            }
            else
            {
                this.lastNameDictionary[oldLastName].Remove(item);
                List<FileCabinetRecord> listFirstName = this.list.FindAll(item1 => item1.FirstName == lastName);
                this.lastNameDictionary.Add(lastName, listFirstName);
            }
        }

        private void ChangeRecordInDateOfBirthDictionary(FileCabinetRecord item,  string oldDateOfBirth, DateTime dateOfBirth)
        {
            string tempDateOfBirh = dateOfBirth.ToString("yyyy-MMM-dd", new CultureInfo("en-US"));
            if (this.dateOfBirthDictionary.ContainsKey(tempDateOfBirh))
            {
                this.dateOfBirthDictionary[oldDateOfBirth].Remove(item);
                this.dateOfBirthDictionary[tempDateOfBirh].Add(item);
            }
            else
            {
                this.dateOfBirthDictionary[oldDateOfBirth].Remove(item);
                List<FileCabinetRecord> listDateOfBirth = this.list.FindAll(item1 => item.DateOfBirth == dateOfBirth);
                this.dateOfBirthDictionary.Add(tempDateOfBirh, listDateOfBirth);
            }
        }
    }
}
