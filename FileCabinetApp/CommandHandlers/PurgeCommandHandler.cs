using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Purge command.
    /// </summary>
    public class PurgeCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Override method Handle by CommandHandlerBase in PurgeCommandHandler.
        /// </summary>
        /// <param name="appCommandRequest">Input parametr record <see cref="AppCommandRequest"/>.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest == null)
            {
                throw new ArgumentNullException(nameof(appCommandRequest));
            }

            if (appCommandRequest.Command == "purge")
            {
                var parameters = appCommandRequest.Parameters;
                Purge(parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private static void Purge(string parameters)
        {
            if (Program.fileCabinetService is FileCabinetFilesystemService)
            {
                Program.fileCabinetService.Purge();
            }
            else
            {
                Console.WriteLine("fileCabinetService is not FileCabinetFilesystemService.");
            }
        }
    }
}
