using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Stat command.
    /// </summary>
    public class StatCommandHandler : CommandHandlerBase
    {
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
            if (Program.fileCabinetService is FileCabinetFilesystemService)
            {
                var recordsCount = Program.fileCabinetService.GetStat().Item1;
                var deleteRecords = Program.fileCabinetService.GetStat().Item2;
                Console.WriteLine($"{recordsCount} record(s).");
                Console.WriteLine($"{deleteRecords} record(s) were deleted.");
            }
            else
            {
                var recordsCount = Program.fileCabinetService.GetStat().Item1;
                Console.WriteLine($"{recordsCount} record(s).");
            }
        }
    }
}
