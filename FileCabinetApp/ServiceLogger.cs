using System;
using System.Collections.ObjectModel;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// Work with Logger.
    /// </summary>
    public class ServiceLogger : IFileCabinetService
    {
        private static IFileCabinetService fileCabinetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLogger"/> class.
        /// </summary>
        public ServiceLogger()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLogger"/> class.
        /// </summary>
        /// <param name="fileCabinet">Input parametr in constructor <see cref="IFileCabinetService"/>.</param>
        public ServiceLogger(IFileCabinetService fileCabinet)
        {
            fileCabinetService = fileCabinet;
        }

        /// <summary>
        /// Gets or sets the Validator of the Program.
        /// </summary>
        /// <value>The Validator of the Program.</value>
        public IValidatorOfParemetrs Validator { get; set; }

        /// <summary>
        /// Implementation IFileCabinetService СreateRecod in class ServiceLogger.
        /// </summary>
        /// <param name="fileCabinetRecord">Input parametr record <see cref="FileCabinetRecord"/>.</param>
        /// <returns>Id <see cref="int"/>.</returns>
        public int CreateRecord(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord == null)
            {
                throw new ArgumentNullException(nameof(fileCabinetRecord));
            }

            var result = fileCabinetService.CreateRecord(fileCabinetRecord);

            using (StreamWriter w = File.AppendText("log.txt"))
            {
                Log($"Calling Create() with FirstName = '{fileCabinetRecord.FirstName}', LastName = '{fileCabinetRecord.LastName}', DateOfBirth = '{fileCabinetRecord.DateOfBirth}' ", w);
                Log($"Create() returned '{result}'.", w);
            }

            return result;
        }

        /// <summary>
        /// Implementation IFileCabinetService EditRecord in class ServiceLogger.
        /// </summary>
        /// <param name="fileCabinetRecord">Input parametr record <see cref="FileCabinetRecord"/>.</param>
        public void EditRecord(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord == null)
            {
                throw new ArgumentNullException(nameof(fileCabinetRecord));
            }

            fileCabinetService.EditRecord(fileCabinetRecord);

            using (StreamWriter w = File.AppendText("log.txt"))
            {
                Log($"Calling Edit() returned  - FirstName = '{fileCabinetRecord.FirstName}', LastName = '{fileCabinetRecord.LastName}', DateOfBirth = '{fileCabinetRecord.DateOfBirth}'", w);
            }
        }

        /// <summary>
        /// Implementation IFileCabinetService FindByDateOfBirth in class ServiceLogger.
        /// </summary>
        /// <param name="dateofbirth">Input parametr FirstName <see cref="string"/>.</param>
        /// <returns>Rerords by dateofbirth <see cref="FileCabinetRecord"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateofbirth)
        {
            var result = fileCabinetService.FindByDateOfBirth(dateofbirth);
            using (StreamWriter w = File.AppendText("log.txt"))
            {
                Log($"Calling FindByDateOfBirth() returned  - {result.Count} records", w);
            }

            return result;
        }

        /// <summary>
        /// Implementation IFileCabinetService FindByFirstName in class ServiceLogger.
        /// </summary>
        /// <param name="firstName">Input parametr FirstName <see cref="string"/>.</param>
        /// <returns>Rerords by firstName <see cref="FileCabinetRecord"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            var result = fileCabinetService.FindByFirstName(firstName);
            using (StreamWriter w = File.AppendText("log.txt"))
            {
                Log($"Calling FindByFirstName() returned  - {result.Count} records", w);
            }

            return result;
        }

        /// <summary>
        /// Implementation IFileCabinetService FindByLastName in class ServiceLogger.
        /// </summary>
        /// <param name="lastName">Input parametr FirstName <see cref="string"/>.</param>
        /// <returns>Rerords by lastName <see cref="FileCabinetRecord"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            var result = fileCabinetService.FindByLastName(lastName);
            using (StreamWriter w = File.AppendText("log.txt"))
            {
                Log($"Calling FindByLastName() returned  - {result.Count} records", w);
            }

            return result;
        }

        /// <summary>
        /// Implementation IFileCabinetService GetRecords in class ServiceLogger.
        /// </summary>
        /// <returns>Rerords <see cref="FileCabinetRecord"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            var result = fileCabinetService.GetRecords();
            using (StreamWriter w = File.AppendText("log.txt"))
            {
                Log($"Calling GetRecords() returned  - {result.Count} records", w);
            }

            return result;
        }

        /// <summary>
        /// Implementation IFileCabinetService GetStat in class ServiceLogger.
        /// </summary>
        /// <returns>Count records <see cref="int"/>.</returns>
        public Tuple<int, int> GetStat()
        {
            var result = fileCabinetService.GetStat();
            using (StreamWriter w = File.AppendText("log.txt"))
            {
                Log($"Calling GetStat() returned  - {result.Item1} records", w);
            }

            return result;
        }

        /// <summary>
        /// Implementation IFileCabinetService MakeSnapshot in class ServiceLogger.
        /// </summary>
        /// <returns>Rerords by dateofbirth <see cref="FileCabinetServiceSnapshot"/>.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            var result = fileCabinetService.MakeSnapshot();
            using (StreamWriter w = File.AppendText("log.txt"))
            {
                Log($"Calling MakeSnapshot(). MakeSnapshot {result.Records.Count} records", w);
            }

            return result;
        }

        /// <summary>
        /// Implementation IFileCabinetService Purge in class ServiceLogger.
        /// </summary>
        public void Purge()
        {
            fileCabinetService.Purge();
            using (StreamWriter w = File.AppendText("log.txt"))
            {
                Log($"Calling Purge() returned", w);
            }
        }

        /// <summary>
        /// Implementation IFileCabinetService Remove in class ServiceLogger.
        /// </summary>
        /// <param name="id">Input parametr id of record <see cref="int"/>.</param>
        public void Remove(int id)
        {
            fileCabinetService.Remove(id);
            using (StreamWriter w = File.AppendText("log.txt"))
            {
                Log($"Calling Remove() - revove {id}", w);
            }
        }

        /// <summary>
        /// Implementation IFileCabinetService Restore in class ServiceLogger.
        /// </summary>
        /// <param name="fileCabinetServiceSnapshot">Input parametr fileCabinetServiceSnapshot <see cref="FileCabinetServiceSnapshot"/>.</param>
        public void Restore(FileCabinetServiceSnapshot fileCabinetServiceSnapshot)
        {
            fileCabinetService.Restore(fileCabinetServiceSnapshot);
            using (StreamWriter w = File.AppendText("log.txt"))
            {
                Log($"Calling Restore() - revove", w);
            }
        }

        private static void Log(string logMessage, TextWriter w)
        {
            w.Write($"\r\n {DateTime.Now.ToLongTimeString()} - {logMessage}.");
        }
    }
}
