using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Base class CommandHandler.
    /// </summary>
    public abstract class CommandHandlerBase : ICommandHandler
    {
        private ICommandHandler nextHandler;

        /// <summary>
        ///  Implementation method SetNext by ICommandHandler in CommandHandlerBase..
        /// </summary>
        /// <param name="commandHandler">Input parametr record <see cref="ICommandHandler"/>.</param>
        /// <returns>Id <see cref="int"/>.</returns>
        public ICommandHandler SetNext(ICommandHandler commandHandler)
        {
            this.nextHandler = commandHandler;
            return this.nextHandler;
        }

        /// <summary>
        /// Implementation method Handle by ICommandHandler in CommandHandlerBase.
        /// </summary>
        /// <param name="appCommandRequest">Input parametr record <see cref="AppCommandRequest"/>.</param>
        public virtual void Handle(AppCommandRequest appCommandRequest)
        {
            if (this.nextHandler != null)
            {
               this.nextHandler.Handle(appCommandRequest);
            }
            else
            {
                this.nextHandler = null;
            }
        }
    }
}
