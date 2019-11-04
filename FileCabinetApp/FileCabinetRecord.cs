using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Work FileCabinetRecord.
/// </summary>
// [Serializable]
public class FileCabinetRecord
{
    /// <summary>
    /// Gets or sets the Id of the record.
    /// </summary>
    /// <value>The Id of the record.</value>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the FirstName of the record.
    /// </summary>
    /// <value>The FirstName of the record.</value>
    public string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the LastName of the record.
    /// </summary>
    /// <value>The LastName of the record.</value>
    public string LastName { get; set; }

    /// <summary>
    /// Gets or sets the DateOfBirth of the record.
    /// </summary>
    /// <value>The DateOfBirth of the record.</value>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Gets or sets the Sex of the record.
    /// </summary>
    /// <value>The Sex of the record.</value>
    public char Sex { get; set; }

    /// <summary>
    /// Gets or sets the Height of the record.
    /// </summary>
    /// <value>The Height of the record.</value>
    public short Height { get; set; }

    /// <summary>
    /// Gets or sets the Salary of the record.
    /// </summary>
    /// <value>The Salary of the record.</value>
    public decimal Salary { get; set; }
}
