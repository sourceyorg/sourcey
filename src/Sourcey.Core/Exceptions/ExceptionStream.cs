using Microsoft.Extensions.Logging;
using Sourcey.Core.Extensions;

namespace Sourcey.Core.Exceptions;

internal sealed class ExceptionStream : IExceptionStream
{
    private readonly List<CachedException> _exceptions = new();
    private readonly ILogger<ExceptionStream> _logger;

    public ExceptionStream(ILogger<ExceptionStream> logger)
    {
        if (logger is null)
            throw new ArgumentNullException(nameof(logger));
        
        _logger = logger;
    }

    public void AddException<TException>(TException exception, CancellationToken cancellationToken = default) where TException : Exception
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation($"{nameof(ExceptionStream)}.{nameof(AddException)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }
        
        _exceptions.Add(new CachedException(typeof(TException).FriendlyFullName(), _exceptions.Count + 1, exception));
    }

    public Exception? GetLastException(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation($"{nameof(ExceptionStream)}.{nameof(GetLastException)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }
        
        return GetExceptions(cancellationToken)?.LastOrDefault();
    }

    public Exception? GetFirstException(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation($"{nameof(ExceptionStream)}.{nameof(GetFirstException)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }

        return GetExceptions(cancellationToken)?.FirstOrDefault();
    }

    public bool TryGetFirstExceptionByType<TException>(out TException? exception, CancellationToken cancellationToken = default) 
        where TException : Exception
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation($"{nameof(ExceptionStream)}.{nameof(TryGetFirstExceptionByType)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }
        
        if (TryGetExceptionsByType<TException>(out var exceptions, cancellationToken) && exceptions is not null)
        {
            exception = exceptions.First();
            return true;
        }

        exception = null;
        return false;
    }

    public bool TryGetLastExceptionByType<TException>(out TException? exception, CancellationToken cancellationToken = default)
        where TException : Exception
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation($"{nameof(ExceptionStream)}.{nameof(TryGetLastExceptionByType)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }
        
        if (TryGetExceptionsByType<TException>(out var exceptions, cancellationToken) && exceptions is not null)
        {
            exception = exceptions.Last();
            return true;
        }

        exception = null;
        return false;
    }

    public bool TryGetExceptionsByType<TException>(out IEnumerable<TException>? exceptions, CancellationToken cancellationToken = default)
        where TException : Exception
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation($"{nameof(ExceptionStream)}.{nameof(TryGetExceptionsByType)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }
        
        var key = typeof(TException).FriendlyFullName();

        exceptions = _exceptions.Where(e => e.Key == key)
            .OrderBy(e => e.Order)
            .Select(e => (TException)e.Exception);

        return exceptions?.Any() == true;
    }

    public IEnumerable<Exception> GetExceptions(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation($"{nameof(ExceptionStream)}.{nameof(GetExceptions)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }
        
        return _exceptions.OrderBy(e => e.Order)
            .Select(e => e.Exception);
    }

    public void ThrowAll(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation($"{nameof(ExceptionStream)}.{nameof(ThrowAll)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }

        if (_exceptions.Count < 1)
            return;

        if (_exceptions.Count < 2)
            throw _exceptions.First().Exception;

        throw new AggregateException(GetExceptions(cancellationToken));
    }

    public void ThrowFirst(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation($"{nameof(ExceptionStream)}.{nameof(ThrowFirst)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }

        if (_exceptions.Count < 1)
            return;

        throw _exceptions.First().Exception;
    }

    public void ThrowLast(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation($"{nameof(ExceptionStream)}.{nameof(ThrowLast)} was cancelled before execution");
            cancellationToken.ThrowIfCancellationRequested();
        }

        if (_exceptions.Count < 1)
            return;

        throw _exceptions.Last().Exception;
    }

    public void ThrowAll<TException>(CancellationToken cancellationToken = default)
        where TException : Exception
    {
        if (!TryGetExceptionsByType<TException>(out var exceptions, cancellationToken) || exceptions is null)
            return;

        throw new AggregateException(exceptions);
    }

    public void ThrowFirst<TException>(CancellationToken cancellationToken = default)
        where TException : Exception
    {
        if (!TryGetExceptionsByType<TException>(out var exceptions, cancellationToken) || exceptions is null)
            return;

        throw exceptions.First();
    }

    public void ThrowLast<TException>(CancellationToken cancellationToken = default)
        where TException : Exception
    {
        if (!TryGetExceptionsByType<TException>(out var exceptions, cancellationToken) || exceptions is null)
            return;

        throw exceptions.Last();
    }

    public bool Any(CancellationToken cancellationToken)
        => _exceptions.Any();
}

internal record CachedException(string Key, int Order, Exception Exception);
