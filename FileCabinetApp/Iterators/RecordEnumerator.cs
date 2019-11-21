using System;
using System.Collections;
using System.Collections.Generic;

namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Class implement IEnumerator for work with foreach.
    /// </summary>
    public sealed class RecordEnumerator : IEnumerator<FileCabinetRecord>
    {
        private readonly List<FileCabinetRecord> records;
        private int position = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordEnumerator"/> class.
        /// </summary>
        /// <param name="records">Input parametr in constructor (list of records) <see cref="FileCabinetRecord"/>.</param>
        public RecordEnumerator(List<FileCabinetRecord> records)
        {
            this.records = records ?? throw new ArgumentNullException(nameof(records));
        }

        /// <summary>
        /// Gets the record.
        /// </summary>
        /// <value>The record.</value>
        public FileCabinetRecord Current
        {
            get
            {
                if (this.position == -1 || this.position >= this.records.Count)
                {
                    throw new InvalidOperationException();
                }

                return this.records[this.position];
            }
        }

        /// <summary>
        /// Gets the record to object.
        /// </summary>
        /// <value>The object.</value>
        object IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }

        /// <summary>
        /// Method Dispose.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Get next record.
        /// </summary>
        /// <returns>Rerord if is exist <see cref="bool"/>.</returns>
        public bool MoveNext()
        {
            if (this.position < this.records.Count - 1)
            {
                this.position++;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Reset.
        /// </summary>
        public void Reset()
        {
            this.position = -1;
        }
    }
}