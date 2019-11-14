using System;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Base class ServiceCommandHandlerBase.
    /// </summary>
    public abstract class ServiceCommandHandlerBase : CommandHandlerBase
    {
        /// <summary>
        /// Base service for CommandHandler classes.
        /// </summary>
#pragma warning disable CA2211 // Non-constant fields should not be visible
#pragma warning disable SA1401 // Fields should be private
        protected static IFileCabinetService service;
#pragma warning restore SA1401 // Fields should be private
#pragma warning restore CA2211 // Non-constant fields should not be visible

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCommandHandlerBase"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Input parametr start id.<see cref="IFileCabinetService"/>.</param>
        protected ServiceCommandHandlerBase(IFileCabinetService fileCabinetService)
        {
            service = fileCabinetService;
        }
    }
}
