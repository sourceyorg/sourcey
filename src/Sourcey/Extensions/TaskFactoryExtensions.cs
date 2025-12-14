namespace Sourcey.Extensions;

public static class TaskFactoryExtensions
{
    public static async ValueTask<bool> WithRetryAsync(
        this Func<Task<bool>> func,
        int retryCount,
        TimeSpan delay,
        CancellationToken cancellationToken = default)
    {
        if (func == null)
            throw new ArgumentNullException(nameof(func));
        if (retryCount < 0)
            throw new ArgumentOutOfRangeException(nameof(retryCount));
        if (delay < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(delay));

        using var timer = new PeriodicTimer(delay);

        while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false)) {
            if (await func().ConfigureAwait(false))
                return true;

            if (retryCount == 0)
                break;

            retryCount--;
        }

        return false;
    }
    
    public static async ValueTask<(bool success, T? result)> WithRetryAsync<T>(
        this Func<Task<T>> func,
        Func<T, bool> validityCheck,
        int retryCount,
        TimeSpan delay,
        CancellationToken cancellationToken = default)
    {
        if (func == null)
            throw new ArgumentNullException(nameof(func));
        if (validityCheck == null)
            throw new ArgumentNullException(nameof(validityCheck));
        if (retryCount < 0)
            throw new ArgumentOutOfRangeException(nameof(retryCount));
        if (delay < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(delay));

        T? result = default;

        using var timer = new PeriodicTimer(delay);

        while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false)) {
            result = await func().ConfigureAwait(false);

            if (validityCheck(result))
                return (true, result);

            if (retryCount == 0)
                break;

            retryCount--;
        }

        return (false, result);
    }

    public static async ValueTask<(bool success, T? result)> WithRetryAsync<T>(
        this Func<Task<T>> func,
        Func<T, ValueTask<bool>> validityCheck,
        int retryCount,
        TimeSpan delay,
        CancellationToken cancellationToken = default)
    {
        if (func == null)
            throw new ArgumentNullException(nameof(func));
        if (validityCheck == null)
            throw new ArgumentNullException(nameof(validityCheck));
        if (retryCount < 0)
            throw new ArgumentOutOfRangeException(nameof(retryCount));
        if (delay < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(delay));

        T? result = default;

        using var timer = new PeriodicTimer(delay);

        while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false)) {
            result = await func().ConfigureAwait(false);

            if (await validityCheck(result).ConfigureAwait(false))
                return (true, result);

            if (retryCount == 0)
                break;

            retryCount--;
        }

        return (false, result);
    }

    public static async ValueTask<(bool success, T? result)> WithRetryAsync<T>(
        this Func<CancellationToken, Task<T>> func,
        Func<T, bool> validityCheck,
        int retryCount,
        TimeSpan delay,
        CancellationToken cancellationToken = default)
    {
        if (func == null)
            throw new ArgumentNullException(nameof(func));
        if (validityCheck == null)
            throw new ArgumentNullException(nameof(validityCheck));
        if (retryCount < 0)
            throw new ArgumentOutOfRangeException(nameof(retryCount));
        if (delay < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(delay));

        T? result = default;

        using var timer = new PeriodicTimer(delay);

        while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false)) {
            result = await func(cancellationToken).ConfigureAwait(false);

            if (validityCheck(result))
                return (true, result);

            if (retryCount == 0)
                break;

            retryCount--;
        }

        return (false, result);
    }

    public static async ValueTask<(bool success, T? result)> WithRetryAsync<T>(
        this Func<ValueTask<T>> func,
        Func<T, bool> validityCheck,
        int retryCount,
        TimeSpan delay,
        CancellationToken cancellationToken = default)
    {
        if (func == null)
            throw new ArgumentNullException(nameof(func));
        if (validityCheck == null)
            throw new ArgumentNullException(nameof(validityCheck));
        if (retryCount < 0)
            throw new ArgumentOutOfRangeException(nameof(retryCount));
        if (delay < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(delay));

        T? result = default;

        using var timer = new PeriodicTimer(delay);

        while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false)) {
            result = await func().ConfigureAwait(false);

            if (validityCheck(result))
                return (true, result);

            if (retryCount == 0)
                break;

            retryCount--;
        }

        return (false, result);
    }

    public static async ValueTask<(bool success, T? result)> WithRetryAsync<T>(
        this Func<ValueTask<T>> func,
        Func<T, ValueTask<bool>> validityCheck,
        int retryCount,
        TimeSpan delay,
        CancellationToken cancellationToken = default)
    {
        if (func == null)
            throw new ArgumentNullException(nameof(func));
        if (validityCheck == null)
            throw new ArgumentNullException(nameof(validityCheck));
        if (retryCount < 0)
            throw new ArgumentOutOfRangeException(nameof(retryCount));
        if (delay < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(delay));

        T? result = default;

        using var timer = new PeriodicTimer(delay);

        while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false)) {
            result = await func().ConfigureAwait(false);

            if (await validityCheck(result).ConfigureAwait(false))
                return (true, result);

            if (retryCount == 0)
                break;

            retryCount--;
        }

        return (false, result);
    }

    public static async ValueTask<(bool success, T? result)> WithRetryAsync<T>(
        this Func<CancellationToken, ValueTask<T>> func,
        Func<T, bool> validityCheck,
        int retryCount,
        TimeSpan delay,
        CancellationToken cancellationToken = default)
    {
        if (func == null)
            throw new ArgumentNullException(nameof(func));
        if (validityCheck == null)
            throw new ArgumentNullException(nameof(validityCheck));
        if (retryCount < 0)
            throw new ArgumentOutOfRangeException(nameof(retryCount));
        if (delay < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(delay));

        T? result = default;

        using var timer = new PeriodicTimer(delay);

        while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false)) {
            result = await func(cancellationToken).ConfigureAwait(false);

            if (validityCheck(result))
                return (true, result);

            if (retryCount == 0)
                break;

            retryCount--;
        }

        return (false, result);
    }

    public static async ValueTask<(bool success, T? result)> WithRetryAsync<T>(
        this Func<CancellationToken, ValueTask<T>> func,
        Func<T, ValueTask<bool>> validityCheck,
        int retryCount,
        TimeSpan delay,
        CancellationToken cancellationToken = default)
    {
        if (func == null)
            throw new ArgumentNullException(nameof(func));
        if (validityCheck == null)
            throw new ArgumentNullException(nameof(validityCheck));
        if (retryCount < 0)
            throw new ArgumentOutOfRangeException(nameof(retryCount));
        if (delay < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(delay));

        T? result = default;

        using var timer = new PeriodicTimer(delay);

        while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false)) {
            result = await func(cancellationToken).ConfigureAwait(false);

            if (await validityCheck(result).ConfigureAwait(false))
                return (true, result);

            if (retryCount == 0)
                break;

            retryCount--;
        }

        return (false, result);
    }
}
