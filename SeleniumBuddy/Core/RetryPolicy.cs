using OpenQA.Selenium;
using SeleniumBuddy.Abstractions;

namespace SeleniumBuddy.Core
{
    /// <summary>
    /// Default retry/backoff policy for flaky Selenium operations.
    /// Retries common transient Selenium exceptions with exponential backoff.
    /// </summary>
    public sealed class RetryPolicy : IRetryPolicy
    {
        private readonly ISeleniumBuddyOptions _options;

        public RetryPolicy(ISeleniumBuddyOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken ct = default)
        {
            _ = action ?? throw new ArgumentNullException(nameof(action));
            await ExecuteAsync<object?>(async c =>
            {
                await action(c).ConfigureAwait(false);
                return null;
            }, ct).ConfigureAwait(false);
        }

        public async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken ct = default)
        {
            _ = action ?? throw new ArgumentNullException(nameof(action));

            var attempts = Math.Max(0, _options.RetryAttempts) + 1; // initial try + retries
            Exception? last = null;

            for (int tryIndex = 0; tryIndex < attempts; tryIndex++)
            {
                ct.ThrowIfCancellationRequested();

                try
                {
                    return await action(ct).ConfigureAwait(false);
                }
                catch (Exception ex) when (IsRetryable(ex))
                {
                    last = ex;

                    // last attempt → break and rethrow below
                    if (tryIndex == attempts - 1)
                        break;

                    // exponential backoff: base * 2^(tryIndex)
                    var delay = TimeSpan.FromMilliseconds(
                        Math.Max(0, _options.RetryBaseBackoff.TotalMilliseconds) * Math.Pow(2, tryIndex)
                    );

                    if (delay > TimeSpan.Zero)
                        await Task.Delay(delay, ct).ConfigureAwait(false);
                }
            }

            // if we reached here, we failed all attempts
            throw last ?? new InvalidOperationException("RetryPolicy failed without captured exception.");
        }

        private static bool IsRetryable(Exception ex)
        {
            // Common transient Selenium issues that benefit from retrying
            return ex switch
            {
                NoSuchElementException => true,
                StaleElementReferenceException => true,
                ElementClickInterceptedException => true,
                ElementNotInteractableException => true,
                InvalidOperationException => true,
                TimeoutException => true,
                WebDriverException => true,
                _ => false
            };
        }
    }
}
