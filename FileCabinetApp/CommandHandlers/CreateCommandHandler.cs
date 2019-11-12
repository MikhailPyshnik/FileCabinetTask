using System;
using System.Globalization;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Create command.
    /// </summary>
    public class CreateCommandHandler : CommandHandlerBase
    {
        private static IFileCabinetService service;
        private static IValidatorOfParemetrs inpuyParamsValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Input parametr start id.<see cref="IFileCabinetService"/>.</param>
        /// <param name="recordValidator">Input parametr amount of records.<see cref="IValidatorOfParemetrs"/>.</param>
        public CreateCommandHandler(IFileCabinetService fileCabinetService, IValidatorOfParemetrs recordValidator)
        {
            service = fileCabinetService;
            inpuyParamsValidator = recordValidator;
        }

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
                    fileCabinetRecord.FirstName = inpuyParamsValidator.ReadInput(inpuyParamsValidator.FirstNameConverter, inpuyParamsValidator.FirstNameValidator);
                    Console.Write("Last name:");
                    fileCabinetRecord.LastName = inpuyParamsValidator.ReadInput(inpuyParamsValidator.LastNameConverter, inpuyParamsValidator.LastNameValidator);
                    Console.Write("Date of birth:");
                    fileCabinetRecord.DateOfBirth = inpuyParamsValidator.ReadInput(inpuyParamsValidator.DayOfBirthConverter, inpuyParamsValidator.DayOfBirthValidator);
                    Console.Write("Person's sex:");
                    fileCabinetRecord.Sex = inpuyParamsValidator.ReadInput(inpuyParamsValidator.SexConverter, inpuyParamsValidator.SexValidator);
                    Console.Write("Person's height:");
                    fileCabinetRecord.Height = inpuyParamsValidator.ReadInput(inpuyParamsValidator.HeightConverter, inpuyParamsValidator.HeightValidator);
                    Console.Write("Person's salary:");
                    fileCabinetRecord.Salary = inpuyParamsValidator.ReadInput(inpuyParamsValidator.SalaryConverter, inpuyParamsValidator.SalaryValidator);
                    int result = service.CreateRecord(fileCabinetRecord);
                    if (result > 0)
                    {
                        Console.WriteLine($"Record #{result} is created.");
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
