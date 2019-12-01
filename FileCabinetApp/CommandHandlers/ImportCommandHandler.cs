using System;
using System.Globalization;
using System.IO;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Import command.
    /// </summary>
    public class ImportCommandHandler : ServiceCommandHandlerBase
    {
        private static IValidatorOfParemetrs inputParamsValidator;
        private static Action<string> action;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Input parametr start id.<see cref="IFileCabinetService"/>.</param>
        /// <param name="recordValidator">Input parametr amount of records.<see cref="IValidatorOfParemetrs"/>.</param>
        /// <param name="printMethod">Input delegate Action for print messages.<see cref="Action"/>.</param>
        public ImportCommandHandler(IFileCabinetService fileCabinetService, IValidatorOfParemetrs recordValidator, Action<string> printMethod)
            : base(fileCabinetService)
        {
            inputParamsValidator = recordValidator ?? throw new ArgumentNullException(nameof(recordValidator));
            action = printMethod ?? throw new ArgumentNullException(nameof(printMethod));
        }

        /// <summary>
        /// Override method Handle by CommandHandlerBase in ImportCommandHandler.
        /// </summary>
        /// <param name="appCommandRequest">Input parametr record <see cref="AppCommandRequest"/>.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest is null)
            {
                throw new ArgumentNullException(nameof(appCommandRequest));
            }

            if (appCommandRequest.Command == "import")
            {
                var parameters = appCommandRequest.Parameters;
                Import(parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private static void Import(string parameters)
        {
            try
            {
                try
                {
                    CultureInfo provider = new CultureInfo("en-US");

                    var parametersArray = parameters.Split(' ', 2);
                    if (parameters.Length == 0)
                    {
                        action($"export command is empty!");
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
                                    action($"Import error: file {thePathToTheFile}. is not exist. -Exit command import.");
                                    return;
                                }

                                ReadRecocdFileCvsOrXml(searchParametr, thePathToTheFile);
                            }
                        }
                        else
                        {
                            action($"The type of file {extension} does not match import type : {searchParametr}.-Exit command import.");
                        }
                    }
                    else
                    {
                        action($"Incorrect (is not csv or xml) type of file - {searchParametr}.-Exit command import.");
                    }
                }
                catch (Exception)
                {
                    throw new ArgumentException("Incorrect input data.- Exit command import.");
                }
            }
            catch (ArgumentException ex)
            {
                action(ex.Message);
            }
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
            try
            {
                StreamReader streamReaderFromCSV = null;
                try
                {
                    streamReaderFromCSV = new StreamReader(thePathToTheFile, System.Text.Encoding.Default);
                    service.Validator = inputParamsValidator;
                    var snapshotFileCabinetService = service.MakeSnapshot();
                    snapshotFileCabinetService.LoadFromCsv(streamReaderFromCSV);
                    action($"{snapshotFileCabinetService.Records.Count} record(s) were imported from {thePathToTheFile}.");
                    var result = service.Restore(snapshotFileCabinetService);
                    action($"{result} records were add to {service.GetType()}.");
                }
                catch (ArgumentException ex)
                {
                    action(ex.Message);
                }
                catch (Exception ex)
                {
                    action(ex.Message);
                    throw new ArgumentException("Incorrect data in CSV.- Exit command import.");
                }
                finally
                {
                    if (streamReaderFromCSV != null)
                    {
                        streamReaderFromCSV.Close();
                    }
                }
            }
            catch (ArgumentException ex)
            {
                action(ex.Message);
            }
        }

        private static void StreamReaderRecordFromXML(string thePathToTheFile)
        {
            try
            {
                StreamReader streamReaderFromXML = null;
                try
                {
                    streamReaderFromXML = new StreamReader(thePathToTheFile, System.Text.Encoding.Default);
                    service.Validator = inputParamsValidator;
                    var snapshotFileCabinetService = service.MakeSnapshot();
                    snapshotFileCabinetService.LoadFromXML(streamReaderFromXML);
                    action($"{snapshotFileCabinetService.Records.Count} record(s) were imported from {thePathToTheFile}.");
                    var result = service.Restore(snapshotFileCabinetService);
                    action($"{result} records were add to {service.GetType()}");
                }
                catch (Exception ex)
                {
                    action(ex.Message);
                    throw new ArgumentException("Incorrect data in XML.- Exit command import.");
                }
                finally
                {
                    if (streamReaderFromXML != null)
                    {
                        streamReaderFromXML.Close();
                    }
                }
            }
            catch (ArgumentException ex)
            {
                action(ex.Message);
            }
        }
    }
}
