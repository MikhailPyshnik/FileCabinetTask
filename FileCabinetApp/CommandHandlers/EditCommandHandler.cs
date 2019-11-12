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
        private static IFileCabinetService service;
        private static IValidatorOfParemetrs inputParamsValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Input parametr start id.<see cref="IFileCabinetService"/>.</param>
        /// <param name="recordValidator">Input parametr amount of records.<see cref="IValidatorOfParemetrs"/>.</param>
        public EditCommandHandler(IFileCabinetService fileCabinetService, IValidatorOfParemetrs recordValidator)
        {
            service = fileCabinetService;
            inputParamsValidator = recordValidator;
        }

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
                    if (service.GetStat().Item1 < 1)
                    {
                        Console.WriteLine("File is not empty.");
                        return;
                    }

                    var records = service.GetRecords();
                    List<FileCabinetRecord> listValidRecords = new List<FileCabinetRecord>(records);
                    if (listValidRecords.Exists(item => item.Id == editInputId))
                    {
                        FileCabinetRecord fileCabinetRecord = new FileCabinetRecord();

                        fileCabinetRecord.Id = editInputId;
                        Console.Write("First name:");
                        fileCabinetRecord.FirstName = inputParamsValidator.ReadInput(inputParamsValidator.FirstNameConverter, inputParamsValidator.FirstNameValidator);
                        Console.Write("Last name:");
                        fileCabinetRecord.LastName = inputParamsValidator.ReadInput(inputParamsValidator.LastNameConverter, inputParamsValidator.LastNameValidator);
                        Console.Write("Date of birth:");
                        fileCabinetRecord.DateOfBirth = inputParamsValidator.ReadInput(inputParamsValidator.DayOfBirthConverter, inputParamsValidator.DayOfBirthValidator);
                        Console.Write("Person's sex:");
                        fileCabinetRecord.Sex = inputParamsValidator.ReadInput(inputParamsValidator.SexConverter, inputParamsValidator.SexValidator);
                        Console.Write("Person's height:");
                        fileCabinetRecord.Height = inputParamsValidator.ReadInput(inputParamsValidator.HeightConverter, inputParamsValidator.HeightValidator);
                        Console.Write("Person's salary:");
                        fileCabinetRecord.Salary = inputParamsValidator.ReadInput(inputParamsValidator.SalaryConverter, inputParamsValidator.SalaryValidator);
                        service.EditRecord(fileCabinetRecord);
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
