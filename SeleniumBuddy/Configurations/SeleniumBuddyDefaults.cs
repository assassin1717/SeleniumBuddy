namespace SeleniumBuddy.Configurations
{
    /// <summary>Centralized default values for SeleniumBuddy configuration.</summary>
    public static class SeleniumBuddyDefaults
    {
        /// <summary>Default maximum time to wait for waits/interactions.</summary>
        public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);

        /// <summary>Default polling interval for wait loops.</summary>
        public static readonly TimeSpan PollingInterval = TimeSpan.FromMilliseconds(250);

        /// <summary>Default number of retry attempts for flaky interactions.</summary>
        public const int RetryAttempts = 2;

        /// <summary>Default base backoff between retries.</summary>
        public static readonly TimeSpan RetryBaseBackoff = TimeSpan.FromMilliseconds(300);

        /// <summary>Whether to capture a screenshot automatically on failure by default.</summary>
        public const bool ScreenshotOnFailure = true;
    }
}
