using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// List command.
    /// </summary>
    public class ListCommandHandler : ServiceCommandHandlerBase
    {
        private Action<IEnumerable<FileCabinetRecord>> printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Input parametr start id.<see cref="IFileCabinetService"/>.</param>
        /// <param name="inputPrinter">Input parametr start id.<see cref="IRecordPrinter"/>.</param>
        public ListCommandHandler(IFileCabinetService fileCabinetService, Action<IEnumerable<FileCabinetRecord>> inputPrinter)
            : base(fileCabinetService)
        {
            this.printer = inputPrinter;
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
                this.List(parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private void List(string parameters)
        {
            var reultList = service.GetRecords();

            if (reultList.Count == 0)
            {
                Console.Write("The list is empty.Add new record => add command - create");
            }
            else
            {
               this.printer(reultList);
            }
        }
    }
}
