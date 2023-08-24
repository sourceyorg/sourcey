﻿namespace Sourcey.Commands.Execution
{
    public abstract class CommandException : Exception
    {
        public CommandException(string message) : base(message) { }
        public CommandException(string message, Exception innerException) : base(message, innerException) { }
    }
}
