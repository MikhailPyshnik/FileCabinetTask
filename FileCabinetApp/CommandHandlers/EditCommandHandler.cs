using System;
using System.Collections.Generic;
using System.Globalization;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Edit command.
    /// </summary>
    public class EditCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Override method Handle by CommandHandlerBase in EditCommandHandler.
        /// </summary>
        /// <param name="appCommandRequest">Input parametr record <see cref="AppCommandRequest"/>.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest == null)
            {
                throw new ArgumentNullException(nameof(appCommandRequest));
            }

            if (appCommandRequest.Command == "edit")
            {
                var parameters = appCommandRequest.Parameters;
                Edit(parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
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
    }
}
