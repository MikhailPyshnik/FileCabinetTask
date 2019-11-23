using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using FileCabinetApp.Iterators;

namespace FileCabinetApp
{
     /// <summary>
     /// Work with StopWatch.
     /// </summary>
    public class ServiceMeter : IFileCabinetService
    {
        private static IFileCabinetService fileCabinetService;

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
        public ServiceMeter(IFileCabinetService fileCabinet)
        {
            fileCabinetService = fileCabinet;
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
            Console.WriteLine($"CreateRecord method execution duration is {ticksThisTime} ticks.");
            return result;
        }

        /// <summary>
        /// Implementation IFileCabinetService EditRecord in class StopWatch.
        /// </summary>
        /// <param name="fileCabinetRecord">Input parametr record <see cref="FileCabinetRecord"/>.</param>
        public void EditRecord(FileCabinetRecord fileCabinetRecord)
        {
            long ticksThisTime = 0;
            var sw = Stopwatch.StartNew();
            fileCabinetService.EditRecord(fileCabinetRecord);
            sw.Stop();
            ticksThisTime = sw.ElapsedTicks;
            Console.WriteLine($"EditRecord method execution duration is {ticksThisTime} ticks.");
        }

        /// <summary>
        /// Implementation IFileCabinetService FindByDateOfBirth in class StopWatch.
        /// </summary>
        /// <param name="dateofbirth">Input parametr FirstName <see cref="string"/>.</param>
        /// <returns>IEnumerable by dateofbirth <see cref="FileCabinetRecord"/>.</returns>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateofbirth)
        {
            long ticksThisTime = 0;
            var sw = Stopwatch.StartNew();
            var result = fileCabinetService.FindByDateOfBirth(dateofbirth);
            sw.Stop();
            ticksThisTime = sw.ElapsedTicks;
            Console.WriteLine($"FindByDateOfBirth method execution duration is {ticksThisTime} ticks.");
            return result;
        }

        /// <summary>
        /// Implementation IFileCabinetService FindByFirstName in class StopWatch.
        /// </summary>
        /// <param name="firstName">Input parametr FirstName <see cref="string"/>.</param>
        /// <returns>IEnumerable by firstName <see cref="FileCabinetRecord"/>.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            long ticksThisTime = 0;
            var sw = Stopwatch.StartNew();
            var result = fileCabinetService.FindByFirstName(firstName);
            sw.Stop();
            ticksThisTime = sw.ElapsedTicks;
            Console.WriteLine($"FindByFirstName method execution duration is {ticksThisTime} ticks.");
            return result;
        }

        /// <summary>
        /// Implementation IFileCabinetService FindByLastName in class StopWatch.
        /// </summary>
        /// <param name="lastName">Input parametr FirstName <see cref="string"/>.</param>
        /// <returns>IEnumerable by lastName <see cref="FileCabinetRecord"/>.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            long ticksThisTime = 0;
            var sw = Stopwatch.StartNew();
            var result = fileCabinetService.FindByLastName(lastName);
            sw.Stop();
            ticksThisTime = sw.ElapsedTicks;
            Console.WriteLine($"FindByLastName method execution duration is {ticksThisTime} ticks.");
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
            Console.WriteLine($"GetRecords method execution duration is {ticksThisTime} ticks.");
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
            Console.WriteLine($"GetStat method execution duration is {ticksThisTime} ticks.");
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
            Console.WriteLine($"Insert method execution duration is {ticksThisTime} ticks.");
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
            Console.WriteLine($"MakeSnapshot method execution duration is {ticksThisTime} ticks.");
            return result;
        }

        /// <summary>
        /// Implementation IFileCabinetService Purge in class StopWatch.
        /// </summary>
        public void Purge()
        {
            long ticksThisTime = 0;
            var sw = Stopwatch.StartNew();
            fileCabinetService.Purge();
            sw.Stop();
            ticksThisTime = sw.ElapsedTicks;
            Console.WriteLine($"Purge method execution duration is {ticksThisTime} ticks.");
        }

        /// <summary>
        /// Implementation IFileCabinetService Remove in class StopWatch.
        /// </summary>
        /// <param name="id">Input parametr id of record <see cref="int"/>.</param>
        public void Remove(int id)
        {
            long ticksThisTime = 0;
            var sw = Stopwatch.StartNew();
            fileCabinetService.Remove(id);
            sw.Stop();
            ticksThisTime = sw.ElapsedTicks;
            Console.WriteLine($"Remove method execution duration is {ticksThisTime} ticks.");
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
            Console.WriteLine($"Delete method execution duration is {ticksThisTime} ticks.");
            return result;
        }

        /// <summary>
        /// Implementation IFileCabinetService Restore in class StopWatch.
        /// </summary>
        /// <param name="fileCabinetServiceSnapshot">Input parametr fileCabinetServiceSnapshot <see cref="FileCabinetServiceSnapshot"/>.</param>
        public void Restore(FileCabinetServiceSnapshot fileCabinetServiceSnapshot)
        {
            long ticksThisTime = 0;
            var sw = Stopwatch.StartNew();
            fileCabinetService.Restore(fileCabinetServiceSnapshot);
            sw.Stop();
            ticksThisTime = sw.ElapsedTicks;
            Console.WriteLine($"Restore method execution duration is {ticksThisTime} ticks.");
        }
    }
}
