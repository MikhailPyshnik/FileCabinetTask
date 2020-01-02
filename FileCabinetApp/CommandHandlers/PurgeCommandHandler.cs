using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Purge command.
    /// </summary>
    public class PurgeCommandHandler : ServiceCommandHandlerBase
    {
        private static Action<string> action;

        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Input parametr start id.<see cref="IFileCabinetService"/>.</param>
        /// <param name="printMethod">Input delegate Action for print messages.<see cref="Action"/>.</param>
        public PurgeCommandHandler(IFileCabinetService fileCabinetService, Action<string> printMethod)
            : base(fileCabinetService)
        {
            action = printMethod ?? throw new ArgumentNullException(nameof(printMethod));
        }

        /// <summary>
        /// Override method Handle by CommandHandlerBase in PurgeCommandHandler.
        /// </summary>
        /// <param name="appCommandRequest">Input parametr record <see cref="AppCommandRequest"/>.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest is null)
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
            if (service.FileCabinetProperties.FileCabinetProperties is FileCabinetFilesystemService)
            {
                var result = service.Purge();
                action($"Data file processing is completed: {result.Item1} of {result.Item2} records were purged.");
            }
            else
            {
                action($"{service.FileCabinetProperties.FileCabinetProperties.GetType()} is not FileCabinetFilesystemService.");
            }
        }
    }
}
