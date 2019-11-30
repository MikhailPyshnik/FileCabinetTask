using System;
using System.Globalization;
using System.IO;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Export command.
    /// </summary>
    public class ExportCommandHandler : ServiceCommandHandlerBase
    {
        private static Action<string> action;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Input parametr start id.<see cref="IFileCabinetService"/>.</param>
        /// <param name="printMethod">Input delegate Action for print messages.<see cref="Action"/>.</param>
        public ExportCommandHandler(IFileCabinetService fileCabinetService, Action<string> printMethod)
            : base(fileCabinetService)
        {
            action = printMethod ?? throw new ArgumentNullException(nameof(printMethod));
        }

        /// <summary>
        /// Override method Handle by CommandHandlerBase in ExportCommandHandler.
        /// </summary>
        /// <param name="appCommandRequest">Input parametr record <see cref="AppCommandRequest"/>.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest is null)
            {
                throw new ArgumentNullException(nameof(appCommandRequest));
            }

            if (appCommandRequest.Command == "export")
            {
                var parameters = appCommandRequest.Parameters;
                Export(parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private static void Export(string parameters)
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
                        action($"File is exist - rewrite {thePathToTheFile}?[Y / n] ");
                        var inputs = Console.ReadLine().ToLower(provider);
                        char charComnandYorN;
                        bool charComnandBool = char.TryParse(inputs, out charComnandYorN);
                        if (!'y'.Equals(charComnandYorN) & !'n'.Equals(charComnandYorN))
                        {
                            action($"Incorrcect command [Y / n] : {inputs}!");
                            return;
                        }

                        if (!charComnandBool)
                        {
                            action($"Incorrcect command [Y / n] : inputs not char - {inputs}");
                            return;
                        }

                        if (charComnandYorN == 'n')
                        {
                            action($"Command - n.-Exit command export.");
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
                            action($"Export failed: can't open file {thePathToTheFile}.-Exit command export.");
                            return;
                        }

                        StreamWriterRecocdFileCvsOrXml(searchParametr, thePathToTheFile);
                    }
                }
                else
                {
                    action($"The type of file {extension} does not match export type : {searchParametr}.-Exit command export.");
                }
            }
            else
            {
                action($"Incorrect  (is not csv or xml) type of file - {searchParametr}.-Exit command export.");
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
                var snapshotFileCabinetService = service.MakeSnapshot();
                snapshotFileCabinetService.SaveToCsv(streamWriterToCsv);
                action($"All records are exported to file {thePathToTheFile}.");
            }
            catch (Exception ex)
            {
                action(ex.Message);
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
                var snapshotFileCabinetService = service.MakeSnapshot();
                snapshotFileCabinetService.SaveToXml(streamWriterToCsv);
            }
            catch (Exception ex)
            {
                action(ex.Message);
                throw new Exception(ex.Message);
            }
            finally
            {
                if (streamWriterToCsv != null)
                {
                    streamWriterToCsv.Close();
                }
            }

            action($"All records are exported to file {thePathToTheFile}.");
        }
    }
}
