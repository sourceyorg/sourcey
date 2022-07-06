﻿namespace Zion.Core.Exceptions
{
    public interface IExceptionStream
    {
        void AddException<TException>(TException exception, CancellationToken cancellationToken = default)
            where TException : Exception;        
        bool TryGetLastExceptionByType<TException>(out TException? exception, CancellationToken cancellationToken = default)
            where TException : Exception;
        bool TryGetFirstExceptionByType<TException>(out TException? exception, CancellationToken cancellationToken = default)
            where TException : Exception;
        bool TryGetExceptionsByType<TException>(out IEnumerable<TException>? exceptions, CancellationToken cancellationToken = default)
            where TException : Exception;
        Exception? GetLastException(CancellationToken cancellationToken = default);
        Exception? GetFirstException(CancellationToken cancellationToken = default);
        IEnumerable<Exception> GetExceptions(CancellationToken cancellationToken = default);
    }
}
