using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
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
        private static FileStream filestream;
        private static IFileCabinetService fileCabinetService;
        private static IValidatorOfParemetrs recordValidator;

        private static string validationRules = "default";

        private static string storageRules = "memory";

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
            new Tuple<string, Action<string>>("export", Export),
            new Tuple<string, Action<string>>("import", Import),
            new Tuple<string, Action<string>>("remove", Remove),
            new Tuple<string, Action<string>>("purge", Purge),
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
            new string[] { "export", "export file", "The 'export' export records to file." },
            new string[] { "import", "import file", "The 'import' import type of file." },
            new string[] { "remove", "remove record by id", "The 'remove' delete record." },
            new string[] { "purge", "purge records in FileCabinetFilesystemService", "The 'purge' records in FileCabinetFilesystemService." },
        };

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
            filestream.Close();
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }

        private static void Stat(string parameters)
        {
            if (fileCabinetService is FileCabinetFilesystemService)
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
                    int res = fileCabinetService.CreateRecord(fileCabinetRecord);
                    if (res > 0)
                    {
                        Console.WriteLine($"Record #{res} is created.");
                    }
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
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
                    CultureInfo provider = new CultureInfo("en-US");
                    int editInputId = Convert.ToInt32(parameters, provider);
                    if (fileCabinetService.GetStat().Item1 < 1)
                    {
                        Console.WriteLine("File is not empty.");
                        return;
                    }

                    var records = fileCabinetService.GetRecords();
                    List<FileCabinetRecord> listValidRecords = new List<FileCabinetRecord>(records);
                    if (listValidRecords.Exists(item => item.Id == editInputId))
                    {
                        FileCabinetRecord fileCabinetRecord = new FileCabinetRecord();

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
                    else
                    {
                        Console.WriteLine($"#{editInputId} record is not found.");
                    }
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
                PrintRecords(records, searchParametr, value);
            }
            else if (searchParametr == "lastname")
            {
                var lastName = parametersArray[1].Trim('"');
                var records = fileCabinetService.FindByLastName(lastName);
                PrintRecords(records, searchParametr, value);
            }
            else if (searchParametr == "dateofbirth")
            {
                var dateofbirth = parametersArray[1].Trim('"');
                var records = fileCabinetService.FindByDateOfBirth(dateofbirth);
                PrintRecords(records, searchParametr, value);
            }
            else
            {
                Console.WriteLine($"There is no '{parametersArray[0]}' parametr.");
            }
        }

        private static void PrintRecords(ReadOnlyCollection<FileCabinetRecord> records, string searchparametr, string value)
        {
            CultureInfo provider = new CultureInfo("en-US");
            if (records.Count == 0)
            {
                Console.WriteLine($"No records are found for {searchparametr} = {value}!");
            }
            else
            {
                foreach (var record in records)
                {
                    Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth.ToString("yyyy-MMM-dd", provider)}, {record.Sex}, {record.Height}, {record.Salary}");
                }
            }
        }

        private static void Export(string parameters)
        {
            CultureInfo provider = new CultureInfo("en-US");

            var parametersArray = parameters.Split(' ', 2);
            if (parameters.Length == 0)
            {
                Console.WriteLine($"export command is empty!");
                return;
            }

            string searchParametr = parametersArray[0].ToLower(provider);
            string thePathToTheFile = parametersArray[1];
            var extension = Path.GetExtension(thePathToTheFile);
            extension = extension.Remove(0, 1);
            if (searchParametr == "csv" || searchParametr == "xml")
            {
                if ((searchParametr == "csv" && extension == "csv") || (searchParametr == "xml" && extension == "xml"))
                {
                    bool containsFile = File.Exists(thePathToTheFile);
                    if (containsFile)
                    {
                        Console.Write($"File is exist - rewrite {thePathToTheFile}?[Y / n] ");
                        var inputs = Console.ReadLine().ToLower(provider);
                        char charComnandYorN;
                        bool charComnandBool = char.TryParse(inputs, out charComnandYorN);
                        if (!'y'.Equals(charComnandYorN) & !'n'.Equals(charComnandYorN))
                        {
                            Console.WriteLine($"Incorrcect command [Y / n] : {inputs}!");
                            return;
                        }

                        if (!charComnandBool)
                        {
                            Console.WriteLine($"Incorrcect command [Y / n] : inputs not char - {inputs}");
                            return;
                        }

                        if (charComnandYorN == 'n')
                        {
                            Console.WriteLine($"Command - n.-Exit command export.");
                            return;
                        }

                        StreamWriterRecocdFileCvsOrXml(searchParametr, thePathToTheFile);
                    }
                    else
                    {
                        var inputDirectoryName = Path.GetDirectoryName(thePathToTheFile);
                        if (inputDirectoryName.Length == 0)
                        {
                            StreamWriterRecocdFileCvsOrXml(searchParametr, thePathToTheFile);
                            return;
                        }

                        bool containsGetDirectoryName = Directory.Exists(inputDirectoryName);
                        if (!containsGetDirectoryName)
                        {
                            Console.WriteLine($"Export failed: can't open file {thePathToTheFile}.-Exit command export.");
                            return;
                        }

                        StreamWriterRecocdFileCvsOrXml(searchParametr, thePathToTheFile);
                    }
                }
                else
                {
                    Console.WriteLine($"The type of file {extension} does not match export type : {searchParametr}.-Exit command export.");
                }
            }
            else
            {
                Console.WriteLine($"Incorrect  (is not csv or xml) type of file - {searchParametr}.-Exit command export.");
            }
        }

        private static void Import(string parameters)
        {
            CultureInfo provider = new CultureInfo("en-US");

            var parametersArray = parameters.Split(' ', 2);
            if (parameters.Length == 0)
            {
                Console.WriteLine($"export command is empty!");
                return;
            }

            string searchParametr = parametersArray[0].ToLower(provider);
            string thePathToTheFile = parametersArray[1];
            var extension = Path.GetExtension(thePathToTheFile);
            extension = extension.Remove(0, 1);
            if (searchParametr == "csv" || searchParametr == "xml")
            {
                if ((searchParametr == "csv" && extension == "csv") || (searchParametr == "xml" && extension == "xml"))
                {
                    bool containsFile = File.Exists(thePathToTheFile);
                    if (containsFile)
                    {
                        ReadRecocdFileCvsOrXml(searchParametr, thePathToTheFile);
                    }
                    else
                    {
                        var inputDirectoryName = Path.GetDirectoryName(thePathToTheFile);
                        if (inputDirectoryName.Length == 0)
                        {
                            ReadRecocdFileCvsOrXml(searchParametr, thePathToTheFile);
                            return;
                        }

                        bool containsGetDirectoryName = Directory.Exists(inputDirectoryName);
                        if (!containsGetDirectoryName)
                        {
                            Console.WriteLine($"Import error: file {thePathToTheFile}. is not exist. -Exit command import.");
                            return;
                        }

                        ReadRecocdFileCvsOrXml(searchParametr, thePathToTheFile);
                    }
                }
                else
                {
                    Console.WriteLine($"The type of file {extension} does not match import type : {searchParametr}.-Exit command import.");
                }
            }
            else
            {
                Console.WriteLine($"Incorrect (is not csv or xml) type of file - {searchParametr}.-Exit command import.");
            }
        }

        private static void Remove(string parameters)
        {
            if (fileCabinetService.GetStat().Item1 < 1)
            {
                Console.WriteLine("File is not empty.");
                return;
            }

            var parametersArray = parameters.Split(' ', 1);
            if (parameters.Length == 0)
            {
                Console.WriteLine("remove command is empty!");
                return;
            }

            if (parametersArray.Length > 1)
            {
                Console.WriteLine("Incorrect value input in comand remove!");
                return;
            }

            string inputId = parametersArray[0];
            int validId;
            bool result = int.TryParse(inputId, out validId);

            List<FileCabinetRecord> list = new List<FileCabinetRecord>(fileCabinetService.GetRecords());

            bool isExists = list.Exists(record => record.Id == validId);

            if (result && isExists)
            {
                fileCabinetService.Remove(validId);
                Console.WriteLine($"Record #{inputId} is removed.");
            }
            else
            {
                Console.WriteLine($"Record #{inputId} doesn't exists.");
            }
        }

        private static void Purge(string parameters)
        {
            if (fileCabinetService is FileCabinetFilesystemService)
            {
                fileCabinetService.Purge();
            }
            else
            {
                Console.WriteLine("fileCabinetService is not FileCabinetFilesystemService.");
            }
        }

        private static void StreamWriterRecocdFileCvsOrXml(string searchParametr, string thePathToTheFile)
        {
            if (searchParametr == "csv")
            {
                StreamWriterRecocdFileToCSV(thePathToTheFile);
            }
            else if (searchParametr == "xml")
            {
                StreamWriterRecocdFileToXML(thePathToTheFile);
            }
        }

        private static void StreamWriterRecocdFileToCSV(string thePathToTheFile)
        {
            StreamWriter streamWriterToCsv = null;
            try
            {
                streamWriterToCsv = new StreamWriter(thePathToTheFile, false, System.Text.Encoding.Default);
                var snapshotFileCabinetService = fileCabinetService.MakeSnapshot();
                snapshotFileCabinetService.SaveToCsv(streamWriterToCsv);
                Console.WriteLine($"All records are exported to file {thePathToTheFile}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new ArgumentException($"{ex.Message}");
            }
            finally
            {
                if (streamWriterToCsv != null)
                {
                    streamWriterToCsv.Close();
                }
            }
        }

        private static void StreamWriterRecocdFileToXML(string thePathToTheFile)
        {
            StreamWriter streamWriterToCsv = null;
            try
            {
                streamWriterToCsv = new StreamWriter(thePathToTheFile, false, System.Text.Encoding.Default);
                var snapshotFileCabinetService = fileCabinetService.MakeSnapshot();
                snapshotFileCabinetService.SaveToXml(streamWriterToCsv);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception(ex.Message);
            }
            finally
            {
                if (streamWriterToCsv != null)
                {
                    streamWriterToCsv.Close();
                }
            }

            Console.WriteLine($"All records are exported to file {thePathToTheFile}.");
        }

        private static void ReadRecocdFileCvsOrXml(string searchParametr, string thePathToTheFile)
        {
            if (searchParametr == "csv")
            {
                StreamReaderRecordFromCSV(thePathToTheFile);
            }
            else if (searchParametr == "xml")
            {
                StreamReaderRecordFromXML(thePathToTheFile);
            }
        }

        private static void StreamReaderRecordFromCSV(string thePathToTheFile)
        {
            StreamReader streamReaderFromCSV = null;
            try
            {
                streamReaderFromCSV = new StreamReader(thePathToTheFile, System.Text.Encoding.Default);
                fileCabinetService.Validator = recordValidator;
                var snapshotFileCabinetService = fileCabinetService.MakeSnapshot();
                snapshotFileCabinetService.LoadFromCsv(streamReaderFromCSV);
                Console.WriteLine($"{snapshotFileCabinetService.Records.Count} record(s) were imported from {thePathToTheFile}.");
                fileCabinetService.Restore(snapshotFileCabinetService);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new ArgumentException($"{ex.Message}");
            }
            finally
            {
                if (streamReaderFromCSV != null)
                {
                    streamReaderFromCSV.Close();
                }
            }
        }

        private static void StreamReaderRecordFromXML(string thePathToTheFile)
        {
            StreamReader streamReaderFromXML = null;
            try
            {
                streamReaderFromXML = new StreamReader(thePathToTheFile, System.Text.Encoding.Default);
                fileCabinetService.Validator = recordValidator;
                var snapshotFileCabinetService = fileCabinetService.MakeSnapshot();
                snapshotFileCabinetService.LoadFromXML(streamReaderFromXML);
                Console.WriteLine($"{snapshotFileCabinetService.Records.Count} record(s) were imported from {thePathToTheFile}.");
                fileCabinetService.Restore(snapshotFileCabinetService);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new ArgumentException($"{ex.Message}");
            }
            finally
            {
                if (streamReaderFromXML != null)
                {
                    streamReaderFromXML.Close();
                }
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