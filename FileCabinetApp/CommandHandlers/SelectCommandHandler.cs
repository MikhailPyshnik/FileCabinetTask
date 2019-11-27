using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Select command.
    /// </summary>
    public class SelectCommandHandler : ServiceCommandHandlerBase
    {
        private Action<IEnumerable<FileCabinetRecord>, string[]> printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Input parametr start id.<see cref="IFileCabinetService"/>.</param>
        /// <param name="inputPrinter">Input parametr start id.<see cref="IRecordPrinter"/>.</param>
        public SelectCommandHandler(IFileCabinetService fileCabinetService, Action<IEnumerable<FileCabinetRecord>, string[]> inputPrinter)
            : base(fileCabinetService)
        {
            this.printer = inputPrinter ?? throw new ArgumentNullException(nameof(inputPrinter));
        }

        /// <summary>
        /// Override method Handle by CommandHandlerBase in FindCommandHandler.
        /// </summary>
        /// <param name="appCommandRequest">Input parametr record <see cref="AppCommandRequest"/>.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
            {
                if (appCommandRequest == null)
                {
                    throw new ArgumentNullException(nameof(appCommandRequest));
                }

                if (appCommandRequest.Command == "select")
                {
                    var parameters = appCommandRequest.Parameters;
                    this.Select(parameters);
                }
                else
                {
                    base.Handle(appCommandRequest);
                }
            }

        private static string[] GetStringArray(string inputString)
        {
            if (inputString == null)
            {
                throw new ArgumentNullException(nameof(inputString));
            }

            string withoutWhiteSpace = inputString.Replace(" ", string.Empty, StringComparison.Ordinal);

            char[] charsToTrim = { '(', ')' };

            string spl = withoutWhiteSpace.Trim(charsToTrim).Replace("'", string.Empty, StringComparison.Ordinal);

            string[] split = spl.Split(new char[] { ',' });

            return split;
        }

        private static string[] GetStringArrayForAndAndOr(string inputString, out string logicalOperator)
        {
            if (inputString == null)
            {
                throw new ArgumentNullException(nameof(inputString));
            }

            if (inputString.Contains("and", StringComparison.Ordinal))
            {
                logicalOperator = "and";
            }
            else
            {
                logicalOperator = "or";
            }

            string vithoutprobel = inputString.Replace(" ", string.Empty, StringComparison.Ordinal);

            char[] charsToTrim = { '(', ')' };

            string spl = vithoutprobel.Trim(charsToTrim).Replace("'", string.Empty, StringComparison.Ordinal);

            string[] split = spl.Split(logicalOperator);

            return split;
        }

        private void Select(string parameters)
        {
            try
            {
                try
                {
                CultureInfo provider = new CultureInfo("en-US");

                string[] value1 = new string[] { "id", "firstname", "lastname", "dateofbirth", "sex", "height", "salary" };

                Regex regex = new Regex(@"(.*)where(.*)", RegexOptions.IgnoreCase);

                MatchCollection matches = regex.Matches(parameters);

                if (matches.Count == 0)
                    {
                    Regex regexList = new Regex(@"(.*)", RegexOptions.IgnoreCase);
                    MatchCollection matchesList = regexList.Matches(parameters);
                    var nameParametrList = matchesList[0].Groups[1].Value.ToLower(provider);
                    var command = GetStringArray(nameParametrList);
                    var selectList = service.GetRecords();
                    this.PrintRecords(selectList, command);
                    }
                    else
                    {
                    string nameParametr = matches[0].Groups[1].Value.ToLower(provider);

                    string inputParametr = matches[0].Groups[2].Value.ToLower(provider);

                    var command = GetStringArray(nameParametr);

                    string logicalOperator;

                    var inputValue = GetStringArrayForAndAndOr(inputParametr, out logicalOperator);

                    var selectRecords = service.SelectByCondition(inputValue, logicalOperator);

                    this.PrintRecords(selectRecords, command);

                    Console.WriteLine("The command Select finished correctly.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw new ArgumentException("Select command incorrect! Select command again.");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }

        private void PrintRecords(IEnumerable<FileCabinetRecord> records, string[] searchparametr)
        {
            if (records == null)
            {
                Console.WriteLine("No records are found!");
            }
            else
            {
                this.printer(records, searchparametr);
            }
        }
    }
}
