using System;
using System.Globalization;
using System.IO;
using CommandLine;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Class of console  FileCabinetGenerator application.
    /// </summary>
    public static class Program
    {
        private static string typeOfFile = "csv";
        private static string inputfilename = "default";
        private static uint amountRecords = 1;
        private static uint startIdToRecords = 1;

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
                Console.WriteLine("Not parsed command!");
                PrintDefault();
            }
            else
            {
                ValidateInputRules(options.InputTypeFile, options.InputFileName, options.InputIdAmount, options.InputIdStart);
            }

            Console.ReadLine();
        }

        private static void ValidateInputRules(string inputTypeFile, string inputFileName, string inputIdAmount, string inputIdStart)
        {
            if (inputTypeFile != null && inputFileName != null && inputIdAmount != null && inputIdStart != null)
            {
                CultureInfo provider = new CultureInfo("en-US");
                string searchParametr = inputTypeFile.ToLower(provider);
                if (searchParametr == "xml" || searchParametr == "csv")
                {
                    if (searchParametr == "xml")
                    {
                        typeOfFile = "xml";
                    }
                }
                else
                {
                    Console.WriteLine($"The incorrect type of file. Default value:{typeOfFile}.");
                }

                string thePathToTheFile = inputFileName;
                inputfilename = inputFileName;
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

                            if (!uint.TryParse(inputIdAmount, out amountRecords))
                            {
                                Console.WriteLine($"The incorrect start id. Default value:{amountRecords}.");
                            }

                            if (!uint.TryParse(inputIdStart, out startIdToRecords))
                            {
                                Console.WriteLine($"The incorrect start id. Default value:{startIdToRecords}.");
                            }

                            ExportFile(inputFileName, startIdToRecords, amountRecords);
                        }
                        else
                        {
                            var inputDirectoryName = Path.GetDirectoryName(thePathToTheFile);
                            if (inputDirectoryName.Length == 0)
                            {
                                if (!uint.TryParse(inputIdAmount, out amountRecords))
                                {
                                    Console.WriteLine($"The incorrect start id. Default value:{amountRecords}.");
                                }

                                if (!uint.TryParse(inputIdStart, out startIdToRecords))
                                {
                                    Console.WriteLine($"The incorrect start id. Default value:{startIdToRecords}.");
                                }

                                ExportFile(inputFileName, startIdToRecords, amountRecords);
                            }

                            bool containsGetDirectoryName = Directory.Exists(inputDirectoryName);
                            if (!containsGetDirectoryName)
                            {
                                PrintDefault();
                                return;
                            }

                            if (!uint.TryParse(inputIdAmount, out amountRecords))
                            {
                                Console.WriteLine($"The incorrect start id. Default value:{amountRecords}.");
                            }

                            if (!uint.TryParse(inputIdStart, out startIdToRecords))
                            {
                                Console.WriteLine($"The incorrect start id. Default value:{startIdToRecords}.");
                            }

                            ExportFile(inputFileName, startIdToRecords, amountRecords);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"The type of file {extension} does not match export type : {thePathToTheFile}.");
                        PrintDefault();
                        return;
                    }
                }
                else
                {
                    Console.WriteLine($"Incorrect  (is not csv or xml) type of file - {searchParametr}.");
                    PrintDefault();
                    return;
                }

                Console.WriteLine($"{amountRecords} records were written to {inputfilename}.");
            }
            else
            {
                PrintDefault();
            }
        }

        private static void PrintDefault()
        {
            Console.WriteLine($"The rules is incorrect. Default value: {typeOfFile} - {inputfilename} - {amountRecords} - {startIdToRecords}.");
        }

        private static void ExportFile(string inputFileName, uint inputIdStart, uint inputIdAmount)
        {
            Generator generator = new Generator(inputIdStart, inputIdAmount);
            if (typeOfFile == "xml")
            {
                generator.ImportToXML(inputFileName);
            }
            else
            {
                generator.ImportToCSV(inputFileName);
            }
        }

        private class Options
        {
            [Option('t', "output-type", Separator = '=', HelpText = "Output format type (csv, xml).")]
            public string InputTypeFile { get; set; }

            [Option('o', "output", Separator = '=', HelpText = "Output file name.")]
            public string InputFileName { get; set; }

            [Option('a', "records-amount", Separator = '=', HelpText = "Amount of generated records.")]
            public string InputIdAmount { get; set; }

            [Option('i', "start-id", Separator = '=', HelpText = "ID value to start.")]
            public string InputIdStart { get; set; }
        }
    }
}
