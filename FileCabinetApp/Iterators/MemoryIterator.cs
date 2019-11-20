using System;
using System.Collections.Generic;

namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Class implement IRecorditerator for work with FileCabinetMemotyService.
    /// </summary>
    public class MemoryIterator : IRecorditerator
    {
        private readonly List<FileCabinetRecord> records;
        private int position;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryIterator"/> class.
        /// </summary>
        /// <param name="records">Input parametr in constructor <see cref="FileCabinetRecord"/>.</param>
        public MemoryIterator(List<FileCabinetRecord> records)
        {
            this.records = records ?? throw new ArgumentNullException(nameof(records));
            this.position = -1;
        }

        /// <summary>
        /// Return records.
        /// </summary>
        /// <returns>Rerord by condition<see cref="FileCabinetRecord"/>.</returns>
        public FileCabinetRecord GetNext()
        {
            return this.records[this.position];
        }

        /// <summary>
        /// Get next record.
        /// </summary>
        /// <returns>Rerord if is exist <see cref="bool"/>.</returns>
        public bool HasMore()
        {
            if (++this.position == this.records.Count)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
