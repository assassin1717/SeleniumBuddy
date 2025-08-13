namespace SeleniumBuddy.Abstractions
{
    /// <summary>
    /// Global configuration for SeleniumBuddy behaviors (timeouts, polling, retries, etc.).
    /// </summary>
    public interface ISeleniumBuddyOptions
    {
        /// <summary>Default maximum time to wait for waits/interactions.</summary>
        TimeSpan DefaultTimeout { get; }

        /// <summary>Polling interval used by wait loops.</summary>
        TimeSpan PollingInterval { get; }

        /// <summary>Number of retry attempts for flaky interactions.</summary>
        int RetryAttempts { get; }

        /// <summary>Base backoff used between retries (e.g., exponential backoff may use this as a seed).</summary>
        TimeSpan RetryBaseBackoff { get; }

        /// <summary>Capture screenshot automatically when an interaction fails after retries.</summary>
        bool ScreenshotOnFailure { get; }
    }
}
