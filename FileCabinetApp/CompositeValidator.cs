using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// Base class CompositeValidator.
    /// </summary>
    public class CompositeValidator : IRecordValidator
    {
        private readonly List<IRecordValidator> validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidator"/> class.
        /// </summary>
        /// <param name="validators">Input parametr list.<see cref="IRecordValidator"/>.</param>
        public CompositeValidator(List<IRecordValidator> validators)
        {
            this.validators = validators ?? throw new ArgumentNullException(nameof(validators));
        }

        /// <summary>
        /// Implements the method ValidateParametrs.
        /// </summary>
        /// <param name="fileCabinetRecord">Input parameter record <see cref="FileCabinetRecord"/>.</param>
        public void ValidateParametrs(FileCabinetRecord fileCabinetRecord)
        {
            foreach (var validator in this.validators)
            {
                validator.ValidateParametrs(fileCabinetRecord);
            }
        }
    }
}
