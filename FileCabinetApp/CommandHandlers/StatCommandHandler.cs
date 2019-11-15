using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Stat command.
    /// </summary>
    public class StatCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Input parametr start id.<see cref="IFileCabinetService"/>.</param>
        public StatCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
        }

        /// <summary>
        /// Override method Handle by CommandHandlerBase in StatCommandHandler.
        /// </summary>
        /// <param name="appCommandRequest">Input parametr record <see cref="AppCommandRequest"/>.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest == null)
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
            if (service is FileCabinetFilesystemService)
            {
                var recordsCount = service.GetStat().Item1;
                var deleteRecords = service.GetStat().Item2;
                Console.WriteLine($"{recordsCount} record(s).");
                Console.WriteLine($"{deleteRecords} record(s) were deleted.");
            }
            else
            {
                var recordsCount = service.GetStat().Item1;
                Console.WriteLine($"{recordsCount} record(s).");
            }
        }
    }
}
