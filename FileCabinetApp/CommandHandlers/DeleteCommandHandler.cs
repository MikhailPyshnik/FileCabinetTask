using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Delete command.
    /// </summary>
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Input parametr start id.<see cref="IFileCabinetService"/>.</param>
        public DeleteCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        /// <summary>
        /// Override method Handle by CommandHandlerBase in RemoveCommandHandler.
        /// </summary>
        /// <param name="appCommandRequest">Input parametr record <see cref="AppCommandRequest"/>.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest == null)
            {
                throw new ArgumentNullException(nameof(appCommandRequest));
            }

            if (appCommandRequest.Command == "delete")
            {
                var parameters = appCommandRequest.Parameters;
                Delete(parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private static void Delete(string parameters)
        {
            try
            {
                try
                {
                    CultureInfo provider = new CultureInfo("en-US");

                    string[] value = new string[] { "id", "firstname", "lastname", "dateofbirth", "sex", "height", "salary" };

                    Regex regex = new Regex(@"where(.*)=(.*)", RegexOptions.IgnoreCase);

                    MatchCollection matches = regex.Matches(parameters);

                    string nameParametr = matches[0].Groups[1].Value.ToLower(provider);

                    var inputParametr = matches[0].Groups[2].Value.ToLower(provider);

                    var command = GetStringArray(nameParametr);

                    var inputValue = GetStringArray(inputParametr);

                    if (!(command.Length == 1 && inputValue.Length == 1))
                    {
                        throw new ArgumentException("Insert - input not correct value.");
                    }

                    string[] temp = new string[] { command[0], inputValue[0] };

                    var listTemp = service.Delete(temp);

                    if (listTemp.Count == 0)
                    {
                        Console.WriteLine("Record not found.");
                    }
                    else if (listTemp.Count > 1)
                    {
                        StringBuilder idstring = new StringBuilder();
                        foreach (var item in listTemp)
                        {
                            idstring.Append($"#{item},");
                        }

                        Console.WriteLine($"Records {idstring} are deleted.");
                    }
                    else
                    {
                        Console.WriteLine($"Record #{listTemp[0]} is deleted.");
                    }
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"{ex.Message}");
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
