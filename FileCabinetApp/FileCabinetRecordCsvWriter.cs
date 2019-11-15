using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Write to CSV file.
    /// </summary>
    public class FileCabinetRecordCsvWriter
    {
        private readonly StreamWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
        /// </summary>
        /// <param name="writer">Input parametr in constructor <see cref="StreamWriter"/>.</param>
        public FileCabinetRecordCsvWriter(StreamWriter writer)
        {
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        /// <summary>
        /// Write records to csv file.
        /// </summary>
        /// <param name="records">Input parametr array of record <see cref="FileCabinetRecord"/>.</param>
        public void Write(FileCabinetRecord[] records)
        {
            if (records == null)
            {
                // Console.WriteLine("Records is null!");
                // throw new ArgumentNullException("Records is null!");
                throw new ArgumentNullException($"{records} is null.");
            }

            string text = null;
            try
            {
                using (this.writer)
                {
                    this.writer.WriteLine("Id, First Name, Last Name, Date of Birth, Sex, Height, Salary.");
                    foreach (var record in records)
                    {
                        text = FileCabinetRecordToString(record);
                        this.writer.WriteLine(text);
                    }
                }
            }
            catch (Exception)
            {
                throw new ApplicationException("Error");
            }
        }

        private static string FileCabinetRecordToString(FileCabinetRecord record)
        {
            var sb = new StringBuilder();
            sb.Append(record.Id + "," + record.FirstName + "," + record.LastName + "," + record.DateOfBirth.ToString("yyyy-MMM-dd", new CultureInfo("en-US")) + "," + record.Sex + "," + record.Height + "," + record.Salary);
            return sb.ToString();
        }
    }
}