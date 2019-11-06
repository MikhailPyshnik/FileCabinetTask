using CommandLine;
using System;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace FileCabinetGenerator
{
    class Program
    {
        private static string typeOfFile = "csv";
        private static string inputfilename = "default";
        private static uint amountRecords = 1;
        private static uint startIdToRecords = 1;


        static void Main(string[] args)
        {
            var options = new Options();
            var result = Parser.Default
                               .ParseArguments<Options>(args)
                               .WithParsed(parsed => options = parsed);
            if (result.Tag == ParserResultType.NotParsed)
            {
                Console.WriteLine($"Not parsed command!");
                PrintDefault();
            }
            else
            {
                ValidateInputRules(options.InputFile, options.InputFileName, options.InputAmount, options.InputIdStast);
                Console.ReadLine();
            }
        }

        private static void ValidateInputRules(string s1, string s2,string s3, string s4)
        {
            if (s1 != null && s2 != null && s3 != null && s4 != null)
            {
                CultureInfo provider = new CultureInfo("en-US");
                string searchParametr = s1.ToLower(provider);
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
                string thePathToTheFile = s2;
                inputfilename = s2;
                var extension = Path.GetExtension(thePathToTheFile);
                extension = extension.Remove(0, 1);
                if (searchParametr == "csv" || searchParametr == "xml")
                {
                    if ((searchParametr == "csv" && extension == "csv") || (searchParametr == "xml" && extension == "xml"))
                    {
                        bool containsFile = File.Exists(thePathToTheFile);
                        if (containsFile)
                        {
                            
                        }
                        else
                        {
                            var inputDirectoryName = Path.GetDirectoryName(thePathToTheFile);
                            if (inputDirectoryName.Length == 0)
                            {

                            }

                            bool containsGetDirectoryName = Directory.Exists(inputDirectoryName);
                            if (!containsGetDirectoryName)
                            {

                            }
                            inputfilename = thePathToTheFile;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"The type of file {extension} does not match export type : {thePathToTheFile}.");
                        PrintDefault();
                    }
                }
                else
                {
                    Console.WriteLine($"Incorrect  (is not csv or xml) type of file - {searchParametr}.");
                    PrintDefault();
                    return;
                }

                if (!UInt32.TryParse(s3, out amountRecords))
                {
                    Console.WriteLine($"The incorrect start id. Default value:{amountRecords}.");
                }

                if (!UInt32.TryParse(s4, out startIdToRecords))
                {
                    Console.WriteLine($"The incorrect start id. Default value:{startIdToRecords}.");
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

        private class Options
        {
            [Option('t', "output-type", Separator = '=', HelpText = "Output format type (csv, xml).")]
            public string InputFile { get; set; }

            [Option('o', "output", Separator = '=', HelpText = "Output file name.")]
            public string InputFileName { get; set; }

            [Option('a', "records-amount", Separator = '=', HelpText = "Amount of generated records.")]
            public string InputAmount { get; set; }

            [Option('i', "start-id", Separator = '=', HelpText = "ID value to start.")]
            public string InputIdStast { get; set; }
        }
    }
}
