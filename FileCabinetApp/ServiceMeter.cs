using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FileCabinetApp
{
     /// <summary>
     /// Work with StopWatch.
     /// </summary>
    public class ServiceMeter : IFileCabinetService
    {
        private static IFileCabinetService fileCabinetService;
        private static Action<string> action;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMeter"/> class.
        /// </summary>
        public ServiceMeter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMeter"/> class.
        /// </summary>
        /// <param name="fileCabinet">Input parametr in constructor <see cref="IFileCabinetService"/>.</param>
        /// <param name="printMethod">Input delegate Action for print messages.<see cref="Action"/>.</param>
        public ServiceMeter(IFileCabinetService fileCabinet, Action<string> printMethod)
        {
            fileCabinetService = fileCabinet ?? throw new ArgumentNullException(nameof(fileCabinet));
            action = printMethod ?? throw new ArgumentNullException(nameof(printMethod));
        }

        /// <summary>
        /// Gets or sets the Validator of the Program.
        /// </summary>
        /// <value>The Validator of the Program.</value>
        public IValidatorOfParemetrs Validator { get; set; }

        /// <summary>
        /// Implementation IFileCabinetService СreateRecod in class StopWatch.
        /// </summary>
        /// <param name="fileCabinetRecord">Input parametr record <see cref="FileCabinetRecord"/>.</param>
        /// <returns>Id <see cref="int"/>.</returns>
        public int CreateRecord(FileCabinetRecord fileCabinetRecord)
        {
            long ticksThisTime = 0;
            var sw = Stopwatch.StartNew();
            var result = fileCabinetService.CreateRecord(fileCabinetRecord);
            sw.Stop();
            ticksThisTime = sw.ElapsedTicks;
            action($"CreateRecord method execution duration is {ticksThisTime} ticks.");
            return result;
        }

        /// <summary>
        /// Implementation IFileCabinetService UpdateRecord.
        /// </summary>
        /// <param name="inputValueArray">Input value array <see cref="string"/>.</param>
        /// <param name="inputParamentArray">Input parametr array <see cref="string"/>.</param>
        public void Update(string[] inputValueArray, string[] inputParamentArray)
        {
            if (inputValueArray == null)
            {
                throw new ArgumentNullException($"{nameof(inputValueArray)} is null!");
            }

            if (inputParamentArray == null)
            {
                throw new ArgumentNullException($"{nameof(inputParamentArray)} is null!");
            }

            long ticksThisTime = 0;
            var sw = Stopwatch.StartNew();
            fileCabinetService.Update(inputValueArray, inputParamentArray);
            sw.Stop();
            ticksThisTime = sw.ElapsedTicks;
            action($"UpdateRecord method execution duration is {ticksThisTime} ticks.");
        }

        /// <summary>
        /// Return records by select in ServiceMeter.
        /// </summary>
        /// <param name="inputParamentArray">Input parametr array <see cref="string"/>.</param>
        /// <param name="logicalOperator">Input parametr for conditional <see cref="string"/>.</param>
        /// <returns>IEnumerable by firstName <see cref="FileCabinetRecord"/>.</returns>
        public IEnumerable<FileCabinetRecord> SelectByCondition(string[] inputParamentArray, string logicalOperator)
        {
            long ticksThisTime = 0;
            var sw = Stopwatch.StartNew();
            var result = fileCabinetService.SelectByCondition(inputParamentArray, logicalOperator);
            sw.Stop();
            ticksThisTime = sw.ElapsedTicks;
            action($"SelectByCondition method execution duration is {ticksThisTime} ticks.");
            return result;
        }

        /// <summary>
        /// Implementation IFileCabinetService GetRecords in class StopWatch.
        /// </summary>
        /// <returns>Rerords <see cref="FileCabinetRecord"/>.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            long ticksThisTime = 0;
            var sw = Stopwatch.StartNew();
            var result = fileCabinetService.GetRecords();
            sw.Stop();
            ticksThisTime = sw.ElapsedTicks;
            action($"GetRecords method execution duration is {ticksThisTime} ticks.");
            return result;
        }

        /// <summary>
        /// Implementation IFileCabinetService GetStat in class StopWatch.
        /// </summary>
        /// <returns>Count records <see cref="int"/>.</returns>
        public Tuple<int, int> GetStat()
        {
            long ticksThisTime = 0;
            var sw = Stopwatch.StartNew();
            var result = fileCabinetService.GetStat();
            sw.Stop();
            ticksThisTime = sw.ElapsedTicks;
            action($"GetStat method execution duration is {ticksThisTime} ticks.");
            return result;
        }

        /// <summary>
        /// Implementation IFileCabinetService СreateRecod.
        /// </summary>
        /// <param name="fileCabinetRecord">Input parametr record <see cref="FileCabinetRecord"/>.</param>
        public void Insert(FileCabinetRecord fileCabinetRecord)
        {
            long ticksThisTime = 0;
            var sw = Stopwatch.StartNew();
            fileCabinetService.Insert(fileCabinetRecord);
            sw.Stop();
            ticksThisTime = sw.ElapsedTicks;
            action($"Insert method execution duration is {ticksThisTime} ticks.");
        }

        /// <summary>
        /// Implementation IFileCabinetService MakeSnapshot in class StopWatch.
        /// </summary>
        /// <returns>Rerords by dateofbirth <see cref="FileCabinetServiceSnapshot"/>.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            long ticksThisTime = 0;
            var sw = Stopwatch.StartNew();
            var result = fileCabinetService.MakeSnapshot();
            sw.Stop();
            ticksThisTime = sw.ElapsedTicks;
            action($"MakeSnapshot method execution duration is {ticksThisTime} ticks.");
            return result;
        }

        /// <summary>
        /// Implementation IFileCabinetService Purge in class StopWatch.
        /// </summary>
        /// <returns>Count records for delete <see cref="Tuple"/>.</returns>
        public Tuple<int, int> Purge()
        {
            long ticksThisTime = 0;
            var sw = Stopwatch.StartNew();
            var result = fileCabinetService.Purge();
            sw.Stop();
            ticksThisTime = sw.ElapsedTicks;
            action($"Purge method execution duration is {ticksThisTime} ticks.");
            return result;
        }

        /// <summary>
        /// Implementation IFileCabinetService Restore.
        /// </summary>
        /// <param name="inputValueArray">Input parametr value <see cref="string"/>.</param>
        /// <returns>ReadOnlyCollection deleted id <see cref="int"/>.</returns>
        public ReadOnlyCollection<int> Delete(string[] inputValueArray)
        {
            long ticksThisTime = 0;
            var sw = Stopwatch.StartNew();
            var result = fileCabinetService.Delete(inputValueArray);
            sw.Stop();
            ticksThisTime = sw.ElapsedTicks;
            action($"Delete method execution duration is {ticksThisTime} ticks.");
            return result;
        }

        /// <summary>
        /// Implementation IFileCabinetService Restore in class StopWatch.
        /// </summary>
        /// <param name="fileCabinetServiceSnapshot">Input parametr fileCabinetServiceSnapshot <see cref="FileCabinetServiceSnapshot"/>.</param>
        /// <returns>Counts add import record(s) <see cref="int"/>.</returns>
        public int Restore(FileCabinetServiceSnapshot fileCabinetServiceSnapshot)
        {
            long ticksThisTime = 0;
            var sw = Stopwatch.StartNew();
            var result = fileCabinetService.Restore(fileCabinetServiceSnapshot);
            sw.Stop();
            ticksThisTime = sw.ElapsedTicks;
            action($"Restore method execution duration is {ticksThisTime} ticks.");
            return result;
        }
    }
}
