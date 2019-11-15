using System;
using System.Collections.Generic;
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
        private const string DeveloperName = "Mikhail Pyshnik";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";

        private static IFileCabinetService fileCabinetService;
        private static IValidatorOfParemetrs recorInputdValidator;
        private static IRecordValidator recordValidator;
        private static FileStream filestream;
        private static bool isRunning = true;

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
                recorInputdValidator = new DefaultValidator();
                recordValidator = new ValidatorBuilder().CreateDefault();
                fileCabinetService = new FileCabinetMemoryService(recordValidator);
            }
            else
            {
                if (options.InputFile == null)
                {
                    recorInputdValidator = new DefaultValidator();
                    recordValidator = new ValidatorBuilder().CreateDefault();
                }
                else
                {
                    string compareInputFile = options.InputFile.ToLower(new CultureInfo("en-US"));
                    if (compareInputFile == "custom")
                    {
                        recorInputdValidator = new CustomValidator();
                        recordValidator = new ValidatorBuilder().CreateCustom();
                        validationRules = "custom";
                    }
                    else
                    {
                        recorInputdValidator = new DefaultValidator();
                        recordValidator = new ValidatorBuilder().CreateDefault();
                    }
                }

                if (options.InputStorage == null)
                {
                    fileCabinetService = new FileCabinetMemoryService(recordValidator);
                }
                else
                {
                    string comandStorage = options.InputStorage.ToLower(new CultureInfo("en-US"));

                    if (comandStorage == "file")
                    {
                        filestream = File.Open("cabinet-records.db", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                        storageRules = "file";
                        fileCabinetService = new FileCabinetFilesystemService(filestream, recordValidator);
                    }
                    else
                    {
                        fileCabinetService = new FileCabinetMemoryService(recordValidator);
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

                        var commandHandler = CreateCommandHandlers();

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

        private static ICommandHandler CreateCommandHandlers()
        {
            var helpHandler = new HelpCommandHandler();
            var createHandler = new CreateCommandHandler(fileCabinetService, recorInputdValidator);
            var importHandler = new ImportCommandHandler(fileCabinetService, recorInputdValidator);
            var statHandler = new StatCommandHandler(fileCabinetService);
            var listHandler = new ListCommandHandler(fileCabinetService, DefaultRecordPrinter);
            var findHandler = new FindCommandHandler(fileCabinetService, DefaultRecordPrinter);
            var editHandler = new EditCommandHandler(fileCabinetService, recorInputdValidator);
            var removeHandler = new RemoveCommandHandler(fileCabinetService);
            var purgeHandler = new PurgeCommandHandler(fileCabinetService);
            var exportHandler = new ExportCommandHandler(fileCabinetService);
            var exitHandler = new ExitCommandHandler(fileCabinetService, filestream, ActionIsRunning);
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

        private static void ActionIsRunning(bool running)
        {
            if (!running)
            {
                isRunning = false;
            }
        }

        private static void DefaultRecordPrinter(IEnumerable<FileCabinetRecord> records)
        {
            if (records == null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            CultureInfo provider = new CultureInfo("en-US");
            foreach (var record in records)
            {
                Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth.ToString("yyyy-MMM-dd", provider)}, {record.Sex}, {record.Height}, {record.Salary}");
            }
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