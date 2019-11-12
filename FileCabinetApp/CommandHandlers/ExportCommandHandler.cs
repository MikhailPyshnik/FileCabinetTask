using System;
using System.Globalization;
using System.IO;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Export command.
    /// </summary>
    public class ExportCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Override method Handle by CommandHandlerBase in ExportCommandHandler.
        /// </summary>
        /// <param name="appCommandRequest">Input parametr record <see cref="AppCommandRequest"/>.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest == null)
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
    }
}
