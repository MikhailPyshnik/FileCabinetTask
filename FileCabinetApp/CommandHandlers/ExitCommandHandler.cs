﻿using System;
using System.IO;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Exit command.
    /// </summary>
    public class ExitCommandHandler : ServiceCommandHandlerBase
    {
        private static FileStream fileStream;
        private static Action<bool> action;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Input parametr start id.<see cref="IFileCabinetService"/>.</param>
        /// <param name="filestream">Input parametr fileStream.<see cref="FileStream"/>.</param>
        /// <param name="exit">Input delegate Action.<see cref="Action"/>.</param>
        public ExitCommandHandler(IFileCabinetService fileCabinetService, FileStream filestream, Action<bool> exit)
             : base(fileCabinetService)
        {
            if (fileCabinetService is FileCabinetFilesystemService)
            {
                fileStream = filestream ?? throw new ArgumentNullException(nameof(filestream));
            }

            action = exit ?? throw new ArgumentNullException(nameof(exit));
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
                fileStream.Close();
            }

            Console.WriteLine("Exiting an application...");
            action(false);
        }
    }
}
