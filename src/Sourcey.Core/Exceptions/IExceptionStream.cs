namespace Sourcey.Core.Exceptions;

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
    bool Any(CancellationToken cancellationToken);
    Exception? GetLastException(CancellationToken cancellationToken = default);
    Exception? GetFirstException(CancellationToken cancellationToken = default);
    IEnumerable<Exception> GetExceptions(CancellationToken cancellationToken = default);
    void ThrowAll(CancellationToken cancellationToken = default);
    void ThrowFirst(CancellationToken cancellationToken = default);
    void ThrowLast(CancellationToken cancellationToken = default);
    void ThrowAll<TException>(CancellationToken cancellationToken = default)
        where TException : Exception;
    void ThrowFirst<TException>(CancellationToken cancellationToken = default)
        where TException : Exception;
    void ThrowLast<TException>(CancellationToken cancellationToken = default)
        where TException : Exception;
}
