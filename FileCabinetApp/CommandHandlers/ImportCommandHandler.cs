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

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Input parametr start id.<see cref="IFileCabinetService"/>.</param>
        /// <param name="recordValidator">Input parametr amount of records.<see cref="IValidatorOfParemetrs"/>.</param>
        public ImportCommandHandler(IFileCabinetService fileCabinetService, IValidatorOfParemetrs recordValidator)
            : base(fileCabinetService)
        {
            inputParamsValidator = recordValidator ?? throw new ArgumentNullException(nameof(recordValidator));
        }

        /// <summary>
        /// Override method Handle by CommandHandlerBase in ImportCommandHandler.
        /// </summary>
        /// <param name="appCommandRequest">Input parametr record <see cref="AppCommandRequest"/>.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest == null)
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
                service.Validator = inputParamsValidator;
                var snapshotFileCabinetService = service.MakeSnapshot();
                snapshotFileCabinetService.LoadFromCsv(streamReaderFromCSV);
                Console.WriteLine($"{snapshotFileCabinetService.Records.Count} record(s) were imported from {thePathToTheFile}.");
                service.Restore(snapshotFileCabinetService);
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
                service.Validator = inputParamsValidator;
                var snapshotFileCabinetService = service.MakeSnapshot();
                snapshotFileCabinetService.LoadFromXML(streamReaderFromXML);
                Console.WriteLine($"{snapshotFileCabinetService.Records.Count} record(s) were imported from {thePathToTheFile}.");
                service.Restore(snapshotFileCabinetService);
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
    }
}
