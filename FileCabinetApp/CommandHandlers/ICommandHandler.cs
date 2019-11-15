using System;

namespace FileCabinetApp
{
    /// <summary>
    ///  Contains methods SetNext and Handle.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// Method SetNext.
        /// </summary>
        /// <param name="commandHandler">Input parametr record <see cref="ICommandHandler"/>.</param>
        /// <returns>Id <see cref="int"/>.</returns>
        ICommandHandler SetNext(ICommandHandler commandHandler);

        /// <summary>
        /// Method Handle.
        /// </summary>
        /// <param name="appCommandRequest">Input parametr record <see cref="AppCommandRequest"/>.</param>
        void Handle(AppCommandRequest appCommandRequest);
    }
}
