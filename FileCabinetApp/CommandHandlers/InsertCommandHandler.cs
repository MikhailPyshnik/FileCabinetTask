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
                                fileCabinetRecord.Id = int.Parse(inputValue[i], provider);
                                break;
                            case "firstname":
                                fileCabinetRecord.FirstName = inputValue[i];
                                break;
                            case "lastname":
                                fileCabinetRecord.LastName = inputValue[i];
                                break;
                            case "dateofbirth":
                                fileCabinetRecord.DateOfBirth = DateTime.Parse(inputValue[i], provider);
                                break;
                            case "sex":
                                fileCabinetRecord.Sex = char.Parse(inputValue[i]);
                                break;
                            case "height":
                                fileCabinetRecord.Height = short.Parse(inputValue[i], provider);
                                break;
                            case "salary":
                                fileCabinetRecord.Salary = decimal.Parse(inputValue[i], provider);
                                break;
                            default:
                                throw new ArgumentException("Not correct valuee!!!!");
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
