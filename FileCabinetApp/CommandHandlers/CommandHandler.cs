using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Work with commands.
    /// </summary>
    public class CommandHandler : CommandHandlerBase
    {
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

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
        /// Override method Handle by CommandHandlerBase in CommandHandler.
        /// </summary>
        /// <param name="appCommandRequest">Input parametr record <see cref="AppCommandRequest"/>.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest == null)
            {
                throw new ArgumentNullException(nameof(appCommandRequest));
            }

            var comand = appCommandRequest.Command;
            var parameters = appCommandRequest.Parameters;

            switch (comand)
            {
                case "help":
                PrintHelp(parameters);
                break;
                case "exit":
                Exit(parameters);
                break;
                case "stat":
                Stat(parameters);
                break;
                case "create":
                Create(parameters);
                break;
                case "list":
                List(parameters);
                break;
                case "edit":
                Edit(parameters);
                break;
                case "find":
                Find(parameters);
                break;
                case "export":
                Export(parameters);
                break;
                case "import":
                Import(parameters);
                break;
                case "remove":
                Remove(parameters);
                break;
                case "purge":
                Purge(parameters);
                break;
                default:
                PrintMissedCommandInfo(comand);
                break;
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

        private static void Exit(string parameters)
        {
            if (Program.fileCabinetService is FileCabinetFilesystemService)
            {
                Program.filestream.Close();
            }

            Console.WriteLine("Exiting an application...");
            Program.isRunning = false;
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

        private static void Create(string parameters)
        {
            try
            {
                try
                {
                    FileCabinetRecord fileCabinetRecord = new FileCabinetRecord();
                    CultureInfo provider = new CultureInfo("en-US");
                    Console.Write("First name:");
                    fileCabinetRecord.FirstName = Program.recordValidator.ReadInput(Program.recordValidator.FirstNameConverter, Program.recordValidator.FirstNameValidator);
                    Console.Write("Last name:");
                    fileCabinetRecord.LastName = Program.recordValidator.ReadInput(Program.recordValidator.LastNameConverter, Program.recordValidator.LastNameValidator);
                    Console.Write("Date of birth:");
                    fileCabinetRecord.DateOfBirth = Program.recordValidator.ReadInput(Program.recordValidator.DayOfBirthConverter, Program.recordValidator.DayOfBirthValidator);
                    Console.Write("Person's sex:");
                    fileCabinetRecord.Sex = Program.recordValidator.ReadInput(Program.recordValidator.SexConverter, Program.recordValidator.SexValidator);
                    Console.Write("Person's height:");
                    fileCabinetRecord.Height = Program.recordValidator.ReadInput(Program.recordValidator.HeightConverter, Program.recordValidator.HeightValidator);
                    Console.Write("Person's salary:");
                    fileCabinetRecord.Salary = Program.recordValidator.ReadInput(Program.recordValidator.SalaryConverter, Program.recordValidator.SalaryValidator);
                    int res = Program.fileCabinetService.CreateRecord(fileCabinetRecord);
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
            var reultList = Program.fileCabinetService.GetRecords();
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
                    if (Program.fileCabinetService.GetStat().Item1 < 1)
                    {
                        Console.WriteLine("File is not empty.");
                        return;
                    }

                    var records = Program.fileCabinetService.GetRecords();
                    List<FileCabinetRecord> listValidRecords = new List<FileCabinetRecord>(records);
                    if (listValidRecords.Exists(item => item.Id == editInputId))
                    {
                        FileCabinetRecord fileCabinetRecord = new FileCabinetRecord();

                        fileCabinetRecord.Id = editInputId;
                        Console.Write("First name:");
                        fileCabinetRecord.FirstName = Program.recordValidator.ReadInput(Program.recordValidator.FirstNameConverter, Program.recordValidator.FirstNameValidator);
                        Console.Write("Last name:");
                        fileCabinetRecord.LastName = Program.recordValidator.ReadInput(Program.recordValidator.LastNameConverter, Program.recordValidator.LastNameValidator);
                        Console.Write("Date of birth:");
                        fileCabinetRecord.DateOfBirth = Program.recordValidator.ReadInput(Program.recordValidator.DayOfBirthConverter, Program.recordValidator.DayOfBirthValidator);
                        Console.Write("Person's sex:");
                        fileCabinetRecord.Sex = Program.recordValidator.ReadInput(Program.recordValidator.SexConverter, Program.recordValidator.SexValidator);
                        Console.Write("Person's height:");
                        fileCabinetRecord.Height = Program.recordValidator.ReadInput(Program.recordValidator.HeightConverter, Program.recordValidator.HeightValidator);
                        Console.Write("Person's salary:");
                        fileCabinetRecord.Salary = Program.recordValidator.ReadInput(Program.recordValidator.SalaryConverter, Program.recordValidator.SalaryValidator);
                        Program.fileCabinetService.EditRecord(fileCabinetRecord);
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
                var records = Program.fileCabinetService.FindByFirstName(firstName);
                PrintRecords(records, searchParametr, value);
            }
            else if (searchParametr == "lastname")
            {
                var lastName = parametersArray[1].Trim('"');
                var records = Program.fileCabinetService.FindByLastName(lastName);
                PrintRecords(records, searchParametr, value);
            }
            else if (searchParametr == "dateofbirth")
            {
                var dateofbirth = parametersArray[1].Trim('"');
                var records = Program.fileCabinetService.FindByDateOfBirth(dateofbirth);
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
            if (Program.fileCabinetService.GetStat().Item1 < 1)
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

            List<FileCabinetRecord> list = new List<FileCabinetRecord>(Program.fileCabinetService.GetRecords());

            bool isExists = list.Exists(record => record.Id == validId);

            if (result && isExists)
            {
                Program.fileCabinetService.Remove(validId);
                Console.WriteLine($"Record #{inputId} is removed.");
            }
            else
            {
                Console.WriteLine($"Record #{inputId} doesn't exists.");
            }
        }

        private static void Purge(string parameters)
        {
            if (Program.fileCabinetService is FileCabinetFilesystemService)
            {
                Program.fileCabinetService.Purge();
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
                var snapshotFileCabinetService = Program.fileCabinetService.MakeSnapshot();
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
                var snapshotFileCabinetService = Program.fileCabinetService.MakeSnapshot();
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
                Program.fileCabinetService.Validator = Program.recordValidator;
                var snapshotFileCabinetService = Program.fileCabinetService.MakeSnapshot();
                snapshotFileCabinetService.LoadFromCsv(streamReaderFromCSV);
                Console.WriteLine($"{snapshotFileCabinetService.Records.Count} record(s) were imported from {thePathToTheFile}.");
                Program.fileCabinetService.Restore(snapshotFileCabinetService);
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
                Program.fileCabinetService.Validator = Program.recordValidator;
                var snapshotFileCabinetService = Program.fileCabinetService.MakeSnapshot();
                snapshotFileCabinetService.LoadFromXML(streamReaderFromXML);
                Console.WriteLine($"{snapshotFileCabinetService.Records.Count} record(s) were imported from {thePathToTheFile}.");
                Program.fileCabinetService.Restore(snapshotFileCabinetService);
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

        private static ICommandHandler CreateCommandHandlers()
        {
            var commandHandler = new CommandHandler();
            return commandHandler;
        }
    }
}
