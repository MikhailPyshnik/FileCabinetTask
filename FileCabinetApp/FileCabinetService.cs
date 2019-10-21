using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, char sex, short height,  decimal salary)
        {
            this.ValidateExtention(firstName, lastName, dateOfBirth, sex, height, salary);

            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Sex = sex,
                Height = height,
                Salary = salary,
            };
            this.list.Add(record);

            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                this.firstNameDictionary[firstName].Add(record);
            }
            else
            {
                List<FileCabinetRecord> listFirstName = this.list.FindAll(item => item.FirstName == firstName);
                this.firstNameDictionary.Add(firstName, listFirstName);
            }

            return record.Id;
        }

        public FileCabinetRecord[] GetRecords()
        {
            FileCabinetRecord[] copyFileCabinetRecord = new FileCabinetRecord[this.list.Count];
            this.list.CopyTo(copyFileCabinetRecord);
            return copyFileCabinetRecord;
        }

        public int GetStat()
        {
            return this.list.Count;
        }

        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, char sex, short height, decimal salary)
        {
            if (id > this.GetStat())
            {
                throw new ArgumentException("Input Id is incorrect value.");
            }

            int editId = id - 1;
            this.ValidateExtention(firstName, lastName, dateOfBirth, sex, height, salary);
            FileCabinetRecord res = this.list.Find(item1 => item1.Id == id);
            string oldFirstName = res.FirstName;
            FileCabinetRecord item = this.list[editId];
            item.FirstName = firstName;
            item.LastName = lastName;
            item.DateOfBirth = dateOfBirth;
            item.Sex = sex;
            item.Height = height;
            item.Salary = salary;

            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                this.firstNameDictionary[oldFirstName].Remove(item);
                this.firstNameDictionary[firstName].Add(item);
            }
            else
            {
                this.firstNameDictionary[oldFirstName].Remove(item);
                List<FileCabinetRecord> listFirstName = this.list.FindAll(item1 => item1.FirstName == firstName);
                this.firstNameDictionary.Add(firstName, listFirstName);
            }
        }

        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            List<FileCabinetRecord> result = new List<FileCabinetRecord>();

            if (this.firstNameDictionary.ContainsKey(firstName))
            {
                result = this.firstNameDictionary[firstName];
            }

            return result.ToArray();
        }

        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            CultureInfo provider = new CultureInfo("en-US");
            List<FileCabinetRecord> result = new List<FileCabinetRecord>();

            foreach (var record in this.list)
            {
                if (record.LastName.ToLower(provider) == lastName)
                {
                    result.Add(record);
                }
            }

            return result.ToArray();
        }

        public FileCabinetRecord[] FindByDateOfBirth(string dateofbirth)
        {
            CultureInfo provider = new CultureInfo("en-US");
            List<FileCabinetRecord> result = new List<FileCabinetRecord>();

            foreach (var record in this.list)
            {
                if (record.DateOfBirth.ToString("yyyy-MMM-dd", provider).ToLower(provider) == dateofbirth)
                {
                    result.Add(record);
                }
            }

            return result.ToArray();
        }

        private void ValidateExtention(string firstName, string lastName, DateTime dateOfBirth, char sex, short height, decimal salary)
        {
            if (firstName == null)
            {
                throw new ArgumentNullException($"First name {nameof(firstName)} is null.");
            }

            if (firstName.Length < 2 || firstName.Length > 60)
            {
                throw new ArgumentException("First name is incorrect value.");
            }

            if (this.WhiteSpace(firstName))
            {
                throw new ArgumentException("First name consists of spaces.");
            }

            if (lastName == null)
            {
                throw new ArgumentNullException($"Last name {nameof(lastName)} is null.");
            }

            if (lastName.Length < 2 || lastName.Length > 60)
            {
                throw new ArgumentException("Last name is incorrect value.");
            }

            if (this.WhiteSpace(lastName))
            {
                throw new ArgumentException("Last name consists of spaces.");
            }

            DateTime date1 = new DateTime(1950, 1, 01);

            if (date1 > dateOfBirth || dateOfBirth > DateTime.Now)
            {
                throw new ArgumentException("Date Of Birth is incorrect value.");
            }

            if (!'F'.Equals(sex) & !'f'.Equals(sex) & !'M'.Equals(sex) & !'m'.Equals(sex))
            {
                throw new ArgumentException("Sex is incorrect value.");
            }

            if (height < 60 || height > 230)
            {
                throw new ArgumentException("Height is incorrect value.");
            }

            if (salary < 500 || height > 10000)
            {
                throw new ArgumentException("Salary is incorrect value.");
            }
        }

        private bool WhiteSpace(string value)
        {
            bool result = true;
            for (int i = 0; i < value.Length; i++)
            {
                if (!char.IsWhiteSpace(value[i]))
                {
                    result = false;
                    return result;
                }
            }

            return result;
        }
    }
}
