using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Update command.
    /// </summary>
    public class UpdateCommandHandler : ServiceCommandHandlerBase
    {
            /// <summary>
            /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
            /// </summary>
            /// <param name="fileCabinetService">Input parametr start id.<see cref="IFileCabinetService"/>.</param>
            public UpdateCommandHandler(IFileCabinetService fileCabinetService)
                : base(fileCabinetService)
            {
            }

            /// <summary>
            /// Override method Handle by CommandHandlerBase in EditCommandHandler.
            /// </summary>
            /// <param name="appCommandRequest">Input parametr record <see cref="AppCommandRequest"/>.</param>
            public override void Handle(AppCommandRequest appCommandRequest)
            {
                if (appCommandRequest == null)
                {
                    throw new ArgumentNullException(nameof(appCommandRequest));
                }

                if (appCommandRequest.Command == "update")
                {
                    var parameters = appCommandRequest.Parameters;
                    Update(parameters);
                }
                else
                {
                    base.Handle(appCommandRequest);
                }
            }

            private static void Update(string parameters)
            {
                try
                {
                    try
                    {
                        CultureInfo provider = new CultureInfo("en-US");

                        string[] value = new string[] { "id", "firstname", "lastname", "dateofbirth", "sex", "height", "salary" };

                        Regex regex = new Regex(@"set(.*)where(.*)", RegexOptions.IgnoreCase);

                        MatchCollection matches = regex.Matches(parameters);

                        string nameParametr = matches[0].Groups[1].Value.ToLower(provider);

                        string inputParametr = matches[0].Groups[2].Value.ToLower(provider);

                        var command = GetStringArray(nameParametr);

                        var inputValue = GetStringArrayForAnd(inputParametr);

                        service.Update(command, inputValue);

                        Console.WriteLine("The command Update finished correctly.");
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw new ArgumentException("Input value in Update is incorrect! Select command again.");
                    }
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
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

            private static string[] GetStringArrayForAnd(string inputString)
            {
                if (inputString == null)
                {
                    throw new ArgumentNullException(nameof(inputString));
                }

                string andReplace = "and";

                string vithoutprobel = inputString.Replace(" ", string.Empty, StringComparison.Ordinal);

                char[] charsToTrim = { '(', ')' };

                string spl = vithoutprobel.Trim(charsToTrim).Replace("'", string.Empty, StringComparison.Ordinal);

                string[] split = spl.Split(andReplace);

                return split;
            }
    }
}
