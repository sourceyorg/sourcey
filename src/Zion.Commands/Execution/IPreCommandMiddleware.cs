﻿namespace Zion.Commands.Execution
{
    internal interface IPreCommandMiddleware<TCommand> : ICommandHandlerMiddleware<TCommand>
        where TCommand : ICommand
    {
    }
}