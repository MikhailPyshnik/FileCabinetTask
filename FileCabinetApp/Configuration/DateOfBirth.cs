using System;

namespace FileCabinetApp.Configuration
{
    /// <summary>
    /// Work DateOfBirth.
    /// </summary>
    public class DateOfBirth
    {
        /// <summary>
        /// Gets or sets the start DateOfBirth.
        /// </summary>
        /// <value>The start DateOfBirth.</value>
        public DateTime From { get; set; }

        /// <summary>
        /// Gets or sets the end DateOfBirth.
        /// </summary>
        /// <value>The end DateOfBirth.</value>
        public DateTime To { get; set; }
    }
}
