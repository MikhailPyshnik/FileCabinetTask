using System;
using System.Collections;
using System.Collections.Generic;

namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Class implement IEnumerable for work with foreach.
    /// </summary>
    public sealed class EnumerableMemoryCollection : IEnumerable<FileCabinetRecord> // Collection
    {
        private readonly List<FileCabinetRecord> records;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumerableMemoryCollection"/> class.
        /// </summary>
        /// <param name="records">Input parametr in constructor (list of positions) <see cref="FileCabinetRecord"/>.</param>
        public EnumerableMemoryCollection(List<FileCabinetRecord> records)
        {
            this.records = records ?? throw new ArgumentNullException(nameof(records));
        }

        /// <summary>
        /// Method return IEnumerator>.
        /// </summary>
        /// <returns>IEnumerable by lastName <see cref="FileCabinetRecord"/>.</returns>
        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            return new RecordEnumerator(this.records);
        }

        /// <summary>
        /// Method return IEnumerator.
        /// </summary>
        /// <returns>Return <see cref="IEnumerator"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator1();
        }

        private IEnumerator<FileCabinetRecord> GetEnumerator1()
        {
            return this.GetEnumerator();
        }
    }
}
