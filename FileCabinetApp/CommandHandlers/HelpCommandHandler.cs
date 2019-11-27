using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Help command.
    /// </summary>
    public class HelpCommandHandler : CommandHandlerBase
    {
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "stat of the record", "The 'stat' command return info about the record." },
            new string[] { "insert", "create a new record", "The 'insert' add record by conditional." },
            new string[] { "select", "get the record", "The 'select' command return record(s) by conditional." },
            new string[] { "create", "create a new record", "The 'create' command add info int the service." },
            new string[] { "list", "list return a copy of record", "The 'list' command return copy all records." },
            new string[] { "update", "updaterecord", "The 'update' edit record by conditional." },
            new string[] { "find", "find record(s)", "The 'find' return all record by conditional." },
            new string[] { "export", "export file", "The 'export' export records to file." },
            new string[] { "import", "import file", "The 'import' import type of file." },
            new string[] { "delete", "delete record(s) by conditional", "The 'delete' delete record(s)." },
            new string[] { "purge", "purge records in FileCabinetFilesystemService", "The 'purge' records in FileCabinetFilesystemService." },
        };

        /// <summary>
        /// Override method Handle by CommandHandlerBase in HelpCommandHandler.
        /// </summary>
        /// <param name="appCommandRequest">Input parametr record <see cref="AppCommandRequest"/>.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest == null)
            {
                throw new ArgumentNullException(nameof(appCommandRequest));
            }

            if (appCommandRequest.Command == "help")
            {
                var parameters = appCommandRequest.Parameters;
                PrintHelp(parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[CommandHelpIndex], helpMessage[DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }
    }
}
