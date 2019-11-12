using System;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Find command.
    /// </summary>
    public class FindCommandHandler : CommandHandlerBase
    {
        private static IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Input parametr start id.<see cref="IFileCabinetService"/>.</param>
        public FindCommandHandler(IFileCabinetService fileCabinetService)
        {
            service = fileCabinetService;
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
                Find(parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private static void Find(string parameters)
        {
            CultureInfo provider = new CultureInfo("en-US");
            var parametersArray = parameters.ToLower(provider).Split(' ', 2);
            string searchParametr = parametersArray[0];
            string value = parametersArray[1];
            if (searchParametr == "firstname")
            {
                var firstName = parametersArray[1].Trim('"');
                var records = service.FindByFirstName(firstName);
                PrintRecords(records, searchParametr, value);
            }
            else if (searchParametr == "lastname")
            {
                var lastName = parametersArray[1].Trim('"');
                var records = service.FindByLastName(lastName);
                PrintRecords(records, searchParametr, value);
            }
            else if (searchParametr == "dateofbirth")
            {
                var dateofbirth = parametersArray[1].Trim('"');
                var records = service.FindByDateOfBirth(dateofbirth);
                PrintRecords(records, searchParametr, value);
            }
            else
            {
                Console.WriteLine($"There is no '{parametersArray[0]}' parametr.");
            }
        }

        private static void PrintRecords(ReadOnlyCollection<FileCabinetRecord> records, string searchparametr, string value)
        {
            CultureInfo provider = new CultureInfo("en-US");
            if (records.Count == 0)
            {
                Console.WriteLine($"No records are found for {searchparametr} = {value}!");
            }
            else
            {
                foreach (var record in records)
                {
                    Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth.ToString("yyyy-MMM-dd", provider)}, {record.Sex}, {record.Height}, {record.Salary}");
                }
            }
        }
    }
}
