﻿using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Create command.
    /// </summary>
    public class CreateCommandHandler : ServiceCommandHandlerBase
    {
        private static IValidatorOfParemetrs inputParamsValidator;
        private static Action<string> action;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Input parametr start id.<see cref="IFileCabinetService"/>.</param>
        /// <param name="printMethod">Input delegate Action for print messages.<see cref="Action"/>.</param>
        /// <param name="recordValidator">Input parametr amount of records.<see cref="IValidatorOfParemetrs"/>.</param>
        public CreateCommandHandler(IFileCabinetService fileCabinetService, Action<string> printMethod, IValidatorOfParemetrs recordValidator)
             : base(fileCabinetService)
        {
            inputParamsValidator = recordValidator ?? throw new ArgumentNullException(nameof(recordValidator));
            action = printMethod ?? throw new ArgumentNullException(nameof(printMethod));
        }

        /// <summary>
        /// Override method Handle by CommandHandlerBase in CreateCommandHandler.
        /// </summary>
        /// <param name="appCommandRequest">Input parametr record <see cref="AppCommandRequest"/>.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest is null)
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
                    int result = service.CreateRecord(fileCabinetRecord);
                    if (result > 0)
                    {
                        action($"Record #{result} is created.");
                    }
                }
                catch (ArgumentNullException ex)
                {
                    action($"{ex.Message} Enter the data again!");
                    Create(parameters);
                }
                catch (ArgumentException ex)
                {
                    action($"{ex.Message} Enter the data again!");
                    Create(parameters);
                }
                catch (Exception ex)
                {
                    action(ex.Message);
                    throw new ArgumentException("Input value in Create is incorrect! Select  command again.");
                }
            }
            catch (ArgumentException ex)
            {
                action($"{ex.Message}");
            }
        }
    }
}
