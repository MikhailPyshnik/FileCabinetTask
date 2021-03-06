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
        private static Action<string> printAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExitCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Input parametr start id.<see cref="IFileCabinetService"/>.</param>
        /// <param name="filestream">Input parametr fileStream.<see cref="FileStream"/>.</param>
        /// <param name="exit">Input delegate Action.<see cref="Action"/>.</param>
        /// <param name="printMethod">Input delegate Action for print messages.<see cref="Action"/>.</param>
        public ExitCommandHandler(IFileCabinetService fileCabinetService, FileStream filestream, Action<bool> exit, Action<string> printMethod)
             : base(fileCabinetService)
        {
            if (fileCabinetService is FileCabinetFilesystemService)
            {
                fileStream = filestream ?? throw new ArgumentNullException(nameof(filestream));
            }

            action = exit ?? throw new ArgumentNullException(nameof(exit));
            printAction = printMethod ?? throw new ArgumentNullException(nameof(printMethod));
        }

        /// <summary>
        /// Override method Handle by CommandHandlerBase in ExitCommandHandler.
        /// </summary>
        /// <param name="appCommandRequest">Input parametr record <see cref="AppCommandRequest"/>.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest is null)
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
            printAction("Exiting an application...");
            action(false);
            Console.ReadLine();
        }
    }
}
