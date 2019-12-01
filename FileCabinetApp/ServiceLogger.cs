using System;
using System.Collections.Generic;
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
            fileCabinetService = fileCabinet ?? throw new ArgumentNullException(nameof(fileCabinet));
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
            if (fileCabinetRecord is null)
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
        /// Implementation IFileCabinetService СreateRecod.
        /// </summary>
        /// <param name="fileCabinetRecord">Input parametr record <see cref="FileCabinetRecord"/>.</param>
        /// <returns>Id<see cref="int"/>.</returns>
        public int Insert(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord is null)
            {
                throw new ArgumentNullException(nameof(fileCabinetRecord));
            }

            var result = fileCabinetService.Insert(fileCabinetRecord);

            using (StreamWriter w = File.AppendText("log.txt"))
            {
                Log($"Calling Insert() with  Id = '{fileCabinetRecord.Id}', FirstName = '{fileCabinetRecord.FirstName}', LastName = '{fileCabinetRecord.LastName}', DateOfBirth = '{fileCabinetRecord.DateOfBirth}' ", w);
            }

            return result;
        }

        /// <summary>
        /// Implementation IFileCabinetService UpdateRecord.
        /// </summary>
        /// <param name="inputValueArray">Input value array <see cref="string"/>.</param>
        /// <param name="inputParamentArray">Input parametr array <see cref="string"/>.</param>
        public void Update(string[] inputValueArray, string[] inputParamentArray)
        {
            if (inputValueArray is null)
            {
                throw new ArgumentNullException($"{nameof(inputValueArray)} is null!");
            }

            if (inputParamentArray is null)
            {
                throw new ArgumentNullException($"{nameof(inputParamentArray)} is null!");
            }

            fileCabinetService.Update(inputValueArray, inputParamentArray);
            using (StreamWriter w = File.AppendText("log.txt"))
            {
                Log("Calling Update() records", w);
            }
        }

        /// <summary>
        /// Return records by select in ServiceLogger.
        /// </summary>
        /// <param name="inputParamentArray">Input parametr array <see cref="string"/>.</param>
        /// <param name="logicalOperator">Input parametr for conditional <see cref="string"/>.</param>
        /// <returns>IEnumerable by firstName <see cref="FileCabinetRecord"/>.</returns>
        public IEnumerable<FileCabinetRecord> SelectByCondition(string[] inputParamentArray, string logicalOperator)
        {
            var result = fileCabinetService.SelectByCondition(inputParamentArray, logicalOperator);
            using (StreamWriter w = File.AppendText("log.txt"))
            {
                Log($"Calling SelectByCondition()", w);
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
        /// <returns>Count records for delete <see cref="Tuple"/>.</returns>
        public Tuple<int, int> Purge()
        {
            var result = fileCabinetService.Purge();
            using (StreamWriter w = File.AppendText("log.txt"))
            {
                Log($"Calling Purge() returned - deleted {result.Item1} from {result.Item2} record(s).", w);
            }

            return result;
        }

        /// <summary>
        /// Implementation IFileCabinetService Restore.
        /// </summary>
        /// <param name="inputValueArray">Input parametr value <see cref="string"/>.</param>
        /// <returns>ReadOnlyCollection deleted id <see cref="int"/>.</returns>
        public ReadOnlyCollection<int> Delete(string[] inputValueArray)
        {
            var result = fileCabinetService.Delete(inputValueArray);
            using (StreamWriter w = File.AppendText("log.txt"))
            {
                Log($"Calling Delete() - deleted {result.Count} record(s).", w);
            }

            return result;
        }

        /// <summary>
        /// Implementation IFileCabinetService Restore in class ServiceLogger.
        /// </summary>
        /// <param name="fileCabinetServiceSnapshot">Input parametr fileCabinetServiceSnapshot <see cref="FileCabinetServiceSnapshot"/>.</param>
        /// <returns>Counts add import record(s) <see cref="int"/>.</returns>
        public int Restore(FileCabinetServiceSnapshot fileCabinetServiceSnapshot)
        {
            var result = fileCabinetService.Restore(fileCabinetServiceSnapshot);
            using (StreamWriter w = File.AppendText("log.txt"))
            {
                Log($"Calling Restore() - revove", w);
            }

            return result;
        }

        private static void Log(string logMessage, TextWriter w)
        {
            w.Write($"{Environment.NewLine} {DateTime.Now.ToLongTimeString()} - {logMessage}.");
        }
    }
}
