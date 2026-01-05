namespace LowPressureZone.Core;

public static class Retry
{
    /// <summary>
    ///     Loops through an asynchronous function up to <paramref name="maxAttempts" /> times,
    ///     waiting <paramref name="delayMs" /> milliseconds between each attempt,
    ///     until the <paramref name="exitPredicate" /> returns true for the result of the function.
    /// </summary>
    /// <param name="maxAttempts">Total times to loop.</param>
    /// <param name="delayMs">Time to wait between loops.</param>
    /// <param name="exitPredicate">Condition for exiting the loop before <paramref name="maxAttempts" /> is reached.</param>
    /// <param name="asyncFunction">Function to retry.</param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T">The return type of <paramref name="asyncFunction" /></typeparam>
    /// <returns></returns>
    public static async Task<T> RetryAsync<T>(
        int maxAttempts,
        int delayMs,
        Func<T, bool> exitPredicate,
        Func<Task<T>> asyncFunction,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(maxAttempts, 0);
        ArgumentOutOfRangeException.ThrowIfLessThan(delayMs, 0);

        T result = default!;

        for (var i = 0; i < maxAttempts; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            result = await asyncFunction.Invoke();

            if (exitPredicate(result))
                return result;

            await Task.Delay(delayMs, cancellationToken);
        }

        return result;
    }
}