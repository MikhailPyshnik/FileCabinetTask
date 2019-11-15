using System;

namespace FileCabinetApp.CommonValidator
{
    /// <summary>
    /// Class HeightValidator.
    /// </summary>
    public class HeightValidator : IRecordValidator
    {
        private readonly short minHeight;
        private readonly short maxHeight;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeightValidator"/> class.
        /// </summary>
        /// <param name="minHeight">Input parametr min height.<see cref="short"/>.</param>
        /// <param name="maxHeight">Input parametr max height.<see cref="short"/>.</param>
        public HeightValidator(short minHeight, short maxHeight)
        {
            this.minHeight = minHeight;
            this.maxHeight = maxHeight;
        }

        /// <summary>
        /// Implements the method ValidateParameters.
        /// </summary>
        /// <param name="fileCabinetRecord">Input parameter record <see cref="FileCabinetRecord"/>.</param>
        public void ValidateParametrs(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord == null)
            {
                throw new ArgumentNullException($"{nameof(fileCabinetRecord)} is null.");
            }

            var value = fileCabinetRecord.Height;
            if (value < this.minHeight || value > this.maxHeight)
            {
                throw new ArgumentException("Height is incorrect value.");
            }
        }
    }
}
