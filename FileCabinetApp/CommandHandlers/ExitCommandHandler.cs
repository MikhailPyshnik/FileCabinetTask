using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Exit command.
    /// </summary>
    public class ExitCommandHandler : CommandHandlerBase
    {
        private static IFileCabinetService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Input parametr start id.<see cref="IFileCabinetService"/>.</param>
        public ExitCommandHandler(IFileCabinetService fileCabinetService)
        {
            service = fileCabinetService;
        }

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
            if (service is FileCabinetFilesystemService)
            {
                Program.filestream.Close();
            }

            Console.WriteLine("Exiting an application...");
            Program.isRunning = false;
        }
    }
}
