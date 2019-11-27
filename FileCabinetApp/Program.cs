using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CommandLine;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.Configuration;
using Microsoft.Extensions.Configuration;

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

        private static string stopWatchRules = "Not used stopwatch";

        private static string loggerRules = "Not used logger";

        private static string[] existCommands = new string[] { "help", "exit", "stat", "create", "list", "find", "export", "import", "purge", "insert", "delete", "update", "select" };

        /// <summary>
        ///  Method Main of console application.The poin of enter application.
        /// </summary>
        /// <param name="args">Input parametr args[] <see cref="string"/>.</param>
        public static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                                         .AddJsonFile("validation-rules.json", true, true)
                                         .Build();

            var validationRulesFromJson = config.Get<ConfigurationFilecabinet>();

            var customValidateRule = validationRulesFromJson.Custom;
            var defaultValidateRule = validationRulesFromJson.Default;

            var options = new Options();
            var result = Parser.Default
                               .ParseArguments<Options>(args)
                               .WithParsed(parsed => options = parsed);
            if (result.Tag == ParserResultType.NotParsed)
            {
                Console.WriteLine($"Not parsed command!");
                recorInputdValidator = new InputValidator(defaultValidateRule);
                recordValidator = new ValidatorBuilder().CreateDefault(defaultValidateRule);
                fileCabinetService = new FileCabinetMemoryService(recordValidator);
            }
            else
            {
                if (options.InputFile == null)
                {
                    recorInputdValidator = new InputValidator(defaultValidateRule);
                    recordValidator = new ValidatorBuilder().CreateDefault(defaultValidateRule);
                }
                else
                {
                    string compareInputFile = options.InputFile.ToLower(new CultureInfo("en-US"));
                    if (compareInputFile == "custom")
                    {
                        recorInputdValidator = new InputValidator(customValidateRule);
                        recordValidator = new ValidatorBuilder().CreateCustom(customValidateRule);
                        validationRules = "custom";
                    }
                    else
                    {
                        recorInputdValidator = new InputValidator(defaultValidateRule);
                        recordValidator = new ValidatorBuilder().CreateDefault(defaultValidateRule);
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

                if (options.InputStopwatch)
                {
                    fileCabinetService.Validator = recorInputdValidator;
                    fileCabinetService = new ServiceMeter(fileCabinetService);
                    stopWatchRules = "Use stopwatch";
                }

                if (options.InputLogger)
                {
                    fileCabinetService.Validator = recorInputdValidator;
                    fileCabinetService = new ServiceLogger(fileCabinetService);
                    loggerRules = "Use logger";
                }

                Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
                Console.WriteLine($"Using {validationRules} validation rules.");
                Console.WriteLine($"Using {storageRules} storage rules.");
                Console.WriteLine($"{stopWatchRules}.");
                Console.WriteLine($"{loggerRules}.");
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
                        List<string> foundWords = Search(command, existCommands, 0.4);
                        Console.WriteLine($"{command} is not a command.See command - help.");
                        Console.WriteLine("The most similar commands:");
                        foundWords.ForEach(i => Console.WriteLine(i));
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
            var insertHandler = new InsertCommandHandler(fileCabinetService);
            var importHandler = new ImportCommandHandler(fileCabinetService, recorInputdValidator);
            var deleteHandler = new DeleteCommandHandler(fileCabinetService);
            var statHandler = new StatCommandHandler(fileCabinetService);
            var selectHandler = new SelectCommandHandler(fileCabinetService, RecordPrinter);
            var updateHandler = new UpdateCommandHandler(fileCabinetService);
            var purgeHandler = new PurgeCommandHandler(fileCabinetService);
            var exportHandler = new ExportCommandHandler(fileCabinetService);
            var exitHandler = new ExitCommandHandler(fileCabinetService, filestream, ActionIsRunning);
            helpHandler.SetNext(createHandler)
                       .SetNext(insertHandler)
                       .SetNext(deleteHandler)
                       .SetNext(importHandler)
                       .SetNext(statHandler)
                       .SetNext(selectHandler)
                       .SetNext(updateHandler)
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

            if (!records.GetEnumerator().MoveNext())
            {
                Console.WriteLine("Don't find records!");
            }
            else
            {
                CultureInfo provider = new CultureInfo("en-US");
                foreach (var record in records)
                {
                    Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth.ToString("yyyy-MMM-dd", provider)}, {record.Sex}, {record.Height}, {record.Salary}");
                }
            }
        }

        private static void RecordPrinter(IEnumerable<FileCabinetRecord> records, string[] fields)
        {
            if (records == null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            if (!records.GetEnumerator().MoveNext())
            {
                Console.WriteLine("Don't find records!");
            }
            else
            {
                CultureInfo provider = new CultureInfo("en-US");
                List<int> lengths;
                List<string[]> rows = new List<string[]>();

                int fieldsLengthArray = fields.Length;

                lengths = fields.Select(t => t.Length).ToList();

                for (int i = 0; i < fields.Length; i++)
                {
                    int lenghtMax = GetFilecabinetRecordParametrToStringMaxLenght(records, fields[i]);
                    if (lengths[i] < lenghtMax)
                    {
                        lengths[i] = lenghtMax;
                    }
                }

                foreach (var record in records)
                {
                    string[] temp = new string[fieldsLengthArray];
                    for (int i = 0; i < fields.Length; i++)
                    {
                        temp[i] = GetFilecabinetRecordParametrToString(record, fields[i]);
                    }

                    rows.Add(temp);
                }

                lengths.ForEach(l => System.Console.Write("+-" + new string('-', l) + '-'));
                Console.WriteLine("+");

                string line = string.Empty;
                for (int i = 0; i < fields.Length; i++)
                {
                    line += "| " + fields[i].PadRight(lengths[i]) + ' ';
                }

                Console.WriteLine(line + "|");

                lengths.ForEach(l => System.Console.Write("+-" + new string('-', l) + '-'));
                Console.WriteLine("+");

                foreach (var row in rows)
                {
                    line = string.Empty;
                    for (int i = 0; i < row.Length; i++)
                    {
                        if (int.TryParse(row[i], out int n))
                        {
                            line += "| " + row[i].PadLeft(lengths[i]) + ' ';  // numbers are padded to the left
                        }
                        else
                        {
                            line += "| " + row[i].PadRight(lengths[i]) + ' ';
                        }
                    }

                    Console.WriteLine(line + "|");
                }

                lengths.ForEach(l => System.Console.Write("+-" + new string('-', l) + '-'));
                Console.WriteLine("+");
            }
        }

        private static string GetFilecabinetRecordParametrToString(FileCabinetRecord record, string parameter)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            CultureInfo provider = new CultureInfo("en-US");

            string result;
            switch (parameter)
            {
                case "id":
                    result = record.Id.ToString(provider);
                    break;
                case "firstname":
                    result = record.FirstName.ToString(provider);
                    break;
                case "lastname":
                    result = record.LastName.ToString(provider);
                    break;
                case "dateofbirth":
                    result = record.DateOfBirth.ToString("yyyy-MMM-dd", provider);
                    break;
                case "sex":
                    result = record.Sex.ToString(provider);
                    break;
                case "height":
                    result = record.Height.ToString(provider);
                    break;
                case "salary":
                    result = record.Salary.ToString(provider);
                    break;
                default:
                    throw new ArgumentException("Not correct value!!!!");
            }

            return result;
        }

        private static int GetFilecabinetRecordParametrToStringMaxLenght(IEnumerable<FileCabinetRecord> records, string parameter)
        {
            if (records == null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            CultureInfo provider = new CultureInfo("en-US");
            int resulMaxtStart = 0;
            int result;

            foreach (var record in records)
            {
                switch (parameter)
                {
                    case "id":
                        result = record.Id.ToString(provider).Length;
                        break;
                    case "firstname":
                        result = record.FirstName.ToString(provider).Length;
                        break;
                    case "lastname":
                        result = record.LastName.ToString(provider).Length;
                        break;
                    case "dateofbirth":
                        result = record.DateOfBirth.ToString("yyyy-MMM-dd", provider).Length;
                        break;
                    case "sex":
                        result = record.Sex.ToString(provider).Length;
                        break;
                    case "height":
                        result = record.Height.ToString(provider).Length;
                        break;
                    case "salary":
                        result = record.Salary.ToString(provider).Length;
                        break;
                    default:
                        throw new ArgumentException("Not correct length value!!!!");
                }

                if (result > resulMaxtStart)
                {
                    resulMaxtStart = result;
                }
            }

            return resulMaxtStart;
        }

        private static List<string> Search(string word, string[] wordList, double fuzzyness)
        {
            List<string> foundWords = new List<string>();

            foreach (string s in wordList)
            {
                // Calculate the Levenshtein-distance:
                int levenshteinDistance =
                    LevenshteinDistance(word, s);

                // Length of the longer string:
                int length = Math.Max(word.Length, s.Length);

                // Calculate the score:
                double score = 1.0 - ((double)levenshteinDistance / length);

                // Match?
                if (score > fuzzyness)
                {
                    foundWords.Add(s);
                }
            }

            return foundWords;
        }

        private static int LevenshteinDistance(string src, string dest)
        {
#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional
            int[,] d = new int[src.Length + 1, dest.Length + 1];
#pragma warning restore CA1814 // Prefer jagged arrays over multidimensional
            int i, j, cost;
            char[] str1 = src.ToCharArray();
            char[] str2 = dest.ToCharArray();

            for (i = 0; i <= str1.Length; i++)
            {
                d[i, 0] = i;
            }

            for (j = 0; j <= str2.Length; j++)
            {
                d[0, j] = j;
            }

            for (i = 1; i <= str1.Length; i++)
            {
                for (j = 1; j <= str2.Length; j++)
                {
                    if (str1[i - 1] == str2[j - 1])
                    {
                        cost = 0;
                    }
                    else
                    {
                        cost = 1;
                    }

                    d[i, j] =
                        Math.Min(
                            d[i - 1, j] + 1,              // Deletion
                            Math.Min(
                                d[i, j - 1] + 1,          // Insertion
                                d[i - 1, j - 1] + cost)); // Substitution

                    if ((i > 1) && (j > 1) && (str1[i - 1] ==
                        str2[j - 2]) && (str1[i - 2] == str2[j - 1]))
                    {
                        d[i, j] = Math.Min(d[i, j], d[i - 2, j - 2] + cost);
                    }
                }
            }

            return d[str1.Length, str2.Length];
        }

        private class Options
        {
            [Option('v', "validation-rules", Separator = '=', HelpText = "Set output to verbose messages.")]
            public string InputFile { get; set; }

            [Option('s', "storage", Separator = ' ', HelpText = "Set output to verbose messages.")]
            public string InputStorage { get; set; }

            [Option("use-stopwatch", Required = false, HelpText = "Use stopwatch.")]
            public bool InputStopwatch { get; set; }

            [Option("use-logger", Required = false, HelpText = "Use logger.")]
            public bool InputLogger { get; set; }
        }
    }
}