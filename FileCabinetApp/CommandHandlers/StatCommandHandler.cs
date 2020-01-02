using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Stat command.
    /// </summary>
    public class StatCommandHandler : ServiceCommandHandlerBase
    {
        private static Action<string> action;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Input parametr start id.<see cref="IFileCabinetService"/>.</param>
        /// <param name="printMethod">Input delegate Action for print messages.<see cref="Action"/>.</param>
        public StatCommandHandler(IFileCabinetService fileCabinetService, Action<string> printMethod)
            : base(fileCabinetService)
        {
            action = printMethod ?? throw new ArgumentNullException(nameof(printMethod));
        }

        /// <summary>
        /// Override method Handle by CommandHandlerBase in StatCommandHandler.
        /// </summary>
        /// <param name="appCommandRequest">Input parametr record <see cref="AppCommandRequest"/>.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest is null)
            {
                throw new ArgumentNullException(nameof(appCommandRequest));
            }

            if (appCommandRequest.Command == "stat")
            {
                var parameters = appCommandRequest.Parameters;
                Stat(parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private static void Stat(string parameters)
        {
            if (service.FileCabinetProperties.FileCabinetProperties is FileCabinetFilesystemService)
            {
                var recordsCount = service.GetStat().Item1;
                var deleteRecords = service.GetStat().Item2;
                action($"{recordsCount} record(s).");
                action($"{deleteRecords} record(s) were deleted.");
            }
            else
            {
                var recordsCount = service.GetStat().Item1;
                action($"{recordsCount} record(s).");
            }
        }
    }
}
