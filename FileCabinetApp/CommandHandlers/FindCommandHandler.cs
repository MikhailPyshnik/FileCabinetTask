using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Find command.
    /// </summary>
    public class FindCommandHandler : ServiceCommandHandlerBase
    {
        private Action<IEnumerable<FileCabinetRecord>> printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Input parametr start id.<see cref="IFileCabinetService"/>.</param>
        /// <param name="inputPrinter">Input parametr start id.<see cref="IRecordPrinter"/>.</param>
        public FindCommandHandler(IFileCabinetService fileCabinetService, Action<IEnumerable<FileCabinetRecord>> inputPrinter)
            : base(fileCabinetService)
        {
            this.printer = inputPrinter;
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

                if (appCommandRequest.Command == "find")
                {
                    var parameters = appCommandRequest.Parameters;
                    this.Find(parameters);
                }
                else
                {
                    base.Handle(appCommandRequest);
                }
            }

        private void Find(string parameters)
        {
            CultureInfo provider = new CultureInfo("en-US");
            var parametersArray = parameters.ToLower(provider).Split(' ', 2);
            if (parametersArray.Length < 2)
            {
                Console.WriteLine($"There is not input parameters.");
                return;
            }

            string searchParametr = parametersArray[0];
            string value = parametersArray[1];

            if (searchParametr == "firstname")
            {
                var firstName = parametersArray[1].Trim('"');
                var records = service.FindByFirstName(firstName);
                this.PrintRecords(records, searchParametr, value);
            }
            else if (searchParametr == "lastname")
            {
                var lastName = parametersArray[1].Trim('"');
                var records = service.FindByLastName(lastName);
                this.PrintRecords(records, searchParametr, value);
            }
            else if (searchParametr == "dateofbirth")
            {
                var dateofbirth = parametersArray[1].Trim('"');
                var records = service.FindByDateOfBirth(dateofbirth);
                this.PrintRecords(records, searchParametr, value);
            }
            else
            {
                Console.WriteLine($"There is no '{parametersArray[0]}' parametr.");
            }
        }

        private void PrintRecords(ReadOnlyCollection<FileCabinetRecord> records, string searchparametr, string value)
        {
            if (records.Count == 0)
            {
                Console.WriteLine($"No records are found for {searchparametr} = {value}!");
            }
            else
            {
                this.printer(records);
            }
        }
    }
}
