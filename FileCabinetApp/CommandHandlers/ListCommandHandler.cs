using System;
using System.Globalization;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// List command.
    /// </summary>
    public class ListCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Input parametr start id.<see cref="IFileCabinetService"/>.</param>
        public ListCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        /// <summary>
        /// Override method Handle by CommandHandlerBase in ListCommandHandler.
        /// </summary>
        /// <param name="appCommandRequest">Input parametr record <see cref="AppCommandRequest"/>.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest == null)
            {
                throw new ArgumentNullException(nameof(appCommandRequest));
            }

            if (appCommandRequest.Command == "list")
            {
                var parameters = appCommandRequest.Parameters;
                List(parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private static void List(string parameters)
        {
            CultureInfo provider = new CultureInfo("en-US");
            var reultList = service.GetRecords();
            if (reultList.Count == 0)
            {
                Console.Write("The list is empty.Add new record => add command - create");
            }
            else
            {
                for (int i = 0; i < reultList.Count; i++)
                {
                    Console.WriteLine($"#{reultList[i].Id},{reultList[i].FirstName},{reultList[i].LastName},{reultList[i].DateOfBirth.ToString("yyyy-MMM-dd", provider)},{reultList[i].Sex},{reultList[i].Height},{reultList[i].Salary}");
                }
            }
        }
    }
}
