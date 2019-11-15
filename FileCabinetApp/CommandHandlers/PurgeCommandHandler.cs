using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Purge command.
    /// </summary>
    public class PurgeCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Input parametr start id.<see cref="IFileCabinetService"/>.</param>
        public PurgeCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

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
            if (service is FileCabinetFilesystemService)
            {
                service.Purge();
            }
            else
            {
                Console.WriteLine("fileCabinetService is not FileCabinetFilesystemService.");
            }
        }
    }
}
