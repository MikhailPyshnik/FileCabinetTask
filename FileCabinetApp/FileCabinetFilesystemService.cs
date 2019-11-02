using System;
using System.Collections.ObjectModel;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// Work with db.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private readonly IValidatorOfParemetrs validator;
        private readonly FileStream fileStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="validator">Input parametr in constructor <see cref="IValidatorOfParemetrs"/>.</param>
        /// <param name="fileStream">Input parametr in constructor <see cref="FileStream"/>.</param>
        public FileCabinetFilesystemService(IValidatorOfParemetrs validator, FileStream fileStream)
        {
            this.validator = validator;
            this.fileStream = fileStream;
        }

        /// <summary>
        /// Implementation IFileCabinetService СreateRecod.
        /// </summary>
        /// <param name="fileCabinetRecord">Input parametr record <see cref="FileCabinetRecord"/>.</param>
        /// <returns>Id <see cref="int"/>.</returns>
        public int CreateRecord(FileCabinetRecord fileCabinetRecord)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implementation IFileCabinetService EditRecord.
        /// </summary>
        /// <param name="fileCabinetRecord">Input parametr record <see cref="FileCabinetRecord"/>.</param>
        public void EditRecord(FileCabinetRecord fileCabinetRecord)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implementation IFileCabinetService FindByDateOfBirth.
        /// </summary>
        /// <param name="dateofbirth">Input parametr FirstName <see cref="string"/>.</param>
        /// <returns>Rerords by dateofbirth <see cref="FileCabinetRecord"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateofbirth)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implementation IFileCabinetService FindByFirstName.
        /// </summary>
        /// <param name="firstName">Input parametr FirstName <see cref="string"/>.</param>
        /// <returns>Rerords by firstName <see cref="FileCabinetRecord"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implementation IFileCabinetService FindByLastName.
        /// </summary>
        /// <param name="lastName">Input parametr FirstName <see cref="string"/>.</param>
        /// <returns>Rerords by lastName <see cref="FileCabinetRecord"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implementation IFileCabinetService GetRecords.
        /// </summary>
        /// <returns>Rerords <see cref="FileCabinetRecord"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implementation IFileCabinetService GetStat.
        /// </summary>
        /// <returns>Count records <see cref="int"/>.</returns>
        public int GetStat()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Implementation IFileCabinetService FindByDateOfBirth.
        /// </summary>
        /// <returns>Rerords by dateofbirth <see cref="FileCabinetServiceSnapshot"/>.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
        }
    }
}
