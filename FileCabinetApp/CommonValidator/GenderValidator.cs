using System;

namespace FileCabinetApp.CommonValidator
{
    /// <summary>
    /// Class GenderValidator.
    /// </summary>
    public class GenderValidator : IRecordValidator
    {
        private readonly char maleSex;
        private readonly char femaleSex;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenderValidator"/> class.
        /// </summary>
        /// <param name="maleSex">Input parameter male sex.<see cref="char"/>.</param>
        /// <param name="femaleSex">Input parameter female sex.<see cref="char"/>.</param>
        public GenderValidator(char maleSex, char femaleSex)
        {
            this.maleSex = maleSex;
            this.femaleSex = femaleSex;
        }

        /// <summary>
        /// Implements the method ValidateParametrs.
        /// </summary>
        /// <param name="fileCabinetRecord">Input parameter record <see cref="FileCabinetRecord"/>.</param>
        public void ValidateParametrs(FileCabinetRecord fileCabinetRecord)
        {
            if (fileCabinetRecord == null)
            {
                throw new ArgumentNullException($"{nameof(fileCabinetRecord)} is null.");
            }

            var value = fileCabinetRecord.Sex;
            if (!this.maleSex.Equals(value) && !this.femaleSex.Equals(value) && !'F'.Equals(value) && !'M'.Equals(value))
            {
                throw new ArgumentException("Sex is incorrect value.");
            }
        }
    }
}
