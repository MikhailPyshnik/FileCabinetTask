using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Exit command.
    /// </summary>
    public class ExitCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Override method Handle by CommandHandlerBase in ExitCommandHandler.
        /// </summary>
        /// <param name="appCommandRequest">Input parametr record <see cref="AppCommandRequest"/>.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest == null)
            {
                throw new ArgumentNullException(nameof(appCommandRequest));
            }

            if (appCommandRequest.Command == "exit")
            {
                var parameters = appCommandRequest.Parameters;
                Exit(parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private static void Exit(string parameters)
        {
            if (Program.fileCabinetService is FileCabinetFilesystemService)
            {
                Program.filestream.Close();
            }

            Console.WriteLine("Exiting an application...");
            Program.isRunning = false;
        }
    }
}
