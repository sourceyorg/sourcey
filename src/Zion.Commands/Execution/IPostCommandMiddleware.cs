﻿namespace Zion.Commands.Execution
{
    internal interface IPostCommandMiddleware<TCommand> : ICommandHandlerMiddleware<TCommand>
        where TCommand : ICommand
    {
    }
}
