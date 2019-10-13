﻿using System;
using System.Globalization;
using System.Threading;

namespace FileCabinetApp
{
    public static class Program
    {
        private const string DeveloperName = "Mikhail Pyshnik";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;
        private static FileCabinetService fileCabinetService = new FileCabinetService();

        private static bool isRunning = true;

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "stat of the record", "The 'stat' command return info about the record." },
            new string[] { "create", "create a new record", "The 'create' command add info int the service." },
            new string[] { "list", "list return a copy of record", "The 'list' command return copy all records." },
        };

        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
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
                CultureInfo provider = new CultureInfo("en-US");
                Console.Write("First name:");
                string inputFirstName = Console.ReadLine();
                Console.Write("Last name:");
                string inputLastName = Console.ReadLine();
                Console.Write("Date of birth:");
                string inputDateOfBirth = Console.ReadLine();
                DateTime dateOfBirth = DateTime.ParseExact(inputDateOfBirth, "dd/MM/yyyy", provider);
                Console.Write("Person's sex:");
                char inputSex = Convert.ToChar(Console.ReadLine(), provider);
                Console.Write("Person's height:");
                short inputHeight = Convert.ToInt16(Console.ReadLine(), provider);
                Console.Write("Person's salary:");
                decimal inputSalary = Convert.ToDecimal(Console.ReadLine(), provider);
                Console.WriteLine($"Record #{fileCabinetService.CreateRecord(inputFirstName, inputLastName, dateOfBirth, inputSex, inputHeight, inputSalary)} is created.");
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"{ex.Message} - enter the data again!");
                Create(parameters);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"{ex.Message} - enter the data again!");
                Create(parameters);
            }
            catch (Exception)
            {
                Console.WriteLine("Input value is incorrect! Enter the data again!");
                Create(parameters);
                throw;
            }
        }

        private static void List(string parameters)
        {
            CultureInfo provider = new CultureInfo("en-US");
            var reultList = fileCabinetService.GetRecords();
            if (reultList.Length == 0)
            {
                Console.Write("The list is empty.Add new record => add command - create");
            }
            else
            {
                for (int i = 0; i < reultList.Length; i++)
                {
                    Console.WriteLine($"#{reultList[i].Id},{reultList[i].FirstName},{reultList[i].LastName},{reultList[i].DateOfBirth.ToString("yyyy-MMM-dd", provider)}");
                }
            }
        }
    }
}