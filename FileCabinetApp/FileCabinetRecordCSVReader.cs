using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace FileCabinetApp
{
    /// <summary>
    /// Read from CSV file.
    /// </summary>
    public class FileCabinetRecordCSVReader
    {
        private readonly StreamReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordCSVReader"/> class.
        /// </summary>
        /// <param name="reader">Input parametr in constructor <see cref="StreamReader"/>.</param>
        public FileCabinetRecordCSVReader(StreamReader reader)
        {
            this.reader = reader;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <returns>Import rerords by IList.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            List<FileCabinetRecord> listImport = new List<FileCabinetRecord>();

            List<string> listReadFile = new List<string>();
            string line;
            while ((line = this.reader.ReadLine()) != null)
            {
                listReadFile.Add(line);
            }

            string[] listReadFileArray = listReadFile.ToArray();

            for (int i = 1; i < listReadFileArray.Length; i++)
            {
                listImport.Add(StreamRiderFromCSV(listReadFileArray[i]));
            }

            return listImport;
        }

        private static FileCabinetRecord StreamRiderFromCSV(string csvline)
        {
            CultureInfo provider = new CultureInfo("en-US");
            string[] values = csvline.Split(',');
            FileCabinetRecord fileCabinetRecord = new FileCabinetRecord();
            fileCabinetRecord.Id = Convert.ToInt32(values[0], provider);
            fileCabinetRecord.FirstName = values[1];
            fileCabinetRecord.LastName = values[2];
            fileCabinetRecord.DateOfBirth = Convert.ToDateTime(values[3], provider);
            fileCabinetRecord.Sex = Convert.ToChar(values[4], provider);
            fileCabinetRecord.Height = Convert.ToInt16(values[5], provider);
            fileCabinetRecord.Salary = Convert.ToDecimal(values[6], provider);

            return fileCabinetRecord;
        }
    }
}
