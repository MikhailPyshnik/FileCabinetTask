﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth)
        {
            // TODO: добавьте реализацию метода
            return 0;
        }

        public FileCabinetRecord[] GetRecords()
        {
            // TODO: добавьте реализацию метода
            return System.Array.Empty<FileCabinetRecord>();
        }

        public int GetStat()
        {
            // TODO: добавьте реализацию метода
            return 0;
        }
    }
}
