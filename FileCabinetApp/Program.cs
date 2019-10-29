using System;
using System.Collections.ObjectModel;
using System.Globalization;
using CommandLine;

namespace FileCabinetApp
{
    /// <summary>
    /// Class of console application.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Mikhail Pyshnik";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private static IFileCabinetService fileCabinetService;
        private static IValidatorOfParemetrs recordValidator;

        private static string validationRules = "default";

        private static bool isRunning = true;

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "stat of the record", "The 'stat' command return info about the record." },
            new string[] { "create", "create a new record", "The 'create' command add info int the service." },
            new string[] { "list", "list return a copy of record", "The 'list' command return copy all records." },
            new string[] { "edit", "edit record", "The 'edit' edit record by id." },
            new string[] { "find", "find record(s)", "The 'find' return all record by условие." },
        };

        /// <summary>
        ///  Method Main of console application.The poin of enter application.
        /// </summary>
        /// <param name="args">Input parametr args[] <see cref="string"/>.</param>
        public static void Main(string[] args)
        {
            var options = new Options();
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o =>
                {
                    if (args == null || args.Length == 0)
                    {
                       fileCabinetService = new FileCabinetService(new DefaultValidator());
                       recordValidator = new DefaultValidator();
                    }
                    else
                    {
                        string compare = o.InputFile.ToLower(new CultureInfo("en-US"));
                        if (compare == "default")
                        {
                           fileCabinetService = new FileCabinetService(new DefaultValidator());
                           recordValidator = new DefaultValidator();
                        }
                        else if (compare == "custom")
                        {
                           fileCabinetService = new FileCabinetService(new CustomValidator());
                           recordValidator = new CustomValidator();
                           validationRules = "custom";
                        }
                        else
                        {
                            fileCabinetService = new FileCabinetService(new DefaultValidator());
                            recordValidator = new DefaultValidator();
                        }
                    }
                });

            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine($"Using {validationRules} validation rules.");
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

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
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
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
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
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            try
            {
                try
                {
                    FileCabinetRecord fileCabinetRecord = new FileCabinetRecord();
                    CultureInfo provider = new CultureInfo("en-US");
                    Console.Write("First name:");
                    fileCabinetRecord.FirstName = recordValidator.ReadInput(recordValidator.FirstNameConverter, recordValidator.FirstNameValidator);
                    Console.Write("Last name:");
                    fileCabinetRecord.LastName = recordValidator.ReadInput(recordValidator.LastNameConverter, recordValidator.LastNameValidator);
                    Console.Write("Date of birth:");
                    fileCabinetRecord.DateOfBirth = recordValidator.ReadInput(recordValidator.DayOfBirthConverter, recordValidator.DayOfBirthValidator);
                    Console.Write("Person's sex:");
                    fileCabinetRecord.Sex = recordValidator.ReadInput(recordValidator.SexConverter, recordValidator.SexValidator);
                    Console.Write("Person's height:");
                    fileCabinetRecord.Height = recordValidator.ReadInput(recordValidator.HeightConverter, recordValidator.HeightValidator);
                    Console.Write("Person's salary:");
                    fileCabinetRecord.Salary = recordValidator.ReadInput(recordValidator.SalaryConverter, recordValidator.SalaryValidator);
                    Console.WriteLine($"Record #{fileCabinetService.CreateRecord(fileCabinetRecord)} is created.");
                }
                catch (ArgumentNullException ex)
                {
                    Console.WriteLine($"{ex.Message} Enter the data again!");
                    Create(parameters);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"{ex.Message} Enter the data again!");
                    Create(parameters);
                }
                catch (Exception)
                {
                    throw new ArgumentException("Input value in Create is incorrect! Select  command again.");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }

        private static void List(string parameters)
        {
            CultureInfo provider = new CultureInfo("en-US");
            var reultList = fileCabinetService.GetRecords();
            if (reultList.Count == 0)
            {
                Console.Write("The list is empty.Add new record => add command - create");
            }
            else
            {
                for (int i = 0; i < reultList.Count; i++)
                {
                    Console.WriteLine($"#{reultList[i].Id},{reultList[i].FirstName},{reultList[i].LastName},{reultList[i].DateOfBirth.ToString("yyyy-MMM-dd", provider)},{reultList[i].Sex},{reultList[i].Height},{reultList[i].Salary}");
                }
            }
        }

        private static void Edit(string parameters)
        {
            try
            {
                try
                {
                    FileCabinetRecord fileCabinetRecord = new FileCabinetRecord();
                    CultureInfo provider = new CultureInfo("en-US");
                    int editInputId = Convert.ToInt32(parameters, provider);
                    if (editInputId > Program.fileCabinetService.GetStat())
                    {
                        Console.WriteLine($"#{editInputId} record is not found.");
                        return;
                    }

                    fileCabinetRecord.Id = editInputId;

                    Console.Write("First name:");
                    fileCabinetRecord.FirstName = recordValidator.ReadInput(recordValidator.FirstNameConverter, recordValidator.FirstNameValidator);
                    Console.Write("Last name:");
                    fileCabinetRecord.LastName = recordValidator.ReadInput(recordValidator.LastNameConverter, recordValidator.LastNameValidator);
                    Console.Write("Date of birth:");
                    fileCabinetRecord.DateOfBirth = recordValidator.ReadInput(recordValidator.DayOfBirthConverter, recordValidator.DayOfBirthValidator);
                    Console.Write("Person's sex:");
                    fileCabinetRecord.Sex = recordValidator.ReadInput(recordValidator.SexConverter, recordValidator.SexValidator);
                    Console.Write("Person's height:");
                    fileCabinetRecord.Height = recordValidator.ReadInput(recordValidator.HeightConverter, recordValidator.HeightValidator);
                    Console.Write("Person's salary:");
                    fileCabinetRecord.Salary = recordValidator.ReadInput(recordValidator.SalaryConverter, recordValidator.SalaryValidator);
                    fileCabinetService.EditRecord(fileCabinetRecord);
                    Console.WriteLine($"Record #{editInputId} is updated");
                }
                catch (ArgumentNullException ex)
                {
                    Console.WriteLine($"{ex.Message} Enter the data again!");
                    Edit(parameters);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"{ex.Message} Enter the data again!");
                    Edit(parameters);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw new ArgumentException("Input value in Edit is incorrect! Select command again.");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }

        private static void Find(string parameters)
        {
            CultureInfo provider = new CultureInfo("en-US");
            var parametersArray = parameters.ToLower(provider).Split(' ', 2);
            string searchParametr = parametersArray[0];
            string value = parametersArray[1];
            if (searchParametr == "firstname")
            {
                var firstName = parametersArray[1].Trim('"');
                var records = fileCabinetService.FindByFirstName(firstName);
                PrintRecords(records, value);
            }
            else if (searchParametr == "lastname")
            {
                var lastName = parametersArray[1].Trim('"');
                var records = fileCabinetService.FindByLastName(lastName);
                PrintRecords(records, value);
            }
            else if (searchParametr == "dateofbirth")
            {
                var dateofbirth = parametersArray[1].Trim('"');
                var records = fileCabinetService.FindByDateOfBirth(dateofbirth);
                PrintRecords(records, value);
            }
            else
            {
                Console.WriteLine($"There is no '{parametersArray[0]}' parametr.");
            }
        }

        private static void PrintRecords(ReadOnlyCollection<FileCabinetRecord> records, string value)
        {
            CultureInfo provider = new CultureInfo("en-US");
            if (records.Count == 0)
            {
                Console.WriteLine($"No records are found for firstName = {value}!");
            }
            else
            {
                foreach (var record in records)
                {
                    Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth.ToString("yyyy-MMM-dd", provider)}, {record.Sex}, {record.Height}, {record.Salary}");
                }
            }
        }

        private class Options
        {
            [Option('v', "validation-rules", Separator = '=', HelpText = "Set output to verbose messages.")]
            public string InputFile { get; set; }
        }
    }
}