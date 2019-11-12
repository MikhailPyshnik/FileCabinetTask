using System;
using System.Globalization;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Create command.
    /// </summary>
    public class CreateCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Override method Handle by CommandHandlerBase in CreateCommandHandler.
        /// </summary>
        /// <param name="appCommandRequest">Input parametr record <see cref="AppCommandRequest"/>.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest == null)
            {
                throw new ArgumentNullException(nameof(appCommandRequest));
            }

            if (appCommandRequest.Command == "create")
            {
                var parameters = appCommandRequest.Parameters;
                Create(parameters);
            }
            else
            {
                    base.Handle(appCommandRequest);
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
    }
}
