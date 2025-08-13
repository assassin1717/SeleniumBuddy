namespace SeleniumBuddy.Abstractions
{
    /// <summary>
    /// Retry/backoff policy for flaky UI operations.
    /// </summary>
    public interface IRetryPolicy
    {
        /// <summary>Executes the action with retries/backoff. Throws the last exception on failure.</summary>
        Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken ct = default);

        /// <summary>Executes the function with retries/backoff. Throws the last exception on failure.</summary>
        Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken ct = default);
    }
}
