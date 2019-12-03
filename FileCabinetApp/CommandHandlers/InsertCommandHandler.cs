using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Insert command.
    /// </summary>
    public class InsertCommandHandler : ServiceCommandHandlerBase
    {
        private static Action<string> action;

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Input parametr start id.<see cref="IFileCabinetService"/>.</param>
        /// <param name="printMethod">Input delegate Action for print messages.<see cref="Action"/>.</param>
        public InsertCommandHandler(IFileCabinetService fileCabinetService, Action<string> printMethod)
             : base(fileCabinetService)
        {
            action = printMethod ?? throw new ArgumentNullException(nameof(printMethod));
        }

        /// <summary>
        /// Override method Handle by CommandHandlerBase in CreateCommandHandler.
        /// </summary>
        /// <param name="appCommandRequest">Input parametr record <see cref="AppCommandRequest"/>.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest is null)
            {
                throw new ArgumentNullException(nameof(appCommandRequest));
            }

            if (appCommandRequest.Command == "insert")
            {
                var parameters = appCommandRequest.Parameters;
                Insert(parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private static void Insert(string parameters)
        {
            try
            {
                try
                {
                    CultureInfo provider = new CultureInfo("en-US");

                    string[] value = new string[] { "id", "firstname", "lastname", "dateofbirth", "sex", "height", "salary" };

                    Regex regex = new Regex(@"(.*)values(.*)", RegexOptions.IgnoreCase);

                    MatchCollection matches = regex.Matches(parameters);

                    string nameParametr = matches[0].Groups[1].Value.ToLower(provider);

                    var inputParametr = matches[0].Groups[2].Value.ToLower(provider);

                    var command = GetStringArray(nameParametr);

                    var inputValue = GetStringArray(inputParametr);

                    if (!(command.Length < 8 && inputValue.Length < 8))
                    {
                        throw new ArgumentException("Insert - input not correct value.");
                    }

                    FileCabinetRecord fileCabinetRecord = new FileCabinetRecord();

                    for (int i = 0; i < command.Length; i++)
                    {
                        switch (command[i])
                        {
                            case "id":
                                int parsedId = 0;
                                var boolId = int.TryParse(inputValue[i], out parsedId);
                                fileCabinetRecord.Id = parsedId;
                                if (!boolId)
                                {
                                    throw new ArgumentException("Not parsed id.Not correct value!Exit insert command.");
                                }

                                break;
                            case "firstname":
                                fileCabinetRecord.FirstName = inputValue[i];
                                break;
                            case "lastname":
                                fileCabinetRecord.LastName = inputValue[i];
                                break;
                            case "dateofbirth":
                                DateTime parsedDate = DateTime.MinValue;
                                var boolDate = DateTime.TryParse(inputValue[i], out parsedDate);
                                fileCabinetRecord.DateOfBirth = parsedDate;
                                if (!boolDate)
                                {
                                    throw new ArgumentException("Not parsed date of birth.Not correct value!Exit insert command.");
                                }

                                break;
                            case "sex":
                                char parsedGender = char.MaxValue;
                                var boolGender = char.TryParse(inputValue[i], out parsedGender);
                                fileCabinetRecord.Sex = parsedGender;
                                if (!boolGender)
                                {
                                    throw new ArgumentException("Not parsed gender.Not correct value!Exit insert command.");
                                }

                                break;
                            case "height":
                                short parsedHeigth = 0;
                                var boolHeigth = short.TryParse(inputValue[i], out parsedHeigth);
                                fileCabinetRecord.Height = parsedHeigth;
                                if (!boolHeigth)
                                {
                                    throw new ArgumentException("Not parsed height.Not correct value!Exit insert command.");
                                }

                                break;
                            case "salary":
                                decimal parsedSalary = 0;
                                var boolSalary = decimal.TryParse(inputValue[i], out parsedSalary);
                                fileCabinetRecord.Salary = parsedSalary;
                                if (!boolSalary)
                                {
                                    throw new ArgumentException("Not parsed salary.Not correct value!Exit insert command.");
                                }

                                break;
                            default:
                                throw new ArgumentException("Not correct value!!!!");
                        }
                    }

                    var result = service.Insert(fileCabinetRecord);
                    if (result == 0)
                    {
                        action($"Not add record.This id contains!");
                    }
                    else
                    {
                        action($"Add record by insert with id - {fileCabinetRecord.Id}.");
                    }
                }
                catch (ArgumentException ex)
                {
                    action($"{ex.Message}");
                }
            }
            catch (ArgumentException ex)
            {
                action($"{ex.Message}");
            }
        }

        private static string[] GetStringArray(string inputString)
        {
            if (inputString == null)
            {
                throw new ArgumentNullException(nameof(inputString));
            }

            string vithoutprobel = inputString.Replace(" ", string.Empty, StringComparison.Ordinal);

            char[] charsToTrim = { '(', ')' };

            string spl = vithoutprobel.Trim(charsToTrim).Replace("'", string.Empty, StringComparison.Ordinal);

            string[] split = spl.Split(new char[] { ',' });

            return split;
        }
    }
}
