using System;
using System.Globalization;
using System.IO;
using CommandLine;
using FileCabinetApp.CommandHandlers;

namespace FileCabinetApp
{
    /// <summary>
    /// Class of console application.
    /// </summary>
    public static class Program
    {
#pragma warning disable SA1401 // Fields should be private
#pragma warning disable CA2211 // Non-constant fields should not be visible
#pragma warning disable SA1307 // Accessible fields should begin with upper-case letter
#pragma warning disable SA1600 // Elements should be documented
        public static bool isRunning = true;
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore SA1307 // Accessible fields should begin with upper-case letter
#pragma warning restore CA2211 // Non-constant fields should not be visible
#pragma warning restore SA1401 // Fields should be private
        private const string DeveloperName = "Mikhail Pyshnik";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";

        private static IFileCabinetService fileCabinetService;
        private static IValidatorOfParemetrs recordValidator;
        private static FileStream filestream;

        private static string validationRules = "default";

        private static string storageRules = "memory";

        private static string[] existCommands = new string[] { "help", "exit", "stat", "create", "list", "edit", "find", "export", "import", "remove", "purge" };

        /// <summary>
        ///  Method Main of console application.The poin of enter application.
        /// </summary>
        /// <param name="args">Input parametr args[] <see cref="string"/>.</param>
        public static void Main(string[] args)
        {
            var options = new Options();
            var result = Parser.Default
                               .ParseArguments<Options>(args)
                               .WithParsed(parsed => options = parsed);
            if (result.Tag == ParserResultType.NotParsed)
            {
                Console.WriteLine($"Not parsed command!");
                recordValidator = new DefaultValidator();
                fileCabinetService = new FileCabinetMemoryService();
            }
            else
            {
                if (options.InputFile == null)
                {
                    recordValidator = new DefaultValidator();
                }
                else
                {
                    string compareInputFile = options.InputFile.ToLower(new CultureInfo("en-US"));
                    if (compareInputFile == "custom")
                    {
                        recordValidator = new CustomValidator();
                        validationRules = "custom";
                    }
                    else
                    {
                        recordValidator = new DefaultValidator();
                    }
                }

                if (options.InputStorage == null)
                {
                    fileCabinetService = new FileCabinetMemoryService();
                }
                else
                {
                    string comandStorage = options.InputStorage.ToLower(new CultureInfo("en-US"));

                    if (comandStorage == "file")
                    {
                        filestream = File.Open("cabinet-records.db", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                        storageRules = "file";
                        fileCabinetService = new FileCabinetFilesystemService(filestream);
                    }
                    else
                    {
                        fileCabinetService = new FileCabinetMemoryService();
                    }
                }

                Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
                Console.WriteLine($"Using {validationRules} validation rules.");
                Console.WriteLine($"Using {storageRules} storage rules.");
                Console.WriteLine(Program.HintMessage);
                Console.WriteLine();

                do
                {
                    Console.Write("> ");
                    var inputs = Console.ReadLine().Split(' ', 2);
                    const int commandIndex = 0;
                    var command = inputs[commandIndex];

                    if (string.IsNullOrEmpty(command))
                    {
                        Console.WriteLine(Program.HintMessage);
                        continue;
                    }

                    if (!Array.Exists(existCommands, element => element == command))
                    {
                        PrintMissedCommandInfo(command);
                        continue;
                    }

                    var index = inputs.Length;
                    if (index >= 0)
                    {
                        const int parametersIndex = 1;
                        var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;

                        var commandHandler = CreateCommandHandlers(fileCabinetService, recordValidator);

                        commandHandler.Handle(
                            new AppCommandRequest
                            {
                                Command = command,
                                Parameters = parameters,
                            });
                    }
                }
                while (isRunning);
            }
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static ICommandHandler CreateCommandHandlers(IFileCabinetService fileCabinetService, IValidatorOfParemetrs validatorOfParemetrs)
        {
            var helpHandler = new HelpCommandHandler();
            var createHandler = new CreateCommandHandler(fileCabinetService, validatorOfParemetrs);
            var importHandler = new ImportCommandHandler(fileCabinetService, validatorOfParemetrs);
            var statHandler = new StatCommandHandler(fileCabinetService);
            var listHandler = new ListCommandHandler(fileCabinetService);
            var findHandler = new FindCommandHandler(fileCabinetService);
            var editHandler = new EditCommandHandler(fileCabinetService, validatorOfParemetrs);
            var removeHandler = new RemoveCommandHandler(fileCabinetService);
            var purgeHandler = new PurgeCommandHandler(fileCabinetService);
            var exportHandler = new ExportCommandHandler(fileCabinetService);
            var exitHandler = new ExitCommandHandler(fileCabinetService, filestream);
            helpHandler.SetNext(createHandler)
                       .SetNext(importHandler)
                       .SetNext(statHandler)
                       .SetNext(listHandler)
                       .SetNext(findHandler)
                       .SetNext(editHandler)
                       .SetNext(removeHandler)
                       .SetNext(purgeHandler)
                       .SetNext(exportHandler)
                       .SetNext(exitHandler);
            return helpHandler;
        }

        private class Options
        {
            [Option('v', "validation-rules", Separator = '=', HelpText = "Set output to verbose messages.")]
            public string InputFile { get; set; }

            [Option('s', "storage", Separator = ' ', HelpText = "Set output to verbose messages.")]
            public string InputStorage { get; set; }
        }
    }
}