namespace FileCabinetApp.Iterators
{
    /// <summary>
    /// Interface for iterations.
    /// </summary>
    public interface IRecorditerator
    {
        /// <summary>
        /// Return next valueif HasMore() is true.
        /// </summary>
        /// <returns>Rerord by firstName <see cref="FileCabinetRecord"/>.</returns>
        FileCabinetRecord GetNext();

        /// <summary>
        /// Possible next value.
        /// </summary>
        /// <returns>Rerords by firstName <see cref="bool"/>.</returns>
        bool HasMore();
    }
}
