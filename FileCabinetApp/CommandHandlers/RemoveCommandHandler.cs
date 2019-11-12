using System;
using System.Collections.Generic;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Remove command.
    /// </summary>
    public class RemoveCommandHandler : CommandHandlerBase
    {
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

            if (appCommandRequest.Command == "remove")
            {
                var parameters = appCommandRequest.Parameters;
                Remove(parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private static void Remove(string parameters)
        {
            if (Program.fileCabinetService.GetStat().Item1 < 1)
            {
                Console.WriteLine("File is not empty.");
                return;
            }

            var parametersArray = parameters.Split(' ', 1);
            if (parameters.Length == 0)
            {
                Console.WriteLine("remove command is empty!");
                return;
            }

            if (parametersArray.Length > 1)
            {
                Console.WriteLine("Incorrect value input in comand remove!");
                return;
            }

            string inputId = parametersArray[0];
            int validId;
            bool result = int.TryParse(inputId, out validId);

            List<FileCabinetRecord> list = new List<FileCabinetRecord>(Program.fileCabinetService.GetRecords());

            bool isExists = list.Exists(record => record.Id == validId);

            if (result && isExists)
            {
                Program.fileCabinetService.Remove(validId);
                Console.WriteLine($"Record #{inputId} is removed.");
            }
            else
            {
                Console.WriteLine($"Record #{inputId} doesn't exists.");
            }
        }
    }
}
